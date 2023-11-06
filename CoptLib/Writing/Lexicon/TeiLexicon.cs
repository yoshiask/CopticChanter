using CoptLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using CoptLib.Models;
using CoptLib.Models.Text;
using OwlCore.Extensions;

namespace CoptLib.Writing.Lexicon;

public class TeiLexicon : ILexicon
{
    private readonly XDocument _teiDoc;
    private List<ILexiconEntry>? _entries;
    private Dictionary<string, LanguageInfo> _usageMappings;

    public TeiLexicon(LanguageInfo language, XDocument teiDoc, Dictionary<string, LanguageInfo> usageMappings)
    {
        Language = language;
        _teiDoc = teiDoc;
        _usageMappings = usageMappings;
    }

    public LanguageInfo Language { get; }

    public async IAsyncEnumerable<ILexiconEntry> GetEntriesAsync(
        [EnumeratorCancellation] CancellationToken token = default)
    {
        if (_entries is null)
        {
            _entries = new();

            var elements = _teiDoc.Root!
                .ElementLocal("text")!
                .ElementLocal("body")!
                .Elements()
                .ToAsyncEnumerable();

            await foreach (var iLexEnElem in elements.WithCancellation(token))
            {
                ILexiconEntry entry;
                if (iLexEnElem.Name.LocalName == "superEntry")
                    entry = new LexiconSuperEntry(iLexEnElem.Elements().Select(ParseSingleEntry).Cast<LexiconEntry>());
                else
                    entry = ParseSingleEntry(iLexEnElem);

                _entries.Add(entry);
                yield return entry;
            }
        }
        else
        {
            foreach (var entry in _entries)
                yield return entry;
        }
    }

    public IAsyncEnumerable<LexiconEntry> SearchAsync(string query, LanguageInfo usage, PartOfSpeech? partOfSpeech = null,
        CancellationToken token = default)
    {
        return GetEntriesAsync(token)
            .SelectMany(e =>
            {
                return e switch
                {
                    LexiconSuperEntry superEntry => superEntry.Entries.ToAsyncEnumerable(),
                    LexiconEntry entry => entry.IntoList().ToAsyncEnumerable(),
                    _ => throw new ArgumentException(nameof(e))
                };
            })
            .Where(IsMatch);

        bool IsMatch(LexiconEntry entry)
        {
            if (partOfSpeech is not null && entry.GrammarGroup.PartOfSpeech != partOfSpeech)
                return false;

            if (!entry.Forms.PruneNull().Any(f => f.Usage.IsEquivalentTo(usage, LanguageEquivalencyOptions.StrictWithWild)))
                return false;

            return entry.Forms.Any(f => f.Orthography.Contains(query))
                   || entry.Senses.Any(f => f.Translations.Any(t => t.ToString().Contains(query)));
        }
    }

    private LexiconEntry ParseSingleEntry(XElement entryElem)
    {
        var id = entryElem.AttributeLocal("id")?.Value;
        
        var typeStr = entryElem.AttributeLocal("type")?.Value;
        var type = typeStr is null
            ? EntryType.Hom
            : (EntryType) Enum.Parse(typeof(EntryType), typeStr, true);

        List<Form> forms = new();
        List<Sense> senses = new();
        GrammarGroup grammarGroup = null;

        foreach (var elem in entryElem.Elements())
        {
            switch (elem.Name.LocalName)
            {
                case "form":
                    var orth = elem.ElementLocal("orth")?.Value;

                    var formTypeStr = elem.AttributeLocal("type")?.Value;
                    var formType = formTypeStr is null
                        ? FormType.Lemma
                        : (FormType) Enum.Parse(typeof(FormType), formTypeStr, true);

                    var usg = MapDialectCode(elem.ElementLocal("usg")?.Value);

                    forms.Add(new Form(formType, usg, orth));
                    break;

                case "gramGrp":
                    var subc = elem.ElementLocal("subc")?.Value;
                    var note = elem.ElementLocal("note")?.Value;

                    PartOfSpeech pos = elem.ElementLocal("pos")?.Value?.TrimEnd('?') switch
                    {
                        "Subst." => PartOfSpeech.Substantive,
                        "Vb." => PartOfSpeech.Verb,
                        "Adv." => PartOfSpeech.Adverb,
                        "Adj." => PartOfSpeech.Adjective,
                        "Präp." => PartOfSpeech.Preposition,
                        "Pron." => PartOfSpeech.Pronoun,
                        "Poss. Pron." => PartOfSpeech.PossessivePronoun,
                        "Interr. Pron." => PartOfSpeech.InterrogativePronoun,
                        "Dem. Pron." => PartOfSpeech.DemonstrativePronoun,
                        "Selbst. Pers. Pron." => PartOfSpeech.PersonalPronoun,
                        "Konj." => PartOfSpeech.Conjugation,
                        "bestimmter Artikel" => PartOfSpeech.DefiniteArticle,
                        "Zahlzeichen" => PartOfSpeech.NumberSign,
                        "Zahlwort" => PartOfSpeech.Numeral,
                        "Possessivartikel" => PartOfSpeech.PossessiveArticle,
                        "Suffixpronomen" => PartOfSpeech.PronounSuffix,
                        "Präfix" => PartOfSpeech.Prefix,
                        "Nominalpräfix" => PartOfSpeech.NominalPrefix,
                        "Verbalpräfix" => PartOfSpeech.VerbalPrefix,
                        "Adjektivpräfix" => PartOfSpeech.AdjectivePrefix,
                        "Präfixpronomen (Präsens I)" => PartOfSpeech.PronounPrefixPresent1,
                        "Präfix der Ordinalzahlen" => PartOfSpeech.OrdinalPrefix,
                        "Possessivpräfix" => PartOfSpeech.PossessivePrefix,
                        "Konjunktiv" => PartOfSpeech.Conjunctive,
                        "Partikel" or "Partikel, enklitisch" => PartOfSpeech.Particle,
                        "Satzkonverter" => PartOfSpeech.SentenceConverter,
                        "Interjektion" => PartOfSpeech.Interjection,
                        "Kompositum" => PartOfSpeech.Composite,
                        "unpersönlicher Ausdruck" => PartOfSpeech.ImpersonalExpression,
                        "" => PartOfSpeech.Unknown,
                        _ => throw new NotImplementedException(),
                    };

                    var gen = elem.ElementLocal("gen")?.Value switch
                    {
                        "m." => Gender.Masculine,
                        "f." => Gender.Feminine,
                        _ => Gender.Neutral,
                    };

                    var num = elem.ElementLocal("number")?.Value switch
                    {
                        "sg." => Number.Singular,
                        "pl." => Number.Plural,
                        _ => Number.None,
                    };

                    List<GrammarEntry> grammarEntries = new();
                    foreach (var grammarEntryElem in elem.ElementsLocal("gram"))
                    {
                        var grammarType = grammarEntryElem.AttributeLocal("type")?.Value switch
                        {
                            "collocPrep" => GrammarType.CollocPrep,
                            "collocAdv" => GrammarType.CollocAdv,
                            "collocPartikel" or "collocParticle" => GrammarType.CollocParticle,
                            "collocNoun" => GrammarType.CollocNoun,
                            "collocConj" => GrammarType.CollocConj,
                            _ => throw new NotImplementedException()
                        };

                        grammarEntries.Add(new(grammarType, grammarEntryElem.Value));
                    }

                    grammarGroup = new(pos, num, gen, grammarEntries, subc, note);
                    break;

                case "sense":
                    var citElem = elem.ElementLocal("cit");
                    var bibl = citElem.ElementLocal("bibl")?.Value;

                    TranslationCollection translations = new();
                    if (citElem.AttributeLocal("type")?.Value == "translation")
                    {
                        translations.AddRange(citElem.ElementsLocal("quote")
                            .Select(transElem => new Run(transElem.Value, null)
                            {
                                Language = LanguageInfo.Parse(transElem.AttributeLocal("lang")!.Value)
                            }));
                    }

                    senses.Add(new(translations, bibl));
                    break;
            }
        }

        return new(id!, type, forms, senses, grammarGroup!);
    }
    
    public static LanguageInfo MapDialectCode(string? dialectCode) => dialectCode switch
    {
        "S" => new LanguageInfo(KnownLanguage.CopticSahidic),
        "B" => new LanguageInfo(KnownLanguage.CopticBohairic),
        "A" => new LanguageInfo(KnownLanguage.CopticAkhmimic),
        "L" => new LanguageInfo(KnownLanguage.CopticLycopolitan),
        "F" => new LanguageInfo(KnownLanguage.CopticFayyumic),
        "M" => new LanguageInfo(KnownLanguage.CopticOxyrhynchite),
        "Ak" => new LanguageInfo(KnownLanguage.Akkadian), // FIXME: Probably wrong
        "J" => new LanguageInfo("cop-jjj"),
        "K" => new LanguageInfo("cop-kkk"),
        "P" => new LanguageInfo("cop-ppp"),
        "V" => new LanguageInfo("cop-vvv"),
        "W" => new LanguageInfo("cop-www"),
        "?" or null => new LanguageInfo(KnownLanguage.Default),
        _ => throw new ArgumentOutOfRangeException(nameof(dialectCode), dialectCode, null),
    }; 
}