using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
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
        public static readonly IReadOnlyDictionary<string, string> CopticAbbreviations = new Dictionary<string, string>
        {
            ["=o=c"] = "[oic",
            ["P=,=c"] = "Pi`,rictoc",
            ["=e=;=u"] = "e;ouab",
        };

        private static Dictionary<string, KnownLanguage> _loanWords;
        /// <summary>
        /// A dictionary of Coptic words loaned from other languages.
        /// </summary>
        public static Dictionary<string, KnownLanguage> LoanWords => _loanWords ?? GetLoanWords();

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
                        (KnownLanguage? lang, double conf) = GuessWordLanguage(srcWord);

                        if (lang == Language.Greek)
                        {
                            // If Greek: /ç/ before before /e/ or /i/, else /x/
                            ipa = (chNextEI || chNext == 'ⲏ' || chNext == 'ⲩ') ? "ç" : "x";
                        }
                        else
                        {
                            // If Coptic: /k/
                            ipa = "k";
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
                        else if (chNextEI && ch == 'ϫ')
                            ipa = "ʤ";
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
        /// Transliterates Coptic text to the specified language using
        /// the Greco-Bohairic pronunciation.
        /// </summary>
        /// <param name="srcText">The Coptic text to transcribe.</param>
        /// <param name="lang">The language to transliterate to.</param>
        /// <param name="srcTxtLength">The number of characters in the original string. Used for optimization.</param>
        public static string Transliterate(PhoneticEquivalent[][] srcText, KnownLanguage lang, int srcTextLength = 0)
        {
            if (!IpaTables.IpaToLanguage.TryGetValue(lang, out var table))
                throw new ArgumentException($"{lang} is not a supported transliteration target.");

            StringBuilder sb = new(srcTextLength);
            foreach (var word in srcText)
            {
                foreach (var pe in word)
                {
                    // Look up IPA letter in table, if it doesn't
                    // exist, leave it as is
                    if (table.TryGetValue(pe.Ipa, out var transliteration))
                        sb.Append(transliteration);
                    else
                        sb.Append(pe.Ipa);
                }

                sb.Append(" ");
            }

            // Remove extra space after last word
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        /// <summary>
        /// Transliterates Coptic text to the specified language using
        /// the Greco-Bohairic pronunciation.
        /// </summary>
        /// <param name="srcText">The Coptic text to transcribe.</param>
        /// <param name="lang">The language to transliterate to.</param>
        public static string Transliterate(string srcText, KnownLanguage lang)
            => Transliterate(PhoneticAnalysis(srcText), lang);

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

        /// <summary>
        /// Replaces all abbreviations with the full word in Coptic Standard.
        /// </summary>
        /// <param name="srcText">The source text in Coptic Standard.</param>
        public static string ExpandAbbreviations(string srcText)
        {
            foreach (string abbr in CopticAbbreviations.Keys)
                srcText = srcText.Replace(abbr, CopticAbbreviations[abbr]);
            return srcText;
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
                var assembly = typeof(CopticInterpreter).GetTypeInfo().Assembly;
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
}
