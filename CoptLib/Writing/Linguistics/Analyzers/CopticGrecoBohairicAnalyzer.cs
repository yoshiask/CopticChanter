using System;
using System.Collections.Generic;
using System.Linq;

namespace CoptLib.Writing.Linguistics.Analyzers;

public class CopticGrecoBohairicAnalyzer : CopticAnalyzer
{
    private static readonly Dictionary<int, PhoneticEquivalent[]> _grecoBohairicWordCache = new();

    public CopticGrecoBohairicAnalyzer() : this(new LanguageInfo(KnownLanguage.Coptic))
    {
    }

    protected CopticGrecoBohairicAnalyzer(LanguageInfo languageInfo) : base(languageInfo, GrecoBohairicSimpleIpaTranscriptions, _grecoBohairicWordCache)
    {
    }

    protected override void LetterPhoneticAnalysis(Span<PhoneticEquivalent> ipaWord, string srcWord)
    {
        KnownLanguage? origin = null;
        for (int i = 0; i < ipaWord.Length; i++)
        {
            var ph = ipaWord[i];
            char ch = char.ToLower(ph.Source);
            string ipa = ph.Ipa;

            bool isFirstChar = (i - 1) < 0;
            bool isLastChar = (i + 1) >= ipaWord.Length;
            char chPrev = !isFirstChar ? ipaWord[i - 1].Source : '\0';
            char chNext = !isLastChar ? ipaWord[i + 1].Source : '\0';
            bool chPrevVow = !isFirstChar && Vowels.Contains(chPrev);
            bool chNextVow = !isLastChar && Vowels.Contains(chNext);
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
                }
            }
            else if (ch == 'ⲓ')
            {
                // Becomes a "yuh" sound before a vowel, dipthong after
                if (chNextVow)
                {
                    ipa = "j";

                    if (!isFirstChar && !Vowels.Contains(chPrev))
                    {
                        // Add /i/ back if following a consonant
                        ipa = "i" + ipa;
                    }
                }
                else if (chPrevVow)
                    ipa = "ɪ";
            }
            else if (ch == 'ⲑ' && (chPrev == 'ⲥ' || chPrev == 'ϣ'))
            {
                // Becomes /t/ when following ⲥ or ϣ
                ipa = "t";
            }
            else if (ch == 'ⲭ')
            {
                // Pronunciation changes depending on the origin of the word
                origin ??= GuessWordLanguage(srcWord).lang;

                if (origin == KnownLanguage.Greek)
                {
                    // If Greek: /ç/ before /e/ or /i/, else /x/
                    ipa = (chNextEI || chNext == 'ⲏ' || chNext == 'ⲩ') ? "ç" : "x";
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
                var chNextIpa = i < ipaWord.Length - 1 ? ipaWord[i + 1].Ipa : null;

                // /g/ if followed by /i/ or /e/
                if (chNextIpa != null &&
                    (chNextIpa.StartsWith("i", StringComparison.Ordinal) || chNextIpa.StartsWith("e", StringComparison.Ordinal)))
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
                    ipa = ".";
                else if (ch == 'ⲃ')
                    ipa = "v";
                else if (chNextEI && ch == 'ϫ')
                    ipa = "ʤ";
            }
            else
            {
                // Current letter preceeds a consonant
                if (ch == 'ⲃ' && chNext == 'ⲩ')
                    ipa = "v";
                else if (ch == 'ⲝ')// || (chNext != null && (ch == 'ⲯ' || ch == 'ϭ')))
                    ipa = "e\u031E" + ipa;
                else if (ch == '\u0300')
                    ipa = "ɛ";
            }

            // Use original character if no IPA equivalent is found
            ipa ??= ch.ToString();
            ipaWord[i].Ipa = ipa;
        }
    }

    protected static IReadOnlyDictionary<char, string> GrecoBohairicSimpleIpaTranscriptions = new Dictionary<char, string>
    {
        ['ⲁ'] = "ä",
        ['ⲃ'] = "b",    // Always pronounced "v" in names
        ['ⲅ'] = "g",
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
}
