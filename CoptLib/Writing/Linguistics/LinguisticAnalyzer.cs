using System;
using System.Linq;
using System.Text;

namespace CoptLib.Writing.Linguistics;

public abstract class LinguisticAnalyzer
{
    public LanguageInfo Language { get; }

    public LinguisticAnalyzer(LanguageInfo languageInfo)
    {
        Language = languageInfo;
    }

    /// <summary>
    /// Analyzes the pronunciation text of <see cref="Language"/>.
    /// </summary>
    /// <param name="srcText">The text to analyze.</param>
    /// <returns>An array of transcribed words using IPA.</returns>
    public abstract PhoneticEquivalent[][] PhoneticAnalysis(string srcText);

    /// <summary>
    /// Transcribes Coptic text using the Greco-Bohairic pronunciation into
    /// <see href="https://en.wikipedia.org/wiki/International_Phonetic_Alphabet">IPA</see>.
    /// </summary>
    /// <param name="srcText">The Coptic text to transcribe.</param>
    /// <remarks>
    /// Use <seealso cref="PhoneticAnalysis(string)"/> for more granular results.
    /// </remarks>
    public string IpaTranscribe(string srcText)
    {
        var words = PhoneticAnalysis(srcText);

        return string.Join(null, words.Select(
            word => string.Join(null, word.Select(
                ph => ph.GetIpa()
            )))
        );
    }

    /// <summary>
    /// Transliterates Coptic text to the specified language using
    /// the Greco-Bohairic pronunciation.
    /// </summary>
    /// <param name="srcText">The Coptic text to transcribe.</param>
    /// <param name="lang">The language to transliterate to.</param>
    /// <param name="srcTextLength">The number of characters in the original string. Used for optimization.</param>
    public string Transliterate(PhoneticEquivalent[][] srcText, KnownLanguage lang, int srcTextLength = 0)
    {
        if (!IpaTables.IpaToLanguage.TryGetValue(lang, out var ipaTable))
            throw new ArgumentException($"{lang} is not a supported transliteration target.");

        StringBuilder sb = new(srcTextLength);
        foreach (var word in srcText)
        {
            foreach (var pe in word)
            {
                // Look up IPA letter in table, if it doesn't
                // exist, leave it as is
                if (!ipaTable.TryGetValue(pe.Ipa.ToLowerInvariant(), out var tl))
                    tl = pe.Ipa;

                tl = pe.IsUpper ? char.ToUpper(tl[0]) + tl.Substring(1) : tl;
                sb.Append(tl);
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Transliterates Coptic text to the specified language using
    /// the Greco-Bohairic pronunciation.
    /// </summary>
    /// <param name="srcText">The Coptic text to transcribe.</param>
    /// <param name="lang">The language to transliterate to.</param>
    public string Transliterate(string srcText, KnownLanguage lang)
        => Transliterate(PhoneticAnalysis(srcText), lang);

    /// <summary>
    /// Replaces all abbreviations with the full word.
    /// </summary>
    /// <param name="srcText">The source text.</param>
    public virtual string ExpandAbbreviations(string srcText) => srcText;
}
