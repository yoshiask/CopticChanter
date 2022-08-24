using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace CoptTest
{
    public class Interpreter
    {
        public static readonly char[] Separators = new char[] { ' ', ',', ':', ';', '.' };
        public static readonly char[] CopticVowels = new char[] { 'ⲁ', 'ⲉ', 'ⲓ', 'ⲏ', 'ⲟ', 'ⲱ' };
        public static readonly string[] VowelCombinations = new string[] { "ⲁⲓ", "ⲟⲩ", "ⲏⲓ", "ⲉⲓ", "ⲓⲱ" };

        [Fact]
        public void SyllableAnalysis_CopticUnicode()
        {
            const string input = "Ⲧⲉⲛⲟ̀ⲩⲱ̀ϣⲧ ⲙ̀Ⲫ̀ⲓⲱⲧ ⲛⲉⲙ ⲡ̀Ϣⲏⲣⲓ: ⲛⲉⲙ ⲡⲓⲠ̀ⲉⲛⲩⲙⲁ ⲉ̅ⲑ̅ⲩ̅: ⲭⲉⲣⲉ ϯⲉ̀ⲕⲕⲗⲏⲥⲓⲁ: ⲡ̀ⲏⲓ ⲛ̀ⲧⲉ ⲛⲓⲁ̀ⲅⲅⲉⲗⲟⲥ";

            string[] words = input.Split(Separators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
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

            Debug.WriteLine(input);
            Debug.WriteLine(output);
        }

        private static bool IsVowelGroup(string text)
        {
            Regex rx = new(@"(ⲁ|ⲉ|ⲓ|ⲏ|ⲟⲩ|ⲟ|ⲱ)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            return rx.Replace(text, string.Empty) == string.Empty;
        }
    }
}
