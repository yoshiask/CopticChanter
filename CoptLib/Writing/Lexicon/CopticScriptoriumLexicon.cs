using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using CoptLib.Models.Text;
using Microsoft.Data.Sqlite;
using OwlCore.ComponentModel;

namespace CoptLib.Writing.Lexicon;

public class CopticScriptoriumLexicon : ILexicon, IAsyncInit
{
    private static readonly string _dbPath;
    private static readonly SqliteConnection _db;

    static CopticScriptoriumLexicon()
    {
        var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        _dbPath = Path.Combine(appDataDir, "CopticLib", "sahidic_dict.db");
        
        _db = new SqliteConnection($"Data Source={_dbPath}");
    }

    public LanguageInfo Language { get; } = new(KnownLanguage.Coptic);
    
    public bool IsInitialized { get; private set; }

    public async IAsyncEnumerable<LexiconEntry> SearchAsync(string query, LanguageInfo usage, PartOfSpeech? partOfSpeech = null,
        [EnumeratorCancellation] CancellationToken token = default)
    {
        var command = _db.CreateCommand();
        command.CommandText = @"SELECT * FROM entries WHERE search MATCH @query 
            OR en MATCH @query 
            OR de MATCH @query 
            OR fr MATCH @query 
            ORDER BY rank";
        command.Parameters.AddWithValue("query", query);

        using var reader = await command.ExecuteReaderAsync(token);
        while (await reader.ReadAsync(token))
        {
            token.ThrowIfCancellationRequested();
            var entry =  ReadLexiconEntry(reader);

            Lazy<bool> matchesPos = new(() =>
                partOfSpeech == null || entry.GrammarGroup.PartOfSpeech == partOfSpeech);
            Lazy<bool> matchesUsage = new(() =>
                entry.Forms.Any(f => f.Usage.IsEquivalentTo(usage, LanguageEquivalencyOptions.StrictWithWild)));
            
            if (matchesPos.Value && matchesUsage.Value)
                yield return entry;
        }
    }

    public async IAsyncEnumerable<ILexiconEntry> GetEntriesAsync([EnumeratorCancellation] CancellationToken token = default)
    {
        var command = _db.CreateCommand();
        command.CommandText = @"SELECT * FROM entries";

        using var reader = await command.ExecuteReaderAsync(token);
        while (await reader.ReadAsync(token))
        {
            token.ThrowIfCancellationRequested();
            yield return ReadLexiconEntry(reader);
        }
    }

    public async Task InitAsync(CancellationToken cancellationToken = default)
    {
        if (IsInitialized) return;

        if (_db.State != ConnectionState.Closed)
        {
            IsInitialized = true;
            return;
        }

        // Copy DB from resources onto disk
        var assembly = typeof(ILexicon).GetTypeInfo().Assembly;
        Directory.CreateDirectory(Path.GetDirectoryName(_dbPath));
        using (var fileStream = File.OpenWrite(_dbPath))
        using (var dbStream =
                     assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Resources.sahidic_dict.db"))
        {
            Guard.IsNotNull(dbStream);

            await dbStream.CopyToAsync(fileStream);
        }
        
        // Connect to DB
        await _db.OpenAsync(cancellationToken);

        IsInitialized = true;
    }

    private static LexiconEntry ReadLexiconEntry(IDataRecord reader)
    {
        var id = reader.GetString(0);
        var name = reader.GetString(2);
        var pos = reader.GetString(3);
        var de = reader.GetString(4);
        var en = reader.GetString(5);
        var fr = reader.GetString(6);
        var etym = reader.GetString(7);
        var search = reader.GetString(9);

        GrammarGroup grammarGroup = new(ParsePartOfSpeech(pos), default, null, null, null, null);
        var senses = ParseSenses(new[]
        {
            (new LanguageInfo(KnownLanguage.German), de),
            (new LanguageInfo(KnownLanguage.English), en),
            (new LanguageInfo(KnownLanguage.French), fr),
        }).ToList();

        var forms = ParseForms(search).ToList();
            
        return new LexiconEntry(id, default, forms, senses, grammarGroup);
    }
    
    private static PartOfSpeech ParsePartOfSpeech(string pos)
    {
        return pos switch
        {
            "N" => PartOfSpeech.Substantive,
            "EXIST" or "V" or "VBD" or "VIMP" or "VSTAT" => PartOfSpeech.Verb,
            "ADV" => PartOfSpeech.Adverb,
            "ADJ" => PartOfSpeech.Adjective,
            "PREP" => PartOfSpeech.Preposition,
            "PRON" => PartOfSpeech.Pronoun,
            "PPERO" => PartOfSpeech.PossessivePronoun,
            "PINT" => PartOfSpeech.InterrogativePronoun,
            "PDEM" => PartOfSpeech.DemonstrativePronoun,
            "PPER" => PartOfSpeech.PersonalPronoun,
            "CONJ" => PartOfSpeech.Conjugation,
            "ART" => PartOfSpeech.DefiniteArticle,
            "NUM" => PartOfSpeech.Numeral,
            "PPOS" => PartOfSpeech.PossessiveArticle,
            "A" => PartOfSpeech.Prefix,
            "PTC" => PartOfSpeech.Particle,
            "C" => PartOfSpeech.SentenceConverter,
            
            "NEG" or "NONE" or "NULL" or
            "" or "?" => PartOfSpeech.Unknown,
            
            _ => throw new NotImplementedException()
        };
    }

    private static (string Id, string Definition, string Bibliography) ParseSensePart(string senseText)
    {
        var senseIdStart = senseText.IndexOf('@') + 1;
        var senseIdEnd = senseText.IndexOf('|');
        var senseId = senseText.Substring(senseIdStart, senseIdEnd - senseIdStart);

        var defMarkerIdx = senseText.IndexOf("~~~", StringComparison.Ordinal);
        var defStart = defMarkerIdx + 3;
        var defEnd = senseText.IndexOf(";;;", StringComparison.Ordinal);

        bool hasBib = defEnd > 0;
        if (!hasBib)
            defEnd = senseText.Length;

        var def = defMarkerIdx > 0
            ? senseText.Substring(defStart, defEnd - defStart)
            : string.Empty;

        string bib;
        if (hasBib)
        {
            var bibStart = defEnd + 3;
            bib = senseText.Substring(bibStart, senseText.Length - bibStart);
        }
        else
        {
            bib = string.Empty;
        }

        return new(senseId, def, bib);
    }
    
    private static IEnumerable<Sense> ParseSenses(IEnumerable<(LanguageInfo, string)> senseTable)
    {
        Dictionary<string, Sense> senses = new();
        // Example:
        // 1@CS15873|ref: 5873 ~~~to be obedient, subservient (object: none);;;DDGLC |||
        // 1@CS13406|ref: 860 ~~~Pentecost, (festival of) the fiftieth day after Easter (Jewish festival);;;DDGLC |||

        foreach (var (lang, text) in senseTable)
        {
            var parts = text
                .Split(new[] {" |||"}, StringSplitOptions.None);
        
            foreach (var partText in parts)
            {
                var part = ParseSensePart(partText);

                var createNew = !senses.TryGetValue(part.Id, out var sense);
                if (createNew || sense is null)
                    sense = new(new(), part.Bibliography);

                sense.Translations.Add(new Run(part.Definition, sense.Translations)
                {
                    Language = lang,
                });

                if (createNew)
                    senses[part.Id] = sense;
            }
        }

        return senses.Values;
    }

    private static Form ParseForm(string formText)
    {
        formText = formText.Trim();
        // Example:
        // ⲭⲟⲣⲟⲥ~S^^CF30042

        var orthEnd = formText.IndexOf('~');
        var orth = formText[..orthEnd];

        var diaStart = orthEnd + 1;
        var diaEnd = formText.IndexOf('^');
        var dia = formText[diaStart..diaEnd];

        return new(FormType.Lemma, TeiLexicon.MapDialectCode(dia), orth);
    }

    private static IEnumerable<Form> ParseForms(string formsText) =>
        formsText.Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries).Select(ParseForm);
}