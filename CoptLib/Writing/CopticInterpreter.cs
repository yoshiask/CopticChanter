﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CoptLib.Writing
{
    public static class CopticInterpreter
    {
        public static readonly char[] Separators = new[] { ' ', ',', ':', ';', '.' };
        public static readonly char[] Vowels = new[] { 'ⲁ', 'ⲉ', 'ⲓ', 'ⲏ', 'ⲟ', 'ⲱ' };
        public static readonly char[] CopticSpecificLetters = new[] { 'ϣ', 'ϥ', 'ϧ', 'ϫ', 'ϯ', 'ϭ' };
        public static readonly char[] GreekSpecificLetters = new[] { 'ⲅ', 'ⲇ', 'ⲍ', 'ⲝ', 'ⲯ' };
        public static readonly string[] GreekSpecificPrefixes = new[] { "ⲡⲣⲟ", "ⲡⲁⲣⲁ", "ⲁⲣⲭ", "ⲟⲙⲟ", "ⲕⲁⲧⲁ", "ⲥⲩⲛ" };
        public static readonly string[] GreekCommonSuffixes = new[] { "ⲁⲥ", "ⲟⲥ", "ⲏⲥ", "ⲁⲛ", "ⲟⲛ", "ⲏⲛ" };
        public static readonly IReadOnlyDictionary<char, string> SimpleIpaTranscriptions = new Dictionary<char, string>
        {
            ['ⲁ'] = "ä",
            ['ⲃ'] = "b",    // Always pronounced "v" in names
            ['ⲅ'] = "ɣ",
            ['ⲇ'] = "ð",    // Pronouned "d" in names
            ['ⲉ'] = "\u0065\u031E",
            ['ⲍ'] = "z",
            ['ⲏ'] = "iː",
            ['ⲑ'] = "θ",
            ['ⲓ'] = "i",
            ['ⲕ'] = "k",
            ['ⲗ'] = "l",
            ['ⲙ'] = "m",
            ['ⲛ'] = "n",
            ['ⲝ'] = "ks",   // Pronounced "eks" when at the start of a word
            ['ⲟ'] = "\u006F\u031E", // "ⲟⲩ" handled by VowelCombinations
            ['ⲡ'] = "p",
            ['ⲣ'] = "ɾ",
            ['ⲥ'] = "s",
            ['ⲧ'] = "t",
            ['ⲩ'] = "i",
            ['ⲫ'] = "f",
            ['ⲭ'] = "k",
            ['ⲯ'] = "ps",   // Pronounced "eps" when following a consonant
            ['ⲱ'] = "\u006F\u031E",
            ['ϣ'] = "ʃ",
            ['ϥ'] = "f",
            ['ϧ'] = "x",
            ['ϩ'] = "h",
            ['ϫ'] = "g",
            ['ϭ'] = "tʃ",   // Pronounced "etʃ" when following a consonant
            ['ϯ'] = "ti",
            ['\u0300'] = "ɛ"  // Jenkim splits syllable
        };

        private static Dictionary<string, Language> _loanWords;
        /// <summary>
        /// A dictionary of Coptic words loaned from other languages.
        /// </summary>
        public static Dictionary<string, Language> LoanWords => _loanWords ?? InitLoanWords();

        /// <summary>
        /// Analyzes Coptic text using the Greco-Bohairic pronounciation.
        /// </summary>
        /// <param name="srcText">The text to analyze.</param>
        /// <returns>An array of transcribed words using IPA.</returns>
        public static PhoneticEquivalent[][] PhoneticAnalysis(string srcText)
        {
            string[] srcWords = srcText.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
            var ipaWords = new PhoneticEquivalent[srcWords.Length][];

            for (int w = 0; w < srcWords.Length; w++)
            {
                string srcWord = srcWords[w];

                // Initial pass assumes default cases
                var ipaWord = new PhoneticEquivalent[srcWord.Length];
                for (int c = 0; c < srcWord.Length; c++)
                {
                    char ch = char.ToLower(srcWord[c]);
                    char? chPrev = (c - 1) >= 0 ? srcWord[c - 1] : null;
                    SimpleIpaTranscriptions.TryGetValue(ch, out var ipa);

                    // Handle jenkim
                    if (ch == '\u0300' && chPrev != null)
                    {
                        // Swap letters
                        var srcChars = srcWord.ToCharArray();
                        srcChars[c] = chPrev.Value;
                        srcChars[--c] = ch;

                        srcWord = new(srcChars);
                    }

                    ipaWord[c] = new(ch, ipa!);
                }

                for (int i = 0; i < ipaWord.Length; i++)
                {
                    var ph = ipaWord[i];
                    char ch = ph.Source;
                    string ipa = ph.Ipa;

                    char? chPrev = (i - 1) >= 0 ? ipaWord[i - 1].Source : null;
                    char? chNext = (i + 1) < ipaWord.Length ? ipaWord[i + 1].Source : null;
                    bool chPrevVow = chPrev.HasValue && Vowels.Contains(chPrev.Value);
                    bool chNextVow = chNext.HasValue && Vowels.Contains(chNext.Value);
                    bool chNextEI = chNext == 'ⲉ' || chNext == 'ⲓ';

                    // Handle special rules
                    if (ch == 'ⲩ')
                    {
                        // Upsilon acts differently depending on very specific
                        // conditions involving vowels
                        if (chPrev == 'ⲟ')
                        {
                            if (chNextVow && chNext != 'ⲱ')
                            {
                                ipa = "w";
                            }
                            else
                            {
                                ipa = "u";
                                ipaWord[i - 1] = new(chPrev.Value, string.Empty);
                            }
                        }
                        else if (chPrev == 'ⲁ' || chPrev == 'ⲉ')
                        {
                            ipa = "v";
                        }
                    }
                    else if (ch == 'ⲓ')
                    {
                        // Becomes a "yuh" sound before a vowel, dipthong after
                        if (chNextVow)
                            ipa = "j";
                        else if (chPrevVow)
                            ipa = "ɪ";
                    }
                    else if (ch == 'ⲭ')
                    {
                        // Pronunciation changes depending on the origin of the word
                        (Language? lang, double conf) = GuessWordLanguage(srcWord);

                        if (lang == Language.Coptic && conf >= 0.7)
                        {
                            // If Coptic: /k/
                            ipa = "k";
                        }
                        else
                        {
                            // If Greek: /ç/ before before /e/ or /i/, else /x/
                            ipa = (chNextEI || chNext == 'ⲏ' || chNext == 'ⲩ') ? "ç" : "x";
                        }
                    }
                    else if (chNextVow)
                    {
                        // Current letter preceeds a vowel
                        if (ch == '\u0300')
                            ipa = ".";
                        if (ch == 'ⲃ')
                            ipa = "v";
                        else if (chNextEI && ch == 'ⲅ')
                            ipa = "g";
                        else if (ch == 'ⲭ' && (chNextEI || chNext == 'ⲏ' || chNext == 'ⲩ'))
                            ipa = "ç";  // NOTE: Does not attempt to decide whether a word is of Greek origin
                        else if (chNextEI && ch == 'ϫ')
                            ipa = "dʒ";
                    }
                    else
                    {
                        // Current letter preceeds a consonant
                        if (ch == 'ⲅ' && i < ipaWord.Length - 1 &&
                                (ipaWord[i + 1].Source == ch || ipaWord[i + 1].Ipa == "g" || ipaWord[i + 1].Ipa == "k"))
                            ipa = "ŋ";
                        else if (ch == 'ⲝ' || (chNext != null && (ch == 'ⲯ' || ch == 'ϭ')))
                            ipa = "e\u031E" + ipa;
                        else if (ch == 'ⲭ' && chPrev != null && chPrev == '\u0300')
                            ipa = "x";   // Hack for Piekhristos vs. Christos
                        else if (ch == '\u0300')
                            ipa = "ɛ";
                    }

                    ipaWord[i].Ipa = ipa ?? ch.ToString();
                }

                ipaWords[w] = ipaWord;
            }

            return ipaWords;
        }

        /// <summary>
        /// Transcribes Coptic text using the Greco-Bohairic pronunciation into
        /// <see href="https://en.wikipedia.org/wiki/International_Phonetic_Alphabet">IPA</see>.
        /// </summary>
        /// <param name="srcText">The Coptic text to transcribe.</param>
        /// <remarks>
        /// Use <seealso cref="PhoneticAnalysis(string)"/> for more granular results.
        /// </remarks>
        public static string IpaTranscribe(string srcText)
        {
            var words = PhoneticAnalysis(srcText);

            return string.Join(" ", words.Select(
                word => string.Join(null, word.Select(
                    ph => ph.Ipa
                )))
            );
        }

        /// <summary>
        /// Uses clues to make an educated guess about which language
        /// a specific word is written in.
        /// </summary>
        /// <param name="word">The word to identify.</param>
        /// <returns>
        /// A <see cref="Language"/> and confidence percentage.
        /// <c>lang</c> is <see langword="null"/> if no language could be picked.
        /// </returns>
        public static (Language? lang, double confidence) GuessWordLanguage(string word)
        {
            var normWord = NormalizeString(word.ToLower());
            char[] chars = normWord.ToCharArray();

            // Some Coptic letters are straight from Demotic and aren't in Greek
            if (normWord.ContainsAny(CopticSpecificLetters))
                return (Language.Coptic, 1.0);

            // With only one exception, words that contain "ⲅ ⲇ ⲍ ⲝ ⲯ" are Greek
            if (normWord.Equals("ⲁⲛⲍⲏⲃ", StringComparison.OrdinalIgnoreCase))
                return (Language.Coptic, 0.9);
            else if (chars.ContainsAny(GreekSpecificLetters))
                return (Language.Greek, 0.9);

            // Check if word is known by Coptic Scriptorium
            if (LoanWords.TryGetValue(normWord, out Language lang))
                return (lang, 1.0);

            // TODO: Accept PhoneticEquivalent[] as input to allow this test case
            // If it has "ⲩ" that is pronounced as "EE"

            // Some prefixes are specific to Greek
            if (normWord.StartsWithAny(GreekSpecificPrefixes))
                return (Language.Greek, 0.8);

            // Some suffixes are much more common in Greek than Coptic
            if (normWord.EndsWithAny(GreekCommonSuffixes))
                return (Language.Greek, 0.7);

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
                    case UnicodeCategory.LowercaseLetter:
                    case UnicodeCategory.UppercaseLetter:
                    case UnicodeCategory.DecimalDigitNumber:
                        stringBuilder.Append(c);
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

        private static Dictionary<string, Language> InitLoanWords()
        {
            const string url = "https://raw.githubusercontent.com/CopticScriptorium/lexical-taggers/v1.4.1/language-tagger/lexicon.txt";
            System.Net.Http.HttpClient client = new();
            string tsv = client.GetStringAsync(url).Result;
            var lines = tsv.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            _loanWords = new(lines.Length);
            foreach (var line in lines)
            {
                var idx = line.IndexOf('\t');
                Language value = Language.Default;
                string key;

                if (idx >= 0)
                {
                    string valueStr = line.Substring(idx + 1);
                    if (!Enum.TryParse(valueStr, out value))
                        value = Language.Default;
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
}