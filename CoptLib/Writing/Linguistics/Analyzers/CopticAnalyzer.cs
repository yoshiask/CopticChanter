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
    private static Dictionary<string, KnownLanguage>? _loanWords;
    
    /// <summary>
    /// A dictionary of Coptic words loaned from other languages.
    /// </summary>
    public static Dictionary<string, KnownLanguage> LoanWords => _loanWords ?? GetLoanWords();

    protected readonly IReadOnlyDictionary<char, string> IpaTranscriptions;
    protected readonly Dictionary<int, PhoneticWord> WordCache;
    protected readonly IReadOnlyList<string> KnownPrefixes;

    public CopticAnalyzer(LanguageInfo languageInfo, IReadOnlyDictionary<char, string> ipaTranscriptions, IReadOnlyList<string> knownPrefixes, Dictionary<int, PhoneticWord> wordCache)
        : base(languageInfo)
    {
        Guard.IsNotNull(ipaTranscriptions);

        IpaTranscriptions = ipaTranscriptions;
        WordCache = wordCache;
        KnownPrefixes = knownPrefixes;
    }

    protected override string ResolveAbbreviationInternal(string key, bool keepAbbreviated)
    {
        var (ex, sh) = CopticAbbreviations[key];
        return keepAbbreviated ? sh : ex;
    }

    public override PhoneticWord[] PhoneticAnalysis(string srcText)
        => PhoneticAnalysis(srcText, true, true);

    /// <inheritdoc cref="PhoneticAnalysis(string)"/>
    public PhoneticWord[] PhoneticAnalysis(string srcText, bool useCache, bool checkPrefixes)
    {
        string[] srcWords = srcText.SplitAndKeep(Separators).ToArray();
        var ipaWords = new PhoneticWord[srcWords.Length];

        for (int w = 0; w < srcWords.Length; w++)
        {
            string srcWordInit = srcWords[w];
            int srcWordInitHash = srcWordInit.ToLower().GetHashCode();
            PhoneticWord word;

            // Check if entire word has known special pronunciation
            if (KnownPronunciationsWithPrefix.TryGetValue(srcWordInitHash, out word))
                goto finished;

            // Check if word is in cache
            if (useCache && WordCache.TryGetValue(srcWordInitHash, out word))
                goto finished;

            string srcWord = srcWordInit;
            string? prefixStr = null;
            int srcWordStartIdx = 0;
            if (checkPrefixes)
            {
                // Separate prefixes
                srcWord = srcWordInit.StripAnyFromStart(KnownPrefixes, out prefixStr, StringComparison.OrdinalIgnoreCase);
                srcWordStartIdx = prefixStr?.Length ?? 0;
            }

            // Initialize phonetic equivalent list and handle prefix
            word = new(new PhoneticEquivalent[srcWordInit.Length], new List<int>());
            if (prefixStr != null)
            {
                // Run analysis on only the prefix
                var prefix = PhoneticAnalysis(prefixStr, useCache, false)[0];

                // Copy pronunciation to full word pronunciation
                prefix.CopyTo(word, 0);

                // Add a syllable break between the prefix and base morphs
                word.SyllableBreaks.Add(prefix.Length);
            }

            // Check if base word has known special pronunciation
            int srcWordHash = srcWord.ToLower().GetHashCode();
            if (KnownPronunciations.TryGetValue(srcWordHash, out var ipaBaseWordKnown))
            {
                // Copy pronunciation to full word pronunciation
                ipaBaseWordKnown.CopyTo(word, srcWordStartIdx);

                goto finished;
            }

            // Initial pass assumes default cases
            for (int c = 0; c < srcWord.Length; c++)
            {
                char ch = srcWord[c];
                char? chPrev = (c - 1) >= 0 ? srcWord[c - 1] : null;
                if (!IpaTranscriptions.TryGetValue(char.ToLower(ch), out var ipa))
                    ipa = ch.ToString();

                // Handle jenkim
                if (ch == '\u0300' && chPrev != null)
                {
                    // Swap letters
                    var srcChars = srcWord.ToCharArray();
                    srcChars[c] = chPrev.Value;
                    srcChars[--c] = ch;

                    srcWord = new(srcChars);
                }

                word.Equivalents[c + srcWordStartIdx] = new(ch, ipa);
            }

            var morph = word.Substring(srcWordStartIdx);
            PhoneticAnalysisInternal(morph, srcWord);
            MarkSyllables(morph);
            morph.CopyTo(word, srcWordStartIdx);

        finished:

            // Preserve casing
            ipaWords[w] = CopyCasing(srcWordInit, word);

            // Add word to cache
            if (useCache && !WordCache.ContainsKey(srcWordInitHash))
                WordCache.Add(srcWordInitHash, word);
        }

        return ipaWords;
    }

    protected abstract void PhoneticAnalysisInternal(PhoneticWord ipaWord, string srcWord);

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
    /// Marks where syllables are in the given <paramref name="word"/>.
    /// </summary>
    private static void MarkSyllables(PhoneticWord word)
    {
        // Start the list with 0, then later end it with the length of the word.
        // This allows the second pass to 
        List<int> breaks = new()
        {
            0
        };

        // First pass: Apply the maximal onset principle
        for (int c = word.Equivalents.Count - 1; c >= 0; --c)
        {
            var currPe = word.Equivalents[c];
            bool isLast = c >= word.Equivalents.Count - 1;
            bool isFirst = c == 0;
            PhoneticEquivalent nextPe = isLast ? default : word.Equivalents[c + 1];
            PhoneticEquivalent prevPe = isFirst ? default : word.Equivalents[c - 1];

            // Jenkim or "ϯ" (/ti/)
            if (currPe.Source is '\u0300' or 'ϯ')
                breaks.Add(c);
            // Long
            else if (currPe.Ipa.Length > 0 && currPe.Ipa[^1] == 'ː')
                breaks.Add(--c);
            // ⲟⲩ digraph
            else if (!isLast && currPe.Source == 'ⲟ' && nextPe.Source == 'ⲩ')
            {
                breaks.Add(c);

                if (c - 1 > 0)
                    breaks.Add(--c);
            }
            // Prevent splitting of other digraphs
            else if (!isLast && prevPe.Source == 'ⲓ' && currPe.Source is 'ⲁ' or 'ⲉ' or 'ⲟ')
                continue;
            // CVC
            else if (!isLast && !isFirst && !prevPe.IsVowel && currPe.IsVowel && !nextPe.IsVowel)
                breaks.Add(--c);
            // VC or CV
            else if (!isLast && currPe.IsVowel ^ nextPe.IsVowel)
                breaks.Add(c);
        }

        // Essentially clip the bounds of each syllable. These invalid
        // syllables should be removed in the following pass.
        breaks.RemoveAll(b => b < 0 || b > word.Length);
        breaks.Sort();
        breaks.Add(word.Length);

        // Second pass: Combine consonant-only syllables and break up complex syllables
        for (int i = 0; i < breaks.Count - 1; ++i)
        {
            int start = breaks[i];
            int end = breaks[i + 1];
            int length = end - start;

            var substring = word.Substring(start, length);

            if (length == 0)
            {
                breaks.Remove(start);
                --i;
            }
            else if (substring.Equivalents.All(s => !s.IsVowel))
            {
                // Maximal onset principle, add the consonants to the next syllable
                // so it has as longer onset, but only if allowed
                bool IsTi() => start < word.Length - 2 && word.Equivalents[start + 1].Source == 'ϯ';
                bool IsOu()
                {
                    if (start < word.Length - 3 && word.Equivalents[start + 1].Source == 'ⲟ' && word.Equivalents[start + 2].Source == 'ⲩ')
                    {
                        // Allow the digraph to be merged into the previous syllable
                        // if a vowel follows the digraph.
                        return start < word.Length - 4 && word.Equivalents[start + 3].IsVowel;
                    }

                    return false;
                }
                bool IsJenkim() => start < word.Length - 2 && word.Equivalents[start + 1].Source == '\u0300';

                if (IsOu() || IsTi() || IsJenkim())
                {
                    breaks.Remove(start);
                    --i;
                }
                else
                {
                    breaks.Remove(end);
                }
            }
        }

        word.SyllableBreaks = new(breaks);
        word.SyllableBreaks.Remove(0);
        word.SyllableBreaks.Remove(word.Length);
    }

    /// <summary>
    /// Returns a new IPA word with the same casing as the source word.
    /// </summary>
    /// <param name="srcWord">The string to copy casing from.</param>
    /// <param name="ipaWord">The pronunciation to copy casing to.</param>
    private static PhoneticWord CopyCasing(string srcWord, PhoneticWord ipaWord)
    {
        // A clone of the array needs to made, otherwise changing the source
        // array will affect all references to it. This is particularly a problem
        // when a word appears more than once in a sentence in different cases.
        var newEquivalents = ipaWord.Equivalents.ToArray();

        for (int c = 0; c < srcWord.Length; c++)
            newEquivalents[c].IsUpper = char.IsUpper(srcWord[c]);
        return new(newEquivalents, ipaWord.SyllableBreaks);
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
        if (ver == null || ver is {Major: 1, Minor: 4, Build: 1})
        {
            // v1.4.1 is the default and is included in CoptLib's embedded resources
            var assembly = typeof(CopticAnalyzer).GetTypeInfo().Assembly;
            using var resource = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Resources.lexicon.tsv")!;
            using StreamReader sr = new(resource);
            tsv = sr.ReadToEnd();
        }
        else
        {
            // Different version was specified, fetch from GitHub
            var url = $"https://raw.githubusercontent.com/CopticScriptorium/lexical-taggers/v{ver}/language-tagger/lexicon.txt";
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
                string valueStr = line[(idx + 1)..];
                if (!Enum.TryParse(valueStr, out value))
                    value = KnownLanguage.Default;
                key = line[..idx];
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
