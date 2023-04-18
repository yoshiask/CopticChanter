using System;
using System.Collections.Generic;
using System.Linq;

namespace CoptLib.Writing.Linguistics.Analyzers;

public class CopticOldBohairicAnalyzer : CopticAnalyzer
{
    private static readonly Dictionary<int, PhoneticWord> _oldBohairicWordCache = new();
 
    public CopticOldBohairicAnalyzer() : this(new LanguageInfo(KnownLanguage.Coptic))
    {
    }

    protected CopticOldBohairicAnalyzer(LanguageInfo languageInfo) : base(languageInfo, OldBohairicSimpleIpaTranscriptions, _oldBohairicWordCache)
    {
    }

    protected override void PhoneticAnalysisInternal(PhoneticWord word, string srcWord)
    {
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

            // Handle special rules
            if (isLastChar && ch == 'ⲃ')
            {
                ipa = "b";
            }
            if (ch == 'ⲩ')
            {
                if (chPrev == 'ⲟ')
                {
                    // Is this correct? No source is very clear on the cases

                    // Technically this is a digraph, but this code
                    // acts like the upsilon is silent and it's
                    // actually O making all the sound
                    ipa = string.Empty;
                    ipaWord[i - 1] = new(chPrev, "u");
                }
                else
                {
                    ipa = "w";
                }
            }
            else if (ch == 'ⲉ' && chPrev == 'ⲓ')
            {
                // Technically this is a digraph, but this code
                // acts like the yota is silent and it's
                // actually ⲉ making all the sound
                ipa = "e";
                ipaWord[i - 1] = new(chPrev, string.Empty);
            }
            else if (ch == 'ⲓ')
            {
                // Becomes a "yuh" sound before a vowel, schwa after
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
                    ipa = "ə";
            }
            else if (ch == 'ⲅ')
            {
                var chNextIpa = i < ipaWord.Count - 1 ? ipaWord[i + 1].Ipa : null;

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
            else if (chNextVow && ch == '\u0300')
            {
                ipa = ".";
            }

            // Use original character if no IPA equivalent is found
            var equivalent = ipaWord[i];
            equivalent.Ipa = ipa ?? ch.ToString();
            ipaWord[i] = equivalent;
        }
    }

    protected static IReadOnlyDictionary<char, string> OldBohairicSimpleIpaTranscriptions = new Dictionary<char, string>
    {
        ['ⲁ'] = "a",
        ['ⲃ'] = "β",
        ['ⲅ'] = "g",
        ['ⲇ'] = "d",
        ['ⲉ'] = "ə",
        ['ⲍ'] = "z",
        ['ⲏ'] = "e",
        ['ⲑ'] = "tʰ",
        ['ⲓ'] = "i",
        ['ⲕ'] = "k",
        ['ⲗ'] = "l",
        ['ⲙ'] = "m",
        ['ⲛ'] = "n",
        ['ⲝ'] = "ks",
        ['ⲟ'] = "ɔ",
        ['ⲡ'] = "p",
        ['ⲣ'] = "ɾ",
        ['ⲥ'] = "s",
        ['ⲧ'] = "d",
        ['ⲩ'] = "w",
        ['ⲫ'] = "pʰ",
        ['ⲭ'] = "kʰ",
        ['ⲯ'] = "ps",
        ['ⲱ'] = "o",
        ['ϣ'] = "ʃ",
        ['ϥ'] = "f",
        ['ϧ'] = "x",
        ['ϩ'] = "h",
        ['ϫ'] = "t͡ʃ",
        ['ϭ'] = "t͡ʃʰ",
        ['ϯ'] = "də",
        ['\u0300'] = "ɛ"
    };
}
