using CoptLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoptLib.Writing.Linguistics.Analyzers;

public class CopticGrecoBohairicAnalyzer : CopticAnalyzer
{
    private static readonly Dictionary<int, PhoneticWord> _grecoBohairicWordCache = new();

    public CopticGrecoBohairicAnalyzer() : this(new LanguageInfo(KnownLanguage.Coptic))
    {
    }

    protected CopticGrecoBohairicAnalyzer(LanguageInfo languageInfo) : base(languageInfo, GrecoBohairicSimpleIpaTranscriptions, BohairicKnownPrefixes, _grecoBohairicWordCache)
    {
    }

    protected override void PhoneticAnalysisInternal(PhoneticWord word, string srcWord)
    {
        KnownLanguage? origin = null;
        var ipaWord = word.Equivalents;

        for (int i = 0; i < ipaWord.Count; i++)
        {
            var ph = ipaWord[i];
            char ch = char.ToLower(ph.Source);
            string ipa = ph.Ipa;

            bool isFirstChar = (i - 1) < 0;
            bool isLastChar = (i + 1) >= ipaWord.Count;
            char chPrev = !isFirstChar ? ipaWord[i - 1].Source : '\0';
            char chNext = !isLastChar ? ipaWord[i + 1].Source : '\0';
            bool chPrevVow = !isFirstChar && Vowels.Contains(chPrev);
            bool chNextVow = !isLastChar && Vowels.Contains(chNext);
            bool chNextEI = chNext == 'ⲉ' || chNext == 'ⲓ';

            bool isVowel = Vowels.Contains(ch) || ch == 'ⲩ' || (!chNextVow && ch == '\u0300');

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

                        if (chNext == 'ⲁ')
                            ipaWord[i + 1] = new(chNext, "æ");

                        isVowel = false;
                    }
                    else
                    {
                        // Technically this is a digraph, but this code
                        // acts like the upsilon is silent and it's
                        // actually O making all the sound
                        ipa = string.Empty;
                        ipaWord[i - 1] = new(chPrev, "u");
                    }
                }
                else if (chPrev == 'ⲁ' || chPrev == 'ⲉ')
                {
                    ipa = "v";
                    isVowel = false;
                }
            }
            else if (ch == 'ⲓ')
            {
                if (chNextVow)
                {
                    // Becomes a "yuh" sound before a vowel
                    ipa = "j";

                    if (!isFirstChar && !Vowels.Contains(chPrev))
                    {
                        // Add /e̞/ back if following a consonant
                        ipa = "e̞" + ipa;
                    }
                    else
                    {
                        isVowel = false;
                    }
                }
                else if (chPrevVow)
                {
                    ipa = "ɪ";

                    if (chPrev == 'ⲟ' || chPrev == 'ⲱ')
                    {
                        // Digraph /ɔɪ/
                        ipaWord[i - 1] = new(chPrev, "ɔ");
                    }
                }
                else if (isLastChar)
                {
                    ipa = "iː";
                }
            }
            else if (ch == 'ⲑ' && (chPrev == 'ⲥ' || chPrev == 'ϣ'))
            {
                // Becomes /t/ when following ⲥ or ϣ
                ipa = "t\u032A";
            }
            else if (ch == 'ⲭ')
            {
                // Pronunciation changes depending on the origin of the word
                origin ??= GuessWordLanguage(srcWord).lang;

                if (origin == KnownLanguage.Greek)
                {
                    // If Greek: /ç/ before /e/ or /i/, else /x/
                    ipa = (chNextEI || chNext == 'ⲏ' || chNext == 'ⲩ') ? "ʃ" : "χ";
                }
                else
                {
                    // If Coptic: /k/
                    ipa = "k";
                }
            }
            else if (ch == 'ⲥ' && (chNext == 'ⲙ' || chPrev == 'ⲛ'))
            {
                // Pronunciation changes depending on the origin of the word
                origin ??= GuessWordLanguage(srcWord).lang;

                if (origin == KnownLanguage.Greek)
                    ipa = "z";
            }
            else if (ch == 'ⲅ')
            {
                var chNextIpa = i < ipaWord.Count - 1 ? ipaWord[i + 1].Ipa : null;

                // /g/ if followed by /i/ or /e/
                if (chNextIpa != null && chNextIpa.StartsWithAny(StringComparison.Ordinal, "i", "e", "ɛ"))
                    ipa = "g";

                // /ŋ/ if followed by /g/ or /k/
                else if (chNextIpa == "g" || chNextIpa == "k")
                    ipa = "ŋ";

                // Otherwise, default to /ɣ/ (see SimpleIpaTranscriptions)
                else
                    ipa = "ɣ";
            }
            else if (ch == 'ⲧ' && chPrev == 'ⲛ')
            {
                origin ??= GuessWordLanguage(srcWord).lang;

                if (origin == KnownLanguage.Greek)
                {
                    // If Greek: /d/ if preceeded by ⲛ in Greek words
                    ipa = "d";
                }
            }
            else if (chNextVow)
            {
                // Current letter preceeds a vowel
                if (ch == '\u0300')
                {
                    ipa = string.Empty;
                    word.SyllableBreaks.Add(i);
                }
                else if (ch == 'ⲃ')
                    ipa = "v";
                else if (chNextEI && ch == 'ϫ')
                    ipa = "d\u0361ʒ";
            }
            else
            {
                // Current letter preceeds a consonant
                if (ch == 'ⲃ' && chNext == 'ⲩ')
                    ipa = "v";
                else if (false)// && ch == 'ⲝ' || (!isLastChar && (ch == 'ⲯ' || ch == 'ϭ')))
                    ipa = "e\u031E" + ipa;
                else if (ch == '\u0300')
                    ipa = "ɛ";
            }

            // Use original character if no IPA equivalent is found
            ipa ??= ch.ToString();

            var equivalent = ipaWord[i];
            equivalent.Ipa = ipa;
            equivalent.IsVowel = isVowel;
            ipaWord[i] = equivalent;
        }
    }

    public static IReadOnlyList<string> BohairicKnownPrefixes = new string[]
    {
        /// Unknown
        "ⲛ\u0300ⲛⲓ", "ⲁⲕ",

        // Source: https://cld.bz/users/user-73469131/Coptic-Bohairic-Introductory-Course1

        /// Definite articles
        // Singular masculine
        "ⲡ̀", "ⲫ̀", "ⲡⲓ",
        // Singular feminine
        "ϯ", "ⲑ̀", "ⲧ̀",
        // Plural
        "ⲛⲓ", "ⲛⲉⲛ", "ⲛ̀",

        /// Indefinite articles
        // Singular (masculine and feminine)
        "ⲟⲩ",
        // Plural
        "ϩⲁⲛ",

        /// Possestive articles
        // Simple
        "ⲙ̀",
        // 2nd masculine singular
        "ⲡⲉⲕ", "ⲧⲉⲕ", "ⲛⲉⲕ",
        // 3rd masculine singular
        "ⲡⲉϥ", "ⲧⲉϥ", "ⲛⲉϥ",
        // 3rd feminine singular
        "ⲡⲉⲥ", "ⲧⲉⲥ", "ⲛⲉⲥ",
        // 1st plural
        "ⲡⲉⲛ", "ⲧⲉⲛ", "ⲛⲉⲛ",
        // 2nd plural
        "ⲡⲉⲧⲉⲛ", "ⲧⲉⲧⲉⲛ", "ⲛⲉⲧⲉⲛ",
        // 3rd plural
        "ⲡⲟⲩ", "ⲧⲟⲩ", "ⲛⲟⲩ",

        /// Relative clauses (who, whom, which)
        "ⲉⲑ", "ⲉⲧⲉ", "ⲉⲧ", "ⲉ̀",

        /// Relative nouns
        // Singular masculine
        "ⲡⲉⲧⲉ", "ⲡⲉⲧ",
        // Plural
        "ⲛⲉⲧⲉ", "ⲛⲉⲧ",

        /// Demonstrative adjectives
        "ⲡⲁⲓ", "ⲧⲁⲓ", "ⲛⲁⲓ",

        // Possessive 1st singular
        "ⲡⲁ", "ⲧⲁ", "ⲛⲁ",
        // 2nd feminine singular
        "ⲡⲉ", "ⲧⲉ", "ⲛⲉ",

        /// Perfect/pluperfect/aorist tense
        "ⲁ̀"
    };

    protected static IReadOnlyDictionary<char, string> GrecoBohairicSimpleIpaTranscriptions = new Dictionary<char, string>
    {
        ['ⲁ'] = "ɑ",
        ['ⲃ'] = "b\u032A",
        ['ⲅ'] = "g",
        ['ⲇ'] = "ð",
        ['ⲉ'] = "ɛ",
        ['ⲍ'] = "z",
        ['ⲏ'] = "iː",
        ['ⲑ'] = "θ",
        ['ⲓ'] = "i",
        ['ⲕ'] = "k",
        ['ⲗ'] = "l",
        ['ⲙ'] = "m",
        ['ⲛ'] = "n",
        ['ⲝ'] = "ks",
        ['ⲟ'] = "o", // "ⲟⲩ" handled by VowelCombinations
        ['ⲡ'] = "p\u032A",
        ['ⲣ'] = "r",
        ['ⲥ'] = "s",
        ['ⲧ'] = "t\u032A",
        ['ⲩ'] = "ɪ",
        ['ⲫ'] = "ɸ",
        ['ⲭ'] = "k",
        ['ⲯ'] = "ps",
        ['ⲱ'] = "o",
        ['ϣ'] = "ʃ",
        ['ϥ'] = "ɸ",
        ['ϧ'] = "χ",
        ['ϩ'] = "h",
        ['ϫ'] = "g",
        ['ϭ'] = "t\u0320ʃ",
        ['ϯ'] = "t\u032Ai",
        ['\u0300'] = "ɛ"  // Jenkim splits syllable
    };
}
