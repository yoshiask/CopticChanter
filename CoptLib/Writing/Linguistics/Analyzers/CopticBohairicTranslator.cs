using AngleSharp.Dom;
using CoptLib.Extensions;
using CoptLib.Writing.Lexicon;
using CoptLib.Writing.Linguistics.XBar;
using OwlCore.ComponentModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CoptLib.Writing.Linguistics.Analyzers;

public class CopticBohairicTranslator : ITranslator
{
    private readonly ILexicon _lexicon = new CopticScriptoriumLexicon();

    public async IAsyncEnumerable<List<IStructuralElement>> AnnotateAsync(string srcText, LanguageInfo sourceLanguage)
    {
        if (_lexicon is IAsyncInit lexiconInit)
            await lexiconInit.InitAsync();

        string[] srcWords = srcText.SplitAndKeep(LinguisticAnalyzer.Separators).ToArray();
        List<IStructuralElement> sentence = new(srcWords.Length);

        int currentOffset = 0;
        for (int w = 0; w < srcWords.Length; w++)
        {
            var srcWord = srcWords[w];
            if (srcWord.Length <= 0)
                continue;

            IStructuralElement? element = null;
            var endIndex = currentOffset + srcWord.Length;
            var range = new Range(currentOffset, endIndex);

            var srcWordNorm = RemoveDiacritics(srcWord);

            List<StructuralElement> wordComponents = [];
            BreakAffixes(srcWordNorm, wordComponents);

            if (element is not null)
                sentence.Add(element);

            currentOffset = endIndex;
        }

        yield return sentence;
    }

    public Task<BinaryNode<IStructuralElement>> TranslateAsync(IAsyncEnumerable<IStructuralElement> annotatedText, LanguageInfo sourceLanguage)
    {
        throw new NotImplementedException();
    }

    private static bool BreakAffixes(string remainingWord, List<StructuralElement> components, int currentOffset = 0, bool? isNoun = null)
    {
        int originalComponentsCount = components.Count;

        // Prefixes that are used on verbs are generally more identifiable than
        // those used on nouns, so check those first.
        if (isNoun is null or false)
        {
            var verb1s = TryIdentifyVerb(remainingWord);
        }

        // Check for noun prefixes
        if ((isNoun is null or true) && remainingWord.StartsWithAny(CopticAnalyzer.NounPrefixes.Keys, out var prefix))
        {
            var meta = CopticAnalyzer.NounPrefixes[prefix];
            if (meta is IDeterminerMeta determinerMeta)
            {
                DeterminerElement element = new(new(currentOffset, currentOffset + prefix.Length), determinerMeta);
                components.Add(element);
            }
        }

        if (remainingWord.Length == 0 || currentOffset == remainingWord.Length || components.Count == originalComponentsCount)
        {
            // TODO: Check if base morph actually exists
            return true;
        }

        remainingWord = remainingWord[..currentOffset];
        currentOffset = components[^1].SourceRange.End.Value;

        return BreakAffixes(remainingWord, components, currentOffset, isNoun);
    }

    public static IEnumerable<VerbMeta> IdentifyVerb(string word)
    {
        List<(Regex, Lazy<NounMeta>)> verbConjs =
        [
            (Verb1stSingRegex, new(() => new(Gender.Unspecified, GrammaticalCount.Singular, PointOfView.First))),
            (Verb1stPlurRegex, new(() => new(Gender.Unspecified, GrammaticalCount.Plural, PointOfView.First))),
            // TODO: Add other conjugations
        ];

        foreach ((var rx, var actor) in verbConjs)
        {
            var match = MatchTense(word, rx);
            if (match is null)
                continue;

            // TODO: Check for base verb in the dictionary
            yield return new(match, actor.Value, null);
        }
    }

    public static VerbMeta? TryIdentifyVerb(string word) => IdentifyVerb(word).FirstOrDefault();

    private static TenseMeta? MatchTense(string word, Regex verbRx)
    {
        var match = verbRx.Match(word);
        if (!match.Success)
            return null;

        var groups = match.Groups;
        var relative = groups["rela"].Success;
        var preterite = groups["pret"].Success;
        var circumstantial = groups["circ"].Success;
        var negative = groups["neg"].Success || groups["optn"].Success || groups["cndn"].Success;
        var focalized = groups["focl"].Success;
        var conditional = groups["cond"].Success;
        var future = groups["futr"].Success;
        var jussive = groups["juss"].Success;
        var aorist = groups["aor"].Success;

        var baseWord = groups["base"].Value;
        var prefix = word[..^baseWord.Length];

        var currentTime = RelativeTime.Unspecified;
        var start = RelativeTime.Unspecified;
        var end = RelativeTime.Unspecified;
        int degree = 0;
        TenseFlags flags = default;

        if (future)
        {
            start = RelativeTime.Future;
            currentTime = RelativeTime.Present;
        }
        else if (aorist)
        {
            start = end = RelativeTime.Aorist;
            flags |= TenseFlags.Ending;
        }

        if (circumstantial)
            flags |= TenseFlags.Circumstantial;
        if (negative)
            flags |= TenseFlags.Negative;
        if (relative)
            flags |= TenseFlags.Relative;
        if (conditional)
            flags |= TenseFlags.Conditional;

        return new(currentTime, start, end, flags, degree);
    }

    private static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

        for (int i = 0; i < normalizedString.Length; i++)
        {
            char c = normalizedString[i];
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder
            .ToString()
            .Normalize(NormalizationForm.FormC);
    }

    private static readonly Regex Verb1stSingRegex = new(@"^((?<rela>ⲉⲧ)?(?<pret>ⲛⲁ|ⲛⲉ)?(?<circ>ⲉ|ⲉⲁ)?((?<neg>ⲙⲡ)?(?<focl>ⲁ)?(?<juss>ⲙⲁⲣ|ⲉⲛⲑⲣ)?|(?<aor>ϣⲁ)?)(ϯ|ⲓ)(?<cond>ϣⲁⲛ|(?<cndn>ϣⲧⲉⲙ))?(?<futr>ⲛⲁ)?|(?<conj>ⲛⲧⲁ|ⲧⲁⲣⲓ)|(?<optn>ⲛⲛⲁ))(?<base>\w+)$");
    private static readonly Regex Verb1stPlurRegex = new(@"^((?<rela>ⲉⲧ)?(?<pret>ⲛⲁ|ⲛⲉ)?(?<circ>ⲉ|ⲉⲁ)?((?<neg>ⲙⲡⲉ?)?(?<focl>ⲁ)?(?<juss>ⲙⲁⲣ|ⲉⲛⲑⲣ)?|(?<aor>ϣⲁ)?)(ⲛ|ⲧ?ⲉⲛ)(?<cond>ϣⲁⲛ|(?<cndn>ϣⲧⲉⲙ))?(?<futr>ⲛⲁ)?|(?<conj>ⲛⲧⲁ|ⲧⲁⲣⲉⲛ)|(?<optn>ⲛⲛⲁ))(?<base>\w+)$");
}
