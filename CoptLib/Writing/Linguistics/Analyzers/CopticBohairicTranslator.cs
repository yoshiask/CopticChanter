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
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CoptLib.Writing.Linguistics.Analyzers;

public class CopticBohairicTranslator : ITranslator, IAsyncInit
{
    private LanguageInfo _language = new(KnownLanguage.CopticBohairic);
    private ILexicon _lexicon;
    private readonly CopticBohairicGrammar _grammar = new();

    public Task SetSourceLanguageAsync(LanguageInfo language) => Task.Run(() => _language = language);

    public async IAsyncEnumerable<IAsyncEnumerable<IEnumerable<IStructuralElement>>> AnnotateAsync(string srcText)
    {
        await InitAsync();

        string[] srcWords = srcText.Split(LinguisticAnalyzer.Separators);

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

            yield return IdentifyWord(srcWordNorm);

            currentOffset = endIndex;
        }
    }

    public Task<BinaryNode<IStructuralElement>> TranslateAsync(IAsyncEnumerable<IStructuralElement> annotatedText)
    {
        throw new NotImplementedException();
    }

    public async IAsyncEnumerable<IEnumerable<IStructuralElement>> IdentifyWord(string word)
    {
        // Check for preposition
        foreach (var prefix in _grammar.Prepositions)
        {
            var match = prefix.Pattern.MatchAsPrefix(word);
            if (match is null)
                continue;

            var meta = prefix.MetaFactory();
            if (meta is null)
                continue;

            yield return StructuralElement.FromMeta(Range.All, meta);
        }

        await foreach (var nounInterpretation in IdentifyNoun(word))
            yield return nounInterpretation;
    }

    public async IAsyncEnumerable<List<IStructuralElement>> IdentifyNoun(string word, List<IStructuralElement>? existingElements = null)
    {
        await InitAsync();

        existingElements ??= [];
        var startIndex = existingElements.LastOrDefault()?.SourceRange.End ?? Index.Start;

        var wordEntries = _lexicon.BasicSearchAsync(word, _language);
        await foreach (var wordEntry in wordEntries)
        {
            // Get the form of the lemma that matches its usage here
            var form = wordEntry.Forms.FirstOrDefault(f => f.Usage == _language && f.Orthography == word);
            if (form is null)
                continue;

            // Verify that this is a noun
            var grammarGroup = form.GrammarGroup ?? wordEntry.GrammarGroup;
            if (grammarGroup.PartOfSpeech != PartOfSpeech.Substantive)
                continue;

            // Check that the gender and number of the base word match any prefixes
            var baseGender = grammarGroup.Gender;
            var baseNumber = grammarGroup.Number.ToGrammaticalCount();
            
            Gender previousGender = Gender.Unspecified;
            GrammaticalCount previousNumber = GrammaticalCount.Unspecified;
            for (int i = 0; i < existingElements.Count; i++)
            {
                var element = existingElements[i];
                if (element is PrepositionElement)
                {
                    // Preposition is linking two ideas, so it effectively resets the inflection
                    previousGender = Gender.Unspecified;
                    previousNumber = GrammaticalCount.Unspecified;
                    continue;
                }

                (var gender, var number) = ElementToGenderAndCount(element);
                if (gender is not Gender.Unspecified)
                    previousGender = gender;
                if (number is not GrammaticalCount.Unspecified)
                    previousNumber = number;
            }

            if (previousGender is not Gender.Unspecified && baseGender is not Gender.Unspecified)
            {
                if (previousGender != baseGender)
                    continue;
            }
            if (previousNumber is not GrammaticalCount.Unspecified && baseNumber is not GrammaticalCount.Unspecified)
            {
                if (previousNumber != baseNumber)
                    continue;
            }

            var baseNounMeta = new NounMeta(new LexiconEntryReference(wordEntry, form),
                new(grammarGroup.Gender, grammarGroup.Number.ToGrammaticalCount()));
            var baseRange = new Range(startIndex, Index.End);
            var baseElement = new NounElement(baseRange, baseNounMeta);

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
                if (meta is IDeterminerMeta determinerMeta && existingElements.OfType<DeterminerElement>().Any())
                    continue;
            }

            var baseStart = match.Groups.Count > 1
                ? match.Groups[1]!.Length
                : match.End;

            Range range = new Range(match.Start, baseStart) + startIndex;

            List<IStructuralElement> newList = new(existingElements);
            newList.AddRange(StructuralElement.FromMeta(range, meta));

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

    public async Task InitAsync(CancellationToken cancellationToken = default)
    {
        if (!IsInitialized)
        {
            bool useXml = true;
            if (useXml)
            {
                var teiDoc = XDocument.Load(@"C:\Users\jjask\source\repos\KELLIA\dictionary\xml\Comprehensive_Coptic_Lexicon-v1.2-2020.xml");
                _lexicon = new TeiLexicon(_language, teiDoc, []);
            }
            else
            {
                _lexicon = new CopticScriptoriumLexicon();
            }

            if (_lexicon is IAsyncInit lexiconInit)
                await lexiconInit.InitAsync();
        }

        IsInitialized = true;
    }

    private static (Gender, GrammaticalCount) ElementToGenderAndCount(IStructuralElement elem)
    {
        if (elem is DeterminerElement detElem)
        {
            var inflection = detElem.Meta switch
            {
                DeterminerArticleMeta articleMeta => articleMeta.Target,
                DeterminerPossessiveMeta possessiveMeta => possessiveMeta.Possessed,
                DeterminerDemonstrativeMeta possessiveMeta => possessiveMeta.Target,
                DeterminerQuantifyingMeta possessiveMeta => possessiveMeta.Target,
                _ => throw new NotImplementedException(),
            };
            return (inflection.Gender, inflection.Number);
        }
        if (elem is StructuralLexeme lexElem)
        {
            var grammarGroup = lexElem.Entry.GrammarGroup;
            return (grammarGroup.Gender, grammarGroup.Number.ToGrammaticalCount());
        }

        return (Gender.Unspecified, GrammaticalCount.Unspecified);
    }

    private static readonly Regex Verb1stSingRegex = new(@"^((?<rela>ⲉⲧ)?(?<pret>ⲛⲁ|ⲛⲉ)?(?<circ>ⲉ|ⲉⲁ)?((?<neg>ⲙⲡ)?(?<focl>ⲁ)?(?<juss>ⲙⲁⲣ|ⲉⲛⲑⲣ)?|(?<aor>ϣⲁ)?)(ϯ|ⲓ)(?<cond>ϣⲁⲛ|(?<cndn>ϣⲧⲉⲙ))?(?<futr>ⲛⲁ)?|(?<conj>ⲛⲧⲁ|ⲧⲁⲣⲓ)|(?<optn>ⲛⲛⲁ))(?<base>\w+)$");
    private static readonly Regex Verb1stPlurRegex = new(@"^((?<rela>ⲉⲧ)?(?<pret>ⲛⲁ|ⲛⲉ)?(?<circ>ⲉ|ⲉⲁ)?((?<neg>ⲙⲡⲉ?)?(?<focl>ⲁ)?(?<juss>ⲙⲁⲣ|ⲉⲛⲑⲣ)?|(?<aor>ϣⲁ)?)(ⲛ|ⲧ?ⲉⲛ)(?<cond>ϣⲁⲛ|(?<cndn>ϣⲧⲉⲙ))?(?<futr>ⲛⲁ)?|(?<conj>ⲛⲧⲁ|ⲧⲁⲣⲉⲛ)|(?<optn>ⲛⲛⲁ))(?<base>\w+)$");

    public bool IsInitialized { get; protected set; }
}
