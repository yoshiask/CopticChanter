﻿using CommunityToolkit.Diagnostics;
using CoptLib.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CoptLib.Writing.Linguistics.Analyzers;

public abstract partial class CopticAnalyzer : LinguisticAnalyzer
{
    private static Dictionary<string, KnownLanguage> _loanWords;
    /// <summary>
    /// A dictionary of Coptic words loaned from other languages.
    /// </summary>
    public static Dictionary<string, KnownLanguage> LoanWords => _loanWords ?? GetLoanWords();

    private static readonly Dictionary<int, PhoneticEquivalent[]> _wordCache = new();

    public CopticAnalyzer(LanguageInfo languageInfo) : base(languageInfo)
    {
        Guard.IsEqualTo(languageInfo.Language, "cop");
    }

    public override string ExpandAbbreviations(string srcText)
    {
        foreach (string abbr in CopticAbbreviations.Keys)
            srcText = srcText.Replace(abbr, CopticAbbreviations[abbr]);
        return srcText;
    }

    public override PhoneticEquivalent[][] PhoneticAnalysis(string srcText)
        => PhoneticAnalysis(srcText, true, true);

    /// <inheritdoc cref="PhoneticAnalysis(string)"/>
    public PhoneticEquivalent[][] PhoneticAnalysis(string srcText, bool useCache, bool checkPrefixes)
    {
        string[] srcWords = srcText.SplitAndKeep(Separators).ToArray();
        var ipaWords = new PhoneticEquivalent[srcWords.Length][];

        for (int w = 0; w < srcWords.Length; w++)
        {
            string srcWordInit = srcWords[w];
            int srcWordInitHash = srcWordInit.ToLower().GetHashCode();
            PhoneticEquivalent[] ipaWord;

            // Check if entire word has known special pronunciation
            if (KnownPronunciationsWithPrefix.TryGetValue(srcWordInitHash, out ipaWord))
                goto finished;

            // Check if word is in cache
            if (useCache && _wordCache.TryGetValue(srcWordInitHash, out ipaWord))
                goto finished;

            string srcWord = srcWordInit;
            string? prefix = null;
            int srcWordStartIdx = 0;
            if (checkPrefixes)
            {
                // Separate prefixes
                srcWord = srcWordInit.StripAnyFromStart(CopticPrefixes, out prefix, StringComparison.OrdinalIgnoreCase);
                srcWordStartIdx = prefix?.Length ?? 0;
            }

            // Initialize phonetic equivalent list and handle prefix
            ipaWord = new PhoneticEquivalent[srcWordInit.Length];
            if (prefix != null)
            {
                // Run analysis on only the prefix
                var ipaPrefix = PhoneticAnalysis(prefix, useCache, false)[0];

                // Copy pronunciation to full word pronunciation
                ipaPrefix.CopyTo(ipaWord, 0);
            }

            // Check if base word has known special pronunciation
            int srcWordHash = srcWord.ToLower().GetHashCode();
            if (KnownPronunciations.TryGetValue(srcWordHash, out var ipaBaseWordKnown))
            {
                // Copy pronunciation to full word pronunciation
                ipaBaseWordKnown.CopyTo(ipaWord, srcWordStartIdx);

                goto finished;
            }

            // Initial pass assumes default cases
            for (int c = 0; c < srcWord.Length; c++)
            {
                char ch = srcWord[c];
                char? chPrev = (c - 1) >= 0 ? srcWord[c - 1] : null;
                SimpleIpaTranscriptions.TryGetValue(char.ToLower(ch), out var ipa);

                // Handle jenkim
                if (ch == '\u0300' && chPrev != null)
                {
                    // Swap letters
                    var srcChars = srcWord.ToCharArray();
                    srcChars[c] = chPrev.Value;
                    srcChars[--c] = ch;

                    srcWord = new(srcChars);
                }

                ipaWord[c + srcWordStartIdx] = new(ch, ipa!);
            }

            LetterPhoneticAnalysis(ipaWord.AsSpan(srcWordStartIdx), srcWord);

        finished:

            // Preserve casing
            ipaWords[w] = CopyCasing(srcWordInit, ipaWord);

            // Add word to cache
            if (useCache && !_wordCache.ContainsKey(srcWordInitHash))
                _wordCache.Add(srcWordInitHash, ipaWord);
        }

        return ipaWords;
    }

    protected abstract void LetterPhoneticAnalysis(Span<PhoneticEquivalent> ipaWord, string srcWord);

    /// <summary>
    /// Uses clues to make an educated guess about which language
    /// a specific word is written in.
    /// </summary>
    /// <param name="word">The word to identify.</param>
    /// <returns>
    /// A <see cref="KnownLanguage"/> and confidence percentage.
    /// <c>lang</c> is <see langword="null"/> if no language could be picked.
    /// </returns>
    public static (KnownLanguage? lang, double confidence) GuessWordLanguage(string word)
    {
        var normWord = NormalizeString(word.ToLower());
        char[] chars = normWord.ToCharArray();

        // Some Coptic letters are straight from Demotic and aren't in Greek
        if (normWord.ContainsAny(CopticSpecificLetters))
            return (KnownLanguage.Coptic, 1.0);

        // With only one exception, words that contain "ⲅ ⲇ ⲍ ⲝ ⲯ" are Greek
        if (normWord.Equals("ⲁⲛⲍⲏⲃ", StringComparison.OrdinalIgnoreCase))
            return (KnownLanguage.Coptic, 0.9);
        else if (chars.ContainsAny(GreekSpecificLetters))
            return (KnownLanguage.Greek, 0.9);

        // Check if word is known by Coptic Scriptorium
        if (LoanWords.TryGetValue(normWord, out KnownLanguage lang))
            return (lang, 1.0);

        // TODO: Accept PhoneticEquivalent[] as input to allow this test case
        // If it has "ⲩ" that is pronounced as "EE"

        // Some prefixes are specific to Greek
        if (normWord.StartsWithAny(GreekSpecificPrefixes))
            return (KnownLanguage.Greek, 0.8);

        // Some suffixes are much more common in Greek than Coptic
        if (normWord.EndsWithAny(GreekCommonSuffixes))
            return (KnownLanguage.Greek, 0.7);

        // Default to nothing
        // NOTE: Maybe we can make an educated guess depending on
        // the Unicode area used?
        return (null, 0.0);
    }

    /// <summary>
    /// Strips diacritic marks from the given string.
    /// </summary>
    /// <remarks>
    /// Adapted from <see href="https://stackoverflow.com/a/780800"/>.
    /// </remarks>
    public static string NormalizeString(string name)
    {
        string normalizedString = name.Normalize(NormalizationForm.FormD);
        StringBuilder stringBuilder = new(name.Length);

        bool prevUnderscore = false;
        foreach (char c in normalizedString)
        {
            switch (CharUnicodeInfo.GetUnicodeCategory(c))
            {
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.DecimalDigitNumber:
                    stringBuilder.Append(char.ToLower(c));
                    break;
                case UnicodeCategory.SpaceSeparator:
                case UnicodeCategory.ConnectorPunctuation:
                case UnicodeCategory.DashPunctuation:
                    if (prevUnderscore)
                    {
                        stringBuilder.Append('_');
                        prevUnderscore = true;
                    }
                    else
                    {
                        prevUnderscore = false;
                    }
                    break;
            }
        }

        // Remove duplicate underscores
        return stringBuilder.ToString();
    }

    /// <summary>
    /// Returns a new IPA word with the same casing as the source word.
    /// </summary>
    /// <param name="srcWord">The string to copy casing from.</param>
    /// <param name="ipaWord">The pronunciation to copy casing to.</param>
    private static PhoneticEquivalent[] CopyCasing(string srcWord, PhoneticEquivalent[] ipaWord)
    {
        // A clone of the array needs to made, otherwise changing the source
        // array will affect all references to it. This is particularly a problem
        // when a word appears more than once in a sentence in different cases.
        var ipaWordNew = (PhoneticEquivalent[])ipaWord.Clone();

        for (int c = 0; c < srcWord.Length; c++)
            ipaWordNew[c].IsUpper = char.IsUpper(srcWord[c]);
        return ipaWordNew;
    }

    /// <summary>
    /// Gets a lexicon of Sahidic Coptic loan words.
    /// </summary>
    /// <param name="ver">
    /// The specific version of Coptic Scriptorium's lexicon to use.
    /// Defaults to v1.4.1.
    /// </param>
    /// <returns>
    /// A dictionary of loan words, where the key is the word (in Unicode)
    /// and the value is the language it is borrowed from.
    /// </returns>
    private static Dictionary<string, KnownLanguage> GetLoanWords(Version? ver = null)
    {
        string tsv;
        if (ver == null || (ver.Major == 1 && ver.Minor == 4 && ver.Build == 1))
        {
            // v1.4.1 is the default and is included in CoptLib's embedded resources
            var assembly = typeof(CopticAnalyzer).GetTypeInfo().Assembly;
            using Stream resource = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Resources.lexicon.tsv");
            using StreamReader sr = new(resource);
            tsv = sr.ReadToEnd();
        }
        else
        {
            // Different version was specified, fetch from GitHub
            string url = $"https://raw.githubusercontent.com/CopticScriptorium/lexical-taggers/v{ver}/language-tagger/lexicon.txt";
            System.Net.Http.HttpClient client = new();
            tsv = client.GetStringAsync(url).Result;
        }

        var lines = tsv.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        _loanWords = new(lines.Length);
        foreach (var line in lines)
        {
            var idx = line.IndexOf('\t');
            KnownLanguage value = KnownLanguage.Default;
            string key;

            if (idx >= 0)
            {
                string valueStr = line.Substring(idx + 1);
                if (!Enum.TryParse(valueStr, out value))
                    value = KnownLanguage.Default;
                key = line.Substring(0, idx);
            }
            else
            {
                key = line;
            }

            if (!_loanWords.ContainsKey(key))
                _loanWords.Add(key, value);
        }

        return _loanWords;
    }
}