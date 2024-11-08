using CoptLib.Writing.Lexicon;
using CoptLib.Writing.Linguistics.XBar;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CoptLib.Writing.Linguistics.Analyzers;

public class CopticBohairicGrammar
{
    const string VILMINOR = "ⲃⲓⲗⲙⲛⲟⲣⲫⲯ";

    const string VILMINOR_REGEX = $"[{VILMINOR}]";
    const string NOT_VILMINOR_REGEX = $"[^{VILMINOR}]";

    private IEnumerable<SemanticPair>? _determiners;
    private IEnumerable<SemanticPair>? _nounPrefixes;

    public IEnumerable<SemanticPair> Articles { get; } =
    [
        // Definite
        new(new Regex($"(ⲡ){NOT_VILMINOR_REGEX}"), () => new DeterminerArticleMeta(DeterminerStrength.Weak, true, new(Gender.Masculine, GrammaticalCount.Singular))),
        new(new Regex($"(ⲫ){VILMINOR_REGEX}"), () => new DeterminerArticleMeta(DeterminerStrength.Weak, true, new(Gender.Masculine, GrammaticalCount.Singular))),
        new("ⲡⲓ", () => new DeterminerArticleMeta(DeterminerStrength.Strong, true, new(Gender.Masculine, GrammaticalCount.Singular))),
        new(new Regex($"(ⲧ){NOT_VILMINOR_REGEX}"), () => new DeterminerArticleMeta(DeterminerStrength.Weak, true, new(Gender.Feminine, GrammaticalCount.Singular))),
        new(new Regex($"(ⲑ){VILMINOR_REGEX}"), () => new DeterminerArticleMeta(DeterminerStrength.Weak, true, new(Gender.Feminine, GrammaticalCount.Singular))),
        new("ϯ", () => new DeterminerArticleMeta(DeterminerStrength.Strong, true, new(Gender.Feminine, GrammaticalCount.Singular))),
        new("ⲛⲓ", () => new DeterminerArticleMeta(DeterminerStrength.Strong, true, new(Number: GrammaticalCount.Plural))),

        // Indefinite
        new("ⲟⲩ", () => new DeterminerArticleMeta(default, false, new(Number: GrammaticalCount.Singular))),
        new("ϩⲁⲛ", () => new DeterminerArticleMeta(default, false, new(Number: GrammaticalCount.Plural))),

        // Possessive Strong
        new(new Regex($"(ⲛ){NOT_VILMINOR_REGEX}"), () => new DeterminerPossessiveMeta(DeterminerStrength.Strong, InflectionMeta.Unspecified, InflectionMeta.Unspecified)),
        new(new Regex($"(ⲙ){VILMINOR_REGEX}"), () => new DeterminerPossessiveMeta(DeterminerStrength.Strong, InflectionMeta.Unspecified, InflectionMeta.Unspecified)),

        // Possessive 1st Person
        new("ⲡⲁ", () => new DeterminerPossessiveMeta(default, new(default, GrammaticalCount.Singular, PointOfView.First), new(Gender.Masculine, GrammaticalCount.Singular))),
        new("ⲧⲁ", () => new DeterminerPossessiveMeta(default, new(default, GrammaticalCount.Singular, PointOfView.First), new(Gender.Feminine, GrammaticalCount.Singular))),
        new("ⲛⲁ", () => new DeterminerPossessiveMeta(default, new(default, GrammaticalCount.Singular, PointOfView.First), new(Number: GrammaticalCount.Plural))),
        new("ⲡⲉⲛ", () => new DeterminerPossessiveMeta(default, new(default, GrammaticalCount.Plural, PointOfView.First), new(Gender.Masculine, GrammaticalCount.Singular))),
        new("ⲧⲉⲛ", () => new DeterminerPossessiveMeta(default, new(default, GrammaticalCount.Plural, PointOfView.First), new(Gender.Feminine, GrammaticalCount.Singular))),
        new("ⲛⲉⲛ", () => new DeterminerPossessiveMeta(default, new(default, GrammaticalCount.Plural, PointOfView.First), new(Number: GrammaticalCount.Plural))),

        // Possessive 2nd Person
        new("ⲡⲉⲕ", () => new DeterminerPossessiveMeta(default, new(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Second), new(Gender.Masculine, GrammaticalCount.Singular))),
        new("ⲧⲉⲕ", () => new DeterminerPossessiveMeta(default, new(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Second), new(Gender.Feminine, GrammaticalCount.Singular))),
        new("ⲛⲉⲕ", () => new DeterminerPossessiveMeta(default, new(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Second), new(Number: GrammaticalCount.Plural))),
        new("ⲡⲉ", () => new DeterminerPossessiveMeta(default, new(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Second), new(Gender.Masculine, GrammaticalCount.Singular))),
        new("ⲧⲉ", () => new DeterminerPossessiveMeta(default, new(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Second), new(Gender.Feminine, GrammaticalCount.Singular))),
        new("ⲛⲉ", () => new DeterminerPossessiveMeta(default, new(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Second), new(Number: GrammaticalCount.Plural))),
        new("ⲡⲉⲧⲉⲛ", () => new DeterminerPossessiveMeta(default, new(default, GrammaticalCount.Plural, PointOfView.Second), new(Gender.Masculine, GrammaticalCount.Singular))),
        new("ⲧⲉⲧⲉⲛ", () => new DeterminerPossessiveMeta(default, new(default, GrammaticalCount.Plural, PointOfView.Second), new(Gender.Feminine, GrammaticalCount.Singular))),
        new("ⲛⲉⲧⲉⲛ", () => new DeterminerPossessiveMeta(default, new(default, GrammaticalCount.Plural, PointOfView.Second), new(Number: GrammaticalCount.Plural))),

        // Possessive 3rd Person
        new("ⲡⲉϥ", () => new DeterminerPossessiveMeta(default, new(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Third), new(Gender.Masculine, GrammaticalCount.Singular))),
        new("ⲧⲉϥ", () => new DeterminerPossessiveMeta(default, new(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Third), new(Gender.Feminine, GrammaticalCount.Singular))),
        new("ⲛⲉϥ", () => new DeterminerPossessiveMeta(default, new(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Third), new(Number: GrammaticalCount.Plural))),
        new("ⲡⲉⲥ", () => new DeterminerPossessiveMeta(default, new(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Third), new(Gender.Masculine, GrammaticalCount.Singular))),
        new("ⲧⲉⲥ", () => new DeterminerPossessiveMeta(default, new(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Third), new(Gender.Feminine, GrammaticalCount.Singular))),
        new("ⲛⲉⲥ", () => new DeterminerPossessiveMeta(default, new(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Third), new(Number: GrammaticalCount.Plural))),
        new("ⲡⲟⲩ", () => new DeterminerPossessiveMeta(default, new(default, GrammaticalCount.Plural, PointOfView.Third), new(Gender.Masculine, GrammaticalCount.Singular))),
        new("ⲧⲟⲩ", () => new DeterminerPossessiveMeta(default, new(default, GrammaticalCount.Plural, PointOfView.Third), new(Gender.Feminine, GrammaticalCount.Singular))),
        new("ⲛⲟⲩ", () => new DeterminerPossessiveMeta(default, new(default, GrammaticalCount.Plural, PointOfView.Third), new(Number: GrammaticalCount.Plural))),

        // Demonstrative
        new("ⲡⲁⲓ", () => new DeterminerDemonstrativeMeta(DeterminerStrength.Near, new(Gender.Masculine, GrammaticalCount.Singular))),
        new("ⲧⲁⲓ", () => new DeterminerDemonstrativeMeta(DeterminerStrength.Near, new(Gender.Feminine, GrammaticalCount.Singular))),
        new("ⲛⲁⲓ", () => new DeterminerDemonstrativeMeta(DeterminerStrength.Near, new(Number: GrammaticalCount.Plural))),
    ];

    public IEnumerable<SemanticPair> ComplexPrefixes { get; } =
    [
        // Example: "ⲡⲓⲙⲁⲛϣⲉⲗⲉⲧ" / "the bridal chamber" ("the place of [the] bride")
        new("ⲙⲁ", () => new NounMeta(new ConceptReference("", "/c/en/house"), InflectionMeta.Unspecified)),

        // Example: "ϯⲙⲉⲧⲣⲉⲙⲛ̀ⲭⲏⲙⲓ" / "the Coptic language" ("[the language of] person of Egypt")
        new("ⲣⲉⲙ", () => new NounMeta(new ConceptReference("", "/c/en/person"), InflectionMeta.Unspecified)),

        // Example: "ⲟⲩⲣⲉϥⲉⲣⲛⲟⲃⲓ" / "a sinner" ("a person that sins")
        new("ⲣⲉϥ", () => new CompoundMeta([new NounMeta(new ConceptReference("", "/c/en/person"), InflectionMeta.Unspecified),])),
    ];

    public IEnumerable<SemanticPair> Determiners
    {
        get
        {
            return _determiners ??= [
                new("ⲉⲧⲉⲙⲙⲁⲩ", () => new DeterminerDemonstrativeMeta(DeterminerStrength.Far, InflectionMeta.Unspecified)),
                new("ⲛⲓⲃⲉⲛ", () => new DeterminerQuantifyingMeta(new(Number: GrammaticalCount.All))),
                .. Articles
            ];
        }
    }

    public IEnumerable<SemanticPair> NounPrefixes
    {
        get
        {
            return _nounPrefixes ??= [
                new("ⲉ", () => new PrepositionMeta(PrepositionType.To)),
                // TODO: "ⲉⲟⲩ" and its contraction "ⲉⲩ"

                new(new Regex($"(ⲛ){NOT_VILMINOR_REGEX}"), () => new PrepositionMeta(PrepositionType.Of)),
                new(new Regex($"(ⲙ){VILMINOR_REGEX}"), () => new PrepositionMeta(PrepositionType.Of)),

                .. ComplexPrefixes,
                .. Articles,
            ];
        }
    }

    public IEnumerable<SemanticPair> Prepositions { get; } =
    [
        new("ⲛⲧⲉ", () => new PrepositionMeta(PrepositionType.Of)),
        new("ϩⲓⲧⲉⲛ", () => new PrepositionMeta(PrepositionType.Through)),

        new("ⲁⲧϭⲛⲉ", () => new PrepositionMeta(PrepositionType.With, true)),
        new("ⲥⲁⲃⲟⲗ", () => new PrepositionMeta(PrepositionType.Away)),
        new("ϣⲁⲉⲃⲟⲗ", () => new PrepositionMeta(PrepositionType.In, true)),
        new("ⲉⲑⲃⲉ", () => new PrepositionMeta(PrepositionType.BecauseOf)),
        new("ⲓⲥϫⲉⲛ", () => new PrepositionMeta(PrepositionType.Since)),
        new("ⲕⲁⲧⲁ", () => new PrepositionMeta(PrepositionType.AccordingTo)),
        new("ⲗⲟⲓⲡⲟⲛ", () => new PrepositionMeta(PrepositionType.After)),
        new("ⲙⲉⲛϩⲓ", () => new PrepositionMeta(PrepositionType.After)),
        new("ⲙⲏⲣ", () => new PrepositionMeta(PrepositionType.OtherSide)),
        new("ϩⲓⲙⲏⲣ", () => new PrepositionMeta(PrepositionType.OtherSide)),
        new("ⲉⲧⲙⲏϯ", () => new PrepositionMeta(PrepositionType.Between)),
        //new("ⲙⲙⲟⲛ", () => new PrepositionMeta(PrepositionType.X)),
        new("ⲛⲉⲙ", () => new PrepositionMeta(PrepositionType.With)),
        new("ⲡⲗⲏⲛ", () => new PrepositionMeta(PrepositionType.Except)),
        new("ⲙⲉⲛⲉⲛⲥⲁ", () => new PrepositionMeta(PrepositionType.After)),
        new("ⲉⲥⲕⲉⲛ", () => new PrepositionMeta(PrepositionType.Beside)),
        new("ⲛⲧⲉⲛ", () => new PrepositionMeta(PrepositionType.BecauseOf)),
        new("ϧⲁⲧⲉⲛ", () => new PrepositionMeta(PrepositionType.Under)),
        new("ⲟⲩⲃⲉ", () => new PrepositionMeta(PrepositionType.Contrasting)),
        new("ⲟⲩⲧⲉ", () => new PrepositionMeta(PrepositionType.Between)),
        //new("ⲛⲟⲩⲉϣⲉⲛ", () => new PrepositionMeta(PrepositionType.X)),
        new("ⲉⲫⲁϩⲟⲩ", () => new PrepositionMeta(PrepositionType.Forward, true)),
        new("ⲥⲁⲫⲁϩⲟⲩ", () => new PrepositionMeta(PrepositionType.Behind)),
        new("ϩⲓⲫⲁϩⲟⲩ", () => new PrepositionMeta(PrepositionType.Behind)),
        new("ⲭⲱⲣⲓⲥ", () => new PrepositionMeta(PrepositionType.With, true)),
        new("ϣⲁ", () => new PrepositionMeta(PrepositionType.To)),
        new("ϧⲁ", () => new PrepositionMeta(PrepositionType.In)),
        //new("ϣⲁⲧⲉⲛ", () => new PrepositionMeta(PrepositionType.To)),
        new("ϧⲉⲛ", () => new PrepositionMeta(PrepositionType.In)),
        new("ϩⲁ", () => new PrepositionMeta(PrepositionType.To)),
        new("ⲉⲑⲏ", () => new PrepositionMeta(PrepositionType.Forward)),
        new("ⲥⲁⲧϩⲏ", () => new PrepositionMeta(PrepositionType.After, true)),
        new("ϩⲁⲑⲏ", () => new PrepositionMeta(PrepositionType.Behind, true)),
        new("ϩⲓⲑⲏ", () => new PrepositionMeta(PrepositionType.Forward)),
        new("ϩⲓ", () => new PrepositionMeta(PrepositionType.Concerning)),
        new("ⲉϩⲣⲉⲛ", () => new PrepositionMeta(PrepositionType.Facing)),
        new("ⲛⲁϩⲣⲉⲛ", () => new PrepositionMeta(PrepositionType.PresenceOf)),
        new("ⲉϧⲟⲩⲛ", () => new PrepositionMeta(PrepositionType.To)),
        new("ϩⲟⲧⲉ", () => new PrepositionMeta(PrepositionType.Beyond)),
        new("ⲉϩⲣⲏⲓ", () => new PrepositionMeta(PrepositionType.Above)),
        new("ⲉϫⲉⲛ", () => new PrepositionMeta(PrepositionType.On)),
        new("ϧⲁϫⲉⲛ", () => new PrepositionMeta(PrepositionType.After, true)),
        new("ϩⲓϫⲉⲛ", () => new PrepositionMeta(PrepositionType.On)),
    ];

    public IEnumerable<SemanticPair> IndependentPersonalPronouns { get; } =
    [
        new("ⲁⲛⲟⲕ", () => new NounMeta(new ConceptReference("", "/c/en/pronoun"), new(Gender.Unspecified, GrammaticalCount.Singular, PointOfView.First))),
        new("ⲛⲑⲟⲕ", () => new NounMeta(new ConceptReference("", "/c/en/pronoun"), new(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Second))),
        new("ⲛⲑⲟ", () => new NounMeta(new ConceptReference("", "/c/en/pronoun"), new(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Second))),
        new("ⲛⲑⲟϥ", () => new NounMeta(new ConceptReference("", "/c/en/pronoun"), new(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Third))),
        new("ⲛⲑⲟⲥ", () => new NounMeta(new ConceptReference("", "/c/en/pronoun"), new(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Third))),
        new("ⲁⲛⲟⲛ", () => new NounMeta(new ConceptReference("", "/c/en/pronoun"), new(Gender.Unspecified, GrammaticalCount.Plural, PointOfView.First))),
        new("ⲛⲑⲱⲧⲉⲛ", () => new NounMeta(new ConceptReference("", "/c/en/pronoun"), new(Gender.Unspecified, GrammaticalCount.Plural, PointOfView.Second))),
        new("ⲛⲑⲱⲟⲩ", () => new NounMeta(new ConceptReference("", "/c/en/pronoun"), new(Gender.Unspecified, GrammaticalCount.Plural, PointOfView.Third))),
    ];

    public IEnumerable<SemanticPair> EmphaticPronouns { get; } =
    [
        new("ϩⲱ", () => new NounMeta(new ConceptReference("", "/c/en/also"), new(Gender.Unspecified, GrammaticalCount.Singular, PointOfView.First))),
        new("ϩⲱⲕ", () => new NounMeta(new ConceptReference("", "/c/en/also"), new(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Second))),
        new("ϩⲱⲓ", () => new NounMeta(new ConceptReference("", "/c/en/also"), new(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Second))),
        new("ϩⲱϥ", () => new NounMeta(new ConceptReference("", "/c/en/also"), new(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Third))),
        new("ϩⲱⲥ", () => new NounMeta(new ConceptReference("", "/c/en/also"), new(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Third))),
        new("ϩⲱⲛ", () => new NounMeta(new ConceptReference("", "/c/en/also"), new(Gender.Unspecified, GrammaticalCount.Plural, PointOfView.First))),
        new("ϩⲱⲧⲉⲛ", () => new NounMeta(new ConceptReference("", "/c/en/also"), new(Gender.Unspecified, GrammaticalCount.Plural, PointOfView.Second))),
        new("ϩⲱⲟⲩ", () => new NounMeta(new ConceptReference("", "/c/en/also"), new(Gender.Unspecified, GrammaticalCount.Plural, PointOfView.Third))),
    ];
}
