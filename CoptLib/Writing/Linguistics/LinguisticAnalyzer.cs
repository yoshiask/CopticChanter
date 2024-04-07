using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoptLib.Writing.Linguistics;

public abstract class LinguisticAnalyzer(LanguageInfo languageInfo)
{
    public static readonly char[] Separators = [' ', ',', ':', ';', '.', '/', '"', '\''];

    public LanguageInfo Language { init; get; } = languageInfo;

    /// <summary>
    /// Analyzes the pronunciation text of <see cref="Language"/>.
    /// </summary>
    /// <param name="srcText">The text to analyze.</param>
    /// <returns>An array of transcribed words using IPA.</returns>
    public abstract PhoneticWord[] PhoneticAnalysis(string srcText);

    /// <summary>
    /// Transcribes text into
    /// <see href="https://en.wikipedia.org/wiki/International_Phonetic_Alphabet">IPA</see>.
    /// </summary>
    /// <param name="srcText">The <see cref="Language"/> text to transcribe.</param>
    /// <remarks>
    /// Use <seealso cref="PhoneticAnalysis(string)"/> for more granular results.
    /// </remarks>
    public string IpaTranscribe(string srcText)
    {
        var words = PhoneticAnalysis(srcText);

        return string.Join(null, words.Select(
            word => word.ToIpaString()
        ));
    }

    /// <summary>
    /// Transliterates text to the specified script.
    /// </summary>
    /// <param name="srcText">The <see cref="Language"/> text to transcribe.</param>
    /// <param name="lang">The script to transliterate to.</param>
    /// <param name="srcTextLength">The number of characters in the original string. Used for optimization.</param>
    /// <param name="syllableSeparator">
    /// The <see cref="string"/> to separate syllables with,
    /// or <see langword="null"/> to not separate syllables.
    /// </param>
    public virtual string Transliterate(IEnumerable<PhoneticWord> srcText, KnownLanguage lang,
        int srcTextLength = 0, SyllableSeparatorSet? syllableSeparatorsOverride = null)
    {
        IReadOnlyDictionary<string, string> ipaTable;
        if (lang == KnownLanguage.IPA)
            ipaTable = new NothingDictionary<string>();
        else if (!IpaTables.IpaToLanguage.TryGetValue(lang, out ipaTable))
            throw new ArgumentException($"{lang} is not a supported transliteration target.");

        // Create syllable separator set for transliteration target
        var ipaSyllableSeparatorSet = SyllableSeparatorSet.IPA;
        SyllableSeparatorSet syllableSeparators = syllableSeparatorsOverride ?? new()
        {
            PrimaryStressed = ipaTable.TryGetValue(ipaSyllableSeparatorSet.PrimaryStressed, out var tlPSep)
                ? tlPSep : ipaSyllableSeparatorSet.PrimaryStressed,
            SecondaryStressed = ipaTable.TryGetValue(ipaSyllableSeparatorSet.SecondaryStressed, out var tlSSep)
                ? tlSSep : ipaSyllableSeparatorSet.SecondaryStressed,
            Unstressed = ipaTable.TryGetValue(ipaSyllableSeparatorSet.Unstressed, out var tlUSep)
                ? tlUSep : ipaSyllableSeparatorSet.Unstressed,
        };

        StringBuilder sb = new(srcTextLength);
        foreach (var word in srcText)
        {
            var syllableBreaks = word.SyllableBreaks;

            for (int i = 0; i < word.Length; i++)
            {
                if (syllableBreaks.Contains(i))
                {
                    var sep = word.GetSeparatorForSyllable(i, syllableSeparators);
                    sb.Append(sep);
                }

                var pe = word.Equivalents[i];

                // Look up IPA letter in table, if it doesn't
                // exist, leave it as is
                if (!ipaTable.TryGetValue(pe.Ipa.ToLowerInvariant(), out var tl))
                    tl = pe.Ipa;

                tl = pe.IsUpper && tl.Length >= 1 ? char.ToUpper(tl[0]) + tl[1..] : tl;
                sb.Append(tl);
            }
        }

        return sb.ToString();
    }
    
    /// <summary>
    /// Transliterates text to the specified script.
    /// </summary>
    /// <param name="srcText">The <see cref="Language"/> text to transcribe.</param>
    /// <param name="lang">The script to transliterate to.</param>
    /// <param name="syllableSeparatorOverride">
    /// The <see cref="string"/> to separate syllables with,
    /// or <see langword="null"/> to not separate syllables.
    /// </param>
    public string Transliterate(string srcText, KnownLanguage lang, SyllableSeparatorSet? syllableSeparatorOverride = null)
        => Transliterate(PhoneticAnalysis(srcText), lang, srcText.Length, syllableSeparatorOverride);

    /// <summary>
    /// Transcribes text into
    /// <see href="https://en.wikipedia.org/wiki/International_Phonetic_Alphabet">IPA</see>,
    /// then generates <see href="https://en.wikipedia.org/wiki/Speech_Synthesis_Markup_Language">SSML</see>
    /// with the pronunciation.
    /// </summary>
    /// <param name="srcText">The <see cref="Language"/> text to transcribe.</param>
    public (string, string) GenerateSsml(string srcText)
    {
        var words = PhoneticAnalysis(srcText);

        string[] ipaWords = new string[words.Length];
        string[] srcWords = new string[words.Length];
        for (int i = 0; i < words.Length; i++)
        {
            var word = words[i];
            ipaWords[i] = word.ToIpaString().ToLower();
            srcWords[i] = word.ToString(false);
        }

        string ipaText = string.Join(null, ipaWords);

        StringBuilder ssmlSentenceBuilder = new();
        for (int w = 0; w < words.Length; w++)
        {
            var ipaWord = ipaWords[w];
            var srcWord = srcWords[w];

            if (srcWord[0] == ' ')
                ssmlSentenceBuilder.Append(' ');
            else if (Separators.Contains(srcWord[0]))
                ssmlSentenceBuilder.Append("<break time=\"300ms\"/>");
            else
                ssmlSentenceBuilder.Append($"<phoneme alphabet=\"ipa\" ph=\"{ipaWord}\">{srcWord}</phoneme>");
        }

        string ssmlSentence = ssmlSentenceBuilder.ToString();
        string ssml = $"<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"en-US\">\r\n    <voice name=\"el-GR-NestorasNeural\">\r\n        <s>{ssmlSentence}</s>\r\n    </voice>\r\n</speak>";
        return (ssml, ipaText);
    }

    /// <summary>
    /// Gets the abbreviation with the provided <see cref="key"/>.
    /// </summary>
    /// <param name="key">The key of the abbreviation to identify.</param>
    /// <param name="keepAbbreviated">Whether to expand the abbreviation to its full meaning.</param>
    /// <param name="capitalize">Whether to force the result to be in sentence case.</param>
    public virtual string ResolveAbbreviation(string key, bool keepAbbreviated, bool? capitalize = null)
    {
        var result = ResolveAbbreviationInternal(key.ToLower(), keepAbbreviated);
        
        capitalize ??= char.IsUpper(key[0]);
        
        return capitalize.Value
            ? char.ToUpper(result[0]) + result[1..]
            : result;
    }

    protected abstract string ResolveAbbreviationInternal(string key, bool keepAbbreviated);
}
