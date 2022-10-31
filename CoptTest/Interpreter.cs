using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace CoptTest
{
    public class Interpreter
    {
        static readonly string[] inputs = new[]
        {
            "ⲛⲓⲁ̀ⲅⲅⲉⲗⲟⲥ", "ⲓⲣⲏⲛⲏ", "ⲟⲩⲟϩ",

            "Ⲡⲓⲭ̀ⲣⲓⲥⲧⲟⲥ ⲁϥⲧⲱⲛϥ ⲉ̀ⲃⲟⲗ ϧⲉⲛ ⲛⲏⲉⲑⲙⲱⲟⲩⲧ: ⲫⲏⲉ̀ⲧⲁϥⲙⲟⲩ ⲁϥϩⲱⲙⲓ ⲉ̀ϫⲉⲛ ⲫ̀ⲙⲟⲩ ⲟⲩⲟϩ ⲛⲏⲉⲧⲭⲏ ϧⲉⲛ ⲛⲓⲙ̀ϩⲁⲩ ⲁϥⲉⲣϩ̀ⲙⲟⲧ ⲛⲱⲟⲩ ⲙ̀ⲡⲓⲱⲛϧ ⲛ̀ⲉ̀ⲛⲉϩ.",
            "Ⲭⲣⲓⲥⲧⲟⲥ ⲁ̀ⲛⲉⲥⲧⲏ ⲉⲕ ⲛⲉⲕⲣⲱⲛ: ⲑⲁⲛⲁⲧⲱ ⲑⲁⲛⲁⲧⲟⲛ: ⲡⲁⲧⲏⲥⲁⲥ ⲕⲉ ⲧⲓⲥ ⲉⲛ ⲧⲓⲥ ⲙ̀ⲛⲏⲙⲁⲥⲓ ⲍⲱⲏⲛ ⲭⲁⲣⲓⲥⲁⲙⲉⲛⲟⲥ.",

            "Ⲧⲉⲛⲟ̀ⲩⲱ̀ϣⲧ ⲙ̀Ⲫ̀ⲓⲱⲧ ⲛⲉⲙ ⲡ̀Ϣⲏⲣⲓ: ⲛⲉⲙ ⲡⲓⲠ̀ⲛⲉⲩⲙⲁ ⲉⲑⲟⲩⲁⲃ: ⲭⲉⲣⲉ ϯⲉ̀ⲕⲕⲗⲏⲥⲓⲁ: ⲡ̀ⲏⲓ ⲛ̀ⲧⲉ ⲛⲓⲁ̀ⲅⲅⲉⲗⲟⲥ",

            "Ϧⲉⲛ ⲟⲩϣⲱⲧ ⲁϥϣⲱⲧ: ⲛ̀ϫⲉ ⲡⲓⲙⲱⲟⲩ ⲛ̀ⲧⲉ ⲫ̀ⲓⲟⲙ: ⲟⲩⲟϩ ⲫ̀ⲛⲟⲩⲛ ⲉⲧϣⲏⲕ: ⲁϥϣⲱⲡⲓ ⲛ̀ⲟⲩⲙⲁ ⲙ̀ⲙⲟϣⲓ.",
            "Ⲟⲩⲕⲁϩⲓ ⲛ̀ⲁⲑⲟⲩⲱⲛϩ: ⲁ̀ⲫ̀ⲣⲏ ϣⲁⲓ ϩⲓϫⲱϥ: ⲟⲩⲙⲱⲓⲧ ⲛ̀ⲁⲧⲥⲓⲛⲓ: ⲁⲩⲙⲟϣⲓ ϩⲓⲱⲧϥ.",
            "Ⲟⲩⲙⲱⲟⲩ ⲉϥⲃⲏⲗ ⲉ̀ⲃⲟⲗ: ⲁϥⲟ̀ϩⲓ ⲉ̀ⲣⲁⲧϥ: ϧⲉⲛ ⲟⲩϩⲱⲃ ⲛ̀ϣ̀ⲫⲏⲣⲓ: ⲙ̀ⲡⲁⲣⲁⲇⲟⲝⲟⲛ.",
            "Ⲫⲁⲣⲁⲱ̀ ⲛⲉⲙ ⲛⲉϥϩⲁⲣⲙⲁ: ⲁⲩⲱⲙⲥ ⲉ̀ⲡⲉⲥⲏⲧ: ⲛⲉⲛϣⲏⲣⲓ ⲙ̀Ⲡⲓⲥⲣⲁⲏⲗ: ⲁⲩⲉⲣϫⲓⲛⲓⲟⲣ ⲙ̀ⲫ̀ⲓⲟⲙ.",
            "Ⲉ̀ⲛⲁϥϩⲱⲥ ϧⲁϫⲱⲟⲩ ⲡⲉ: ⲛ̀ϫⲉ Ⲙⲱⲩ̀ⲥⲏⲥ ⲡⲓⲡ̀ⲣⲟⲫⲏⲧⲏⲥ: ϣⲁ ⲛ̀ⲧⲉϥϭⲓⲧⲟⲩ ⲉ̀ϧⲟⲩⲛ: ϩⲓ ⲡ̀ϣⲁϥⲉ ⲛ̀Ⲥⲓⲛⲁ.",
            "Ⲉ̀ⲛⲁϥϩⲱⲥ ⲉ̀Ⲫ̀ⲛⲟⲩϯ: ϧⲉⲛ ⲧⲁⲓϩⲱⲇⲏ ⲙ̀ⲃⲉⲣⲓ: ϫⲉ ⲙⲁⲣⲉⲛϩⲱⲥ ⲉ̀Ⲡ̀ϭⲟⲓⲥ: ϫⲉ ϧⲉⲛ ⲟⲩⲱ̀ⲟⲩ ⲅⲁⲣ ⲁϥϭⲓⲱ̀ⲟⲩ.",
            "Ϩⲓⲧⲉⲛ ⲛⲓⲉⲩⲭⲏ: ⲛ̀ⲧⲉ Ⲙⲱⲩ̀ⲥⲏⲥ ⲡⲓⲁⲣⲭⲏⲡ̀ⲣⲟⲫⲏⲧⲏⲥ: Ⲡ̀ϭⲟⲓⲥ ⲁ̀ⲣⲓϩ̀ⲙⲟⲧ ⲛⲁⲛ: ⲙ̀ⲡⲓⲭⲱ ⲉ̀ⲃⲟⲗ ⲛ̀ⲧⲉ ⲛⲉⲛⲛⲟⲃⲓ.",
            "Ϩⲓⲧⲉⲛ ⲛⲓⲡ̀ⲣⲉⲥⲃⲓⲁ: ⲛ̀ⲧⲉ Ϯⲑⲉⲟ̀ⲧⲟⲕⲟⲥ ⲉⲑⲟⲩⲁⲃ Ⲙⲁⲣⲓⲁ: Ⲡ̀ϭⲟⲓⲥ ⲁ̀ⲣⲓϩ̀ⲙⲟⲧ ⲛⲁⲛ: ⲙ̀ⲡⲓⲭⲱ ⲉ̀ⲃⲟⲗ ⲛ̀ⲧⲉ ⲛⲉⲛⲛⲟⲃⲓ.",
            "Ⲧⲉⲛⲟⲩⲱϣⲧ ⲙ̀ⲙⲟⲕ ⲱ̀ Ⲡⲓⲭ̀ⲣⲓⲥⲧⲟⲥ: ⲛⲉⲙ Ⲡⲉⲕⲓⲱⲧ ⲛ̀ⲁ̀ⲅⲁⲑⲟⲥ: ⲛⲉⲙ Ⲡⲓⲡ̀ⲛⲉⲩⲙⲁ ⲉⲑⲟⲩⲁⲃ: ϫⲉ ⲁⲕⲓ̀ ⲁⲕⲥⲱϯ ⲙ̀ⲙⲟⲛ.",
        };

        public static readonly char[] Separators = new char[] { ' ', ',', ':', ';', '.' };
        public static readonly char[] CopticVowels = new char[] { 'ⲁ', 'ⲉ', 'ⲓ', 'ⲏ', 'ⲟ', 'ⲱ' };
        public static readonly string[] VowelCombinations = new string[] { "ⲁⲓ", "ⲟⲩ", "ⲏⲓ", "ⲉⲓ", "ⲓⲱ" };
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

        public sealed class PhoneticEquivalent
        {
            public PhoneticEquivalent(char source, string ipa)
            {
                Source = source;
                Ipa = ipa;
            }

            public char Source { get; set; }
            public string Ipa { get; set; }

            public override string ToString() => $"('{Source}', \"{Ipa}\")";
        }

        /// <summary>
        /// Transcribes Coptic text using the Greco-Bohairic pronounciation into IPA.
        /// </summary>
        /// <param name="srcText">The text to transcribe.</param>
        /// <returns>An array of transcribed words.</returns>
        public PhoneticEquivalent[][] IpaTranscribe(string srcText)
        {
            string[] srcWords = srcText.Replace("\u200D", string.Empty)
                .Split(Separators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
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
                    bool chPrevVow = chPrev.HasValue && CopticVowels.Contains(chPrev.Value);
                    bool chNextVow = chNext.HasValue && CopticVowels.Contains(chNext.Value);
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

        [Fact]
        public void IpaTranscribe_CopticUnicode()
        {
            StringBuilder sb = new();

            foreach (var input in inputs)
            {
                var words = IpaTranscribe(input);

                string result = string.Join(' ', words.Select(
                    word => string.Join(null, word.Select(
                        ph => ph.Ipa
                    )))
                );

                Debug.WriteLine(result);
                sb.AppendLine($"{input},{result}");
            }

            string outPath = Path.Combine(AppContext.BaseDirectory, "TestResults", nameof(IpaTranscribe_CopticUnicode) + ".csv");
            Directory.CreateDirectory(Path.GetDirectoryName(outPath));
            File.WriteAllText(outPath, sb.ToString(), Encoding.Unicode);
        }

        [Fact]
        public void SyllableAnalysis_CopticUnicode()
        {
            string[] words = inputs[4].Split(Separators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            List<string>[] results = new List<string>[words.Length];

            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i];
                List<string> sylls = new();

                if (word.Length <= 2)
                {
                    // Words shorter than 3 are likely only one syllable
                    sylls.Add(word);
                }
                else
                {
                    // Generate list parallel to `words` that stores if the character is a vowel
                    bool[] isVowel = new bool[word.Length];
                    for (int c = 0; c < word.Length; c++)
                        isVowel[c] = CopticVowels.Contains(char.ToLower(word[c]));

                    StringBuilder sb = new();
                    bool prevVowel = false;

                    for (int c = 0; c < word.Length; c++)
                    {
                        bool first = c == 0;
                        bool last = c >= word.Length - 1;
                        bool secondToLast = c == word.Length - 2;
                        bool vowel = isVowel[c];
                        bool nextVowel = !last && isVowel[c + 1];
                        bool nextNextVowel = !(last || secondToLast) && isVowel[c + 2];

                        char ch = word[c];
                        char nextCh = !last ? word[c + 1] : ' ';
                        char nextNextCh = !(last || secondToLast) ? word[c + 2] : ' ';

                        bool endSyllable = false;

                        if (char.ToLower(nextCh) == 'ⲩ')
                        {
                            // Upsilon is sometimes a vowel, sometimes a consonant
                            if (vowel)
                            {
                                nextVowel = char.ToLower(ch) == 'ⲟ'
                                    && (!CopticVowels.Contains(nextNextCh) || nextNextCh == 'ⲱ');
                            }
                            else
                            {
                                nextVowel = true;
                            }

                            // Update isVowel list
                            isVowel[c + 1] = nextVowel;
                        }

                        if (!last && nextCh == 0x0300)
                        {
                            // Found a jenkim, split
                            sb.Append(ch);
                            sb.Append('\u0300');
                            sylls.Add(sb.ToString());
                            sb.Clear();

                            // Make sure to skip jenkim
                            c++;
                        }
                        else if (char.ToLower(ch) == 'ϯ')
                        {
                            // 'Ϯ' or 'ϯ' represents the "tee" sound, which makes
                            // basically means it's always its own syllable

                            if (sb.Length > 0)
                            {
                                sylls.Add(sb.ToString());
                                sb.Clear();
                            }

                            prevVowel = true;
                            endSyllable = true;
                        }
                        else if (!nextVowel && nextNextVowel && !(first || sb.Length == 0))
                        {
                            // ⲁϥ-ϣⲁ, VCCV pattern
                            // ⲙⲁ-ⲣⲱ, CVCV pattern
                            if (prevVowel ^ vowel)
                                endSyllable = true;
                            else
                                sb.Append(ch);
                        }
                        // invalid VV pattern
                        else if (vowel && nextVowel && !VowelCombinations.Contains($"{ch}{nextCh}"))
                        {
                            endSyllable = true;
                        }
                        else
                        {
                            sb.Append(ch);
                        }

                        if (endSyllable)
                        {
                            sb.Append(ch);
                            sylls.Add(sb.ToString());
                            sb.Clear();
                        }

                        prevVowel = vowel;
                    }

                    // Ensure the last syllable is added
                    string remainingSyll = sb.ToString();
                    if (!string.IsNullOrEmpty(remainingSyll))
                        sylls.Add(remainingSyll);
                }

                // Correct final syllable if necessary
                if (false)//sylls.Count >= 2)
                {
                    string lastSyll = sylls[^1];
                    string penultSyll = sylls[^2];

                    if (penultSyll.Length >= 2)
                    {
                        char penultSyllLastCh = penultSyll[^1];
                        if (CopticVowels.Contains(lastSyll[0]) && !CopticVowels.Contains(penultSyllLastCh))
                        {
                            // Move last consonant of second-to-last syllable to
                            // beginning of last syllable
                            sylls[^2] = penultSyll[0..^1];
                            sylls[^1] = penultSyllLastCh + lastSyll;
                        }
                    }
                }

                // Add to results
                results[i] = sylls;
            }

            // Display input with syllable breaks
            var newWords = results.Select(s => string.Join('·', s));
            string output = string.Join(" ", newWords);

            Debug.WriteLine(inputs[4]);
            Debug.WriteLine(output);
        }

        private static bool IsVowelGroup(string text)
        {
            Regex rx = new(@"(ⲁ|ⲉ|ⲓ|ⲏ|ⲟⲩ|ⲟ|ⲱ)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            return rx.Replace(text, string.Empty) == string.Empty;
        }
    }
}
