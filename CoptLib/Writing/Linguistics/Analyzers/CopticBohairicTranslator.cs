using CoptLib.Extensions;
using CoptLib.Writing.Lexicon;
using CoptLib.Writing.Linguistics.XBar;
using OwlCore.ComponentModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CoptLib.Writing.Linguistics.Analyzers;

public class CopticBohairicTranslator : ITranslator
{
    private LanguageInfo _language = new(KnownLanguage.CopticBohairic);
    private readonly ILexicon _lexicon = new CopticScriptoriumLexicon();
    private readonly CopticBohairicGrammar _grammar = new();

    public Task SetSourceLanguageAsync(LanguageInfo language) => Task.Run(() => _language = language);

    public async IAsyncEnumerable<List<IStructuralElement>> AnnotateAsync(string srcText)
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

            var endIndex = currentOffset + srcWord.Length;
            var range = new Range(currentOffset, endIndex);

            var srcWordNorm = NormalizeText(srcWord);

            List<StructuralElement> wordComponents = [];

            if (wordComponents is not null)
                sentence.AddRange(wordComponents);

            currentOffset = endIndex;
        }

        yield return sentence;
    }

    public Task<BinaryNode<IStructuralElement>> TranslateAsync(IAsyncEnumerable<IStructuralElement> annotatedText)
    {
        throw new NotImplementedException();
    }

    public async IAsyncEnumerable<List<IStructuralElement>> IdentifyNoun(string word, List<IStructuralElement>? existingElements = null)
    {
        // TODO: Remove
        if (_lexicon is IAsyncInit lexiconInit)
            await lexiconInit.InitAsync();

        existingElements ??= [];
        var startIndex = existingElements.LastOrDefault()?.SourceRange.End ?? Index.Start;

        var wordEntries = _lexicon.BasicSearchAsync(word, _language);
        await foreach (var wordEntry in wordEntries)
        {

            var baseRange = new Range(startIndex, Index.End);
            var baseElement = new StructuralLexeme(baseRange, wordEntry, 0);

            yield return new(existingElements)
            {
                baseElement
            };
        }

        // Check each possible prefix
        foreach (var prefix in _grammar.NounPrefixes)
        {
            var match = prefix.Pattern.MatchAsPrefix(word);
            if (match is null)
                continue;

            var meta = prefix.MetaFactory();
            if (meta is null)
                continue;

            if (existingElements.Count > 0)
            {
                // There are already some prefixes, let's do some pruning!

                bool isArticle = _grammar.Articles.Contains(prefix);
                if (meta is IDeterminerMeta determinerMeta && existingElements.Any(e => e is IDeterminerMeta or DeterminerElement))
                    continue;
            }

            var baseStart = match.Groups.Count > 1
                ? match.Groups[1]!.Length
                : match.End;

            Range range = new Range(match.Start, baseStart) + startIndex;

            StructuralElement element = meta switch
            {
                IDeterminerMeta detMeta => new DeterminerElement(range, detMeta),
                PrepositionMeta prepMeta => new PrepositionElement(range, prepMeta),

                _ => throw new NotImplementedException($"Unrecognized meta type: {meta.GetType().Name}")
            };

            List<IStructuralElement> newList = new(existingElements)
            {
                element
            };

            if (baseStart < word.Length)
            {
                var baseRange = new Range(baseStart, Index.End);
                var baseWord = word.Substring(baseRange);

                await foreach (var child in IdentifyNoun(baseWord, newList))
                    yield return child;
            }

            // TODO: Yield this interpretation if we've consumed the entire string
            //yield return newList;
        }
    }

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

    public static string NormalizeText(string text)
    {
        var normalizedString = text.ToLower().Normalize(NormalizationForm.FormD);
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

    private static int GetSequenceHashCode<T>(IEnumerable<T> sequence)
    {
        const int seed = 487;
        const int modifier = 31;

        unchecked
        {
            return sequence.Aggregate(seed, (current, item) =>
                (current * modifier) + item.GetHashCode());
        }
    }

    private static readonly Regex Verb1stSingRegex = new(@"^((?<rela>ⲉⲧ)?(?<pret>ⲛⲁ|ⲛⲉ)?(?<circ>ⲉ|ⲉⲁ)?((?<neg>ⲙⲡ)?(?<focl>ⲁ)?(?<juss>ⲙⲁⲣ|ⲉⲛⲑⲣ)?|(?<aor>ϣⲁ)?)(ϯ|ⲓ)(?<cond>ϣⲁⲛ|(?<cndn>ϣⲧⲉⲙ))?(?<futr>ⲛⲁ)?|(?<conj>ⲛⲧⲁ|ⲧⲁⲣⲓ)|(?<optn>ⲛⲛⲁ))(?<base>\w+)$");
    private static readonly Regex Verb1stPlurRegex = new(@"^((?<rela>ⲉⲧ)?(?<pret>ⲛⲁ|ⲛⲉ)?(?<circ>ⲉ|ⲉⲁ)?((?<neg>ⲙⲡⲉ?)?(?<focl>ⲁ)?(?<juss>ⲙⲁⲣ|ⲉⲛⲑⲣ)?|(?<aor>ϣⲁ)?)(ⲛ|ⲧ?ⲉⲛ)(?<cond>ϣⲁⲛ|(?<cndn>ϣⲧⲉⲙ))?(?<futr>ⲛⲁ)?|(?<conj>ⲛⲧⲁ|ⲧⲁⲣⲉⲛ)|(?<optn>ⲛⲛⲁ))(?<base>\w+)$");
}
