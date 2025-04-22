using System.Collections.Generic;
using System.Text.RegularExpressions;
using CoptLib.Writing.Lexicon;
using CoptLib.Writing.Linguistics.XBar;

namespace CoptLib.Writing.Linguistics.Analyzers;

public partial class CopticBohairicGrammar
{
    private IEnumerable<SemanticPair>? _verbPrefixes;

    public SemanticPair VerbNominalizer { get; } =
        new("ϫⲓⲛ", _ => new NominalizingMeta(NominalizingType.Verb, NominalizingType.Noun));

    public IEnumerable<SemanticPair> VerbConjugationPrefixes { get; } =
    [
        // Present basic
        ..GenerateSemanticPairForVerbTense(new TenseMeta(RelativeTime.Present), [
            ("ϯ", new InflectionMeta(Gender.Unspecified, GrammaticalCount.Singular, PointOfView.First)),
            ("ⲕ", new InflectionMeta(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Second)),
            ("ϥ", new InflectionMeta(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Third)),
            ("ⲧⲉⲛ", new InflectionMeta(Gender.Unspecified, GrammaticalCount.Plural, PointOfView.First)),
            ("ⲧⲉⲧⲉⲛ", new InflectionMeta(Gender.Unspecified, GrammaticalCount.Plural, PointOfView.Second)),
            ("ⲧⲉ", new InflectionMeta(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Third)),
            ("ⲥⲉ", new InflectionMeta(Gender.Unspecified, GrammaticalCount.Plural, PointOfView.Third)),
            ("ⲥ", new InflectionMeta(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Second)),
        ]),

        // Present circumstantial
        ..GenerateSemanticPairForVerbTense(new TenseMeta(RelativeTime.Present, Flags: TenseFlags.Circumstantial), [
            ("ⲉⲓ", new InflectionMeta(Gender.Unspecified, GrammaticalCount.Singular, PointOfView.First)),
            ("ⲉⲕ", new InflectionMeta(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Second)),
            ("ⲉⲣⲉ", new InflectionMeta(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Second)),
            ("ⲉϥ", new InflectionMeta(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Third)),
            ("ⲉⲥ", new InflectionMeta(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Third)),
            ("ⲉⲛ", new InflectionMeta(Gender.Unspecified, GrammaticalCount.Plural, PointOfView.First)),
            ("ⲉⲣⲉⲧⲉⲛ", new InflectionMeta(Gender.Unspecified, GrammaticalCount.Plural, PointOfView.Second)),
            ("ⲉⲩ", new InflectionMeta(Gender.Unspecified, GrammaticalCount.Plural, PointOfView.Third)),
        ]),

        // Present relative
        ..GenerateSemanticPairForVerbTense(new TenseMeta(RelativeTime.Present, Flags: TenseFlags.Relative), [
            ("ⲉϯ", new InflectionMeta(Gender.Unspecified, GrammaticalCount.Singular, PointOfView.First)),
            ("ⲉⲧⲉⲕ", new InflectionMeta(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Second)),
            ("ⲉⲧⲉ", new InflectionMeta(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Second)),
            ("ⲉⲧⲉϥ", new InflectionMeta(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Third)),
            ("ⲉⲧⲉⲥ", new InflectionMeta(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Third)),
            ("ⲉⲧⲉⲛ", new InflectionMeta(Gender.Unspecified, GrammaticalCount.Plural, PointOfView.First)),
            ("ⲉⲧⲉⲛⲧⲉⲛ", new InflectionMeta(Gender.Unspecified, GrammaticalCount.Plural, PointOfView.Second)),
            ("ⲉⲧⲟⲩ", new InflectionMeta(Gender.Unspecified, GrammaticalCount.Plural, PointOfView.Third)),
        ]),

        // Future basic
        ..GenerateSemanticPairForVerbTense(new TenseMeta(RelativeTime.Future), [
            ("ϯⲛⲁ", new InflectionMeta(Gender.Unspecified, GrammaticalCount.Singular, PointOfView.First)),
            ("ⲭⲛⲁ", new InflectionMeta(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Second)),
            ("ϥⲛⲁ", new InflectionMeta(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Third)),
            ("ⲥⲛⲁ", new InflectionMeta(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Second)),
            ("ⲧⲉⲛⲁ", new InflectionMeta(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Third)),
            ("ⲧⲉⲛⲛⲁ", new InflectionMeta(Gender.Unspecified, GrammaticalCount.Plural, PointOfView.First)),
            ("ⲧⲉⲧⲉⲛⲛⲁ", new InflectionMeta(Gender.Unspecified, GrammaticalCount.Plural, PointOfView.Second)),
            ("ⲥⲉⲛⲁ", new InflectionMeta(Gender.Unspecified, GrammaticalCount.Plural, PointOfView.Third)),
        ]),

        // Conditional
        ..GenerateSemanticPairForVerbTense(new TenseMeta(RelativeTime.Future), [
            (new Regex("(ⲁⲓ(ϣⲁⲛ)?)"),
                new InflectionMeta(Gender.Unspecified, GrammaticalCount.Singular, PointOfView.First)),
            (new Regex("(ⲁⲕ(ϣⲁⲛ)?)"),
                new InflectionMeta(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Second)),
            (new Regex("(ⲁⲣⲉ(ϣⲁⲛ)?)"),
                new InflectionMeta(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Third)),
            (new Regex("(ⲁϥ(ϣⲁⲛ)?)"),
                new InflectionMeta(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Second)),
            (new Regex("(ⲁⲥ(ϣⲁⲛ)?)"),
                new InflectionMeta(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Third)),
            (new Regex("(ⲁⲛ(ϣⲁⲛ)?)"),
                new InflectionMeta(Gender.Unspecified, GrammaticalCount.Plural, PointOfView.First)),
            (new Regex("(ⲁⲣⲉⲧⲉⲛ(ϣⲁⲛ)?)"),
                new InflectionMeta(Gender.Unspecified, GrammaticalCount.Plural, PointOfView.Second)),
            (new Regex("(ⲁⲩ(ϣⲁⲛ)?)"),
                new InflectionMeta(Gender.Unspecified, GrammaticalCount.Plural, PointOfView.Third)),
        ]),
    ];

    private static IEnumerable<SemanticPair> GenerateSemanticPairForVerbTense(TenseMeta tense,
        List<(Pattern, InflectionMeta)> inflections)
    {
        foreach (var (pattern, inflection) in inflections)
            yield return new SemanticPair(pattern, _ => new VerbMeta(tense, inflection));
    }
}