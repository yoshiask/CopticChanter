using CoptLib.Writing.Lexicon;
using CoptLib.Writing.Linguistics.XBar;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CoptLib.Writing.Linguistics.Analyzers;

public partial class CopticBohairicGrammar
{
    const string VILMINOR = "ⲃⲓⲗⲙⲛⲟⲣⲫⲯ";
    const string VOWELS = "ⲱⲉⲏⲩⲓⲟⲁ";

    const string VILMINOR_REGEX = $"[{VILMINOR}]";
    const string NOT_VILMINOR_REGEX = $"[^{VILMINOR}]";
    const string VOWELS_REGEX = $"[{VOWELS}]";
    const string NOT_VOWELS_REGEX = $"[^{VOWELS}]";

    private IEnumerable<SemanticPair>? _determiners;
    private IEnumerable<SemanticPair>? _nounPrefixes;
    private IEnumerable<SemanticPair>? _pronouns;

    public SemanticPair GenericNominalizer { get; } =
        new("(ⲙⲉ(?:ⲧ|ⲑ))", _ => new NominalizingMeta(NominalizingType.Unspecified, NominalizingType.Noun));

    public SemanticPair AgentNounConverter { get; } =
        new("ⲣⲉϥ", _ => new NominalizingMeta(NominalizingType.Verb, NominalizingType.Agent));

    public SemanticPair Denominalizer { get; } =
        new("ⲉⲣ", _ => new NominalizingMeta(NominalizingType.Noun | NominalizingType.Adjective, NominalizingType.Verb));

    public IEnumerable<SemanticPair> Articles { get; } =
    [
        // Definite
        new(new Regex($"(ⲡ){NOT_VILMINOR_REGEX}"), _ => new DeterminerArticleMeta(DeterminerStrength.Weak, true, new(Gender.Masculine, GrammaticalCount.Singular))),
        new(new Regex($"(ⲫ){VILMINOR_REGEX}"), _ => new DeterminerArticleMeta(DeterminerStrength.Weak, true, new(Gender.Masculine, GrammaticalCount.Singular))),
        new("ⲡⲓ", _ => new DeterminerArticleMeta(DeterminerStrength.Strong, true, new(Gender.Masculine, GrammaticalCount.Singular))),
        new(new Regex($"(ⲧ){NOT_VILMINOR_REGEX}"), _ => new DeterminerArticleMeta(DeterminerStrength.Weak, true, new(Gender.Feminine, GrammaticalCount.Singular))),
        new(new Regex($"(ⲑ){VILMINOR_REGEX}"), _ => new DeterminerArticleMeta(DeterminerStrength.Weak, true, new(Gender.Feminine, GrammaticalCount.Singular))),
        new("ϯ", _ => new DeterminerArticleMeta(DeterminerStrength.Strong, true, new(Gender.Feminine, GrammaticalCount.Singular))),
        new("ⲛⲓ", _ => new DeterminerArticleMeta(DeterminerStrength.Strong, true, new(Number: GrammaticalCount.Plural))),

        // Indefinite
        new("ⲟⲩ", _ => new DeterminerArticleMeta(default, false, new(Number: GrammaticalCount.Singular))),
        new("ϩⲁⲛ", _ => new DeterminerArticleMeta(default, false, new(Number: GrammaticalCount.Plural))),

        // Possessive Strong
        new(new Regex($"(ⲛ){NOT_VILMINOR_REGEX}"), _ => new DeterminerPossessiveMeta(DeterminerStrength.Strong, InflectionMeta.Unspecified, InflectionMeta.Unspecified)),
        new(new Regex($"(ⲙ){VILMINOR_REGEX}"), _ => new DeterminerPossessiveMeta(DeterminerStrength.Strong, InflectionMeta.Unspecified, InflectionMeta.Unspecified)),

        // Possessive 1st Person
        new("ⲡⲁ", _ => new DeterminerPossessiveMeta(default, new(default, GrammaticalCount.Singular, PointOfView.First), new(Gender.Masculine, GrammaticalCount.Singular))),
        new("ⲧⲁ", _ => new DeterminerPossessiveMeta(default, new(default, GrammaticalCount.Singular, PointOfView.First), new(Gender.Feminine, GrammaticalCount.Singular))),
        new("ⲛⲁ", _ => new DeterminerPossessiveMeta(default, new(default, GrammaticalCount.Singular, PointOfView.First), new(Number: GrammaticalCount.Plural))),
        new("ⲡⲉⲛ", _ => new DeterminerPossessiveMeta(default, new(default, GrammaticalCount.Plural, PointOfView.First), new(Gender.Masculine, GrammaticalCount.Singular))),
        new("ⲧⲉⲛ", _ => new DeterminerPossessiveMeta(default, new(default, GrammaticalCount.Plural, PointOfView.First), new(Gender.Feminine, GrammaticalCount.Singular))),
        new("ⲛⲉⲛ", _ => new DeterminerPossessiveMeta(default, new(default, GrammaticalCount.Plural, PointOfView.First), new(Number: GrammaticalCount.Plural))),

        // Possessive 2nd Person
        new("ⲡⲉⲕ", _ => new DeterminerPossessiveMeta(default, new(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Second), new(Gender.Masculine, GrammaticalCount.Singular))),
        new("ⲧⲉⲕ", _ => new DeterminerPossessiveMeta(default, new(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Second), new(Gender.Feminine, GrammaticalCount.Singular))),
        new("ⲛⲉⲕ", _ => new DeterminerPossessiveMeta(default, new(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Second), new(Number: GrammaticalCount.Plural))),
        new("ⲡⲉ", _ => new DeterminerPossessiveMeta(default, new(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Second), new(Gender.Masculine, GrammaticalCount.Singular))),
        new("ⲧⲉ", _ => new DeterminerPossessiveMeta(default, new(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Second), new(Gender.Feminine, GrammaticalCount.Singular))),
        new("ⲛⲉ", _ => new DeterminerPossessiveMeta(default, new(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Second), new(Number: GrammaticalCount.Plural))),
        new("ⲡⲉⲧⲉⲛ", _ => new DeterminerPossessiveMeta(default, new(default, GrammaticalCount.Plural, PointOfView.Second), new(Gender.Masculine, GrammaticalCount.Singular))),
        new("ⲧⲉⲧⲉⲛ", _ => new DeterminerPossessiveMeta(default, new(default, GrammaticalCount.Plural, PointOfView.Second), new(Gender.Feminine, GrammaticalCount.Singular))),
        new("ⲛⲉⲧⲉⲛ", _ => new DeterminerPossessiveMeta(default, new(default, GrammaticalCount.Plural, PointOfView.Second), new(Number: GrammaticalCount.Plural))),

        // Possessive 3rd Person
        new("ⲡⲉϥ", _ => new DeterminerPossessiveMeta(default, new(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Third), new(Gender.Masculine, GrammaticalCount.Singular))),
        new("ⲧⲉϥ", _ => new DeterminerPossessiveMeta(default, new(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Third), new(Gender.Feminine, GrammaticalCount.Singular))),
        new("ⲛⲉϥ", _ => new DeterminerPossessiveMeta(default, new(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Third), new(Number: GrammaticalCount.Plural))),
        new("ⲡⲉⲥ", _ => new DeterminerPossessiveMeta(default, new(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Third), new(Gender.Masculine, GrammaticalCount.Singular))),
        new("ⲧⲉⲥ", _ => new DeterminerPossessiveMeta(default, new(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Third), new(Gender.Feminine, GrammaticalCount.Singular))),
        new("ⲛⲉⲥ", _ => new DeterminerPossessiveMeta(default, new(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Third), new(Number: GrammaticalCount.Plural))),
        new("ⲡⲟⲩ", _ => new DeterminerPossessiveMeta(default, new(default, GrammaticalCount.Plural, PointOfView.Third), new(Gender.Masculine, GrammaticalCount.Singular))),
        new("ⲧⲟⲩ", _ => new DeterminerPossessiveMeta(default, new(default, GrammaticalCount.Plural, PointOfView.Third), new(Gender.Feminine, GrammaticalCount.Singular))),
        new("ⲛⲟⲩ", _ => new DeterminerPossessiveMeta(default, new(default, GrammaticalCount.Plural, PointOfView.Third), new(Number: GrammaticalCount.Plural))),

        // Demonstrative
        new("ⲡⲁⲓ", _ => new DeterminerDemonstrativeMeta(DeterminerStrength.Near, new(Gender.Masculine, GrammaticalCount.Singular))),
        new("ⲧⲁⲓ", _ => new DeterminerDemonstrativeMeta(DeterminerStrength.Near, new(Gender.Feminine, GrammaticalCount.Singular))),
        new("ⲛⲁⲓ", _ => new DeterminerDemonstrativeMeta(DeterminerStrength.Near, new(Number: GrammaticalCount.Plural))),
    ];

    public IEnumerable<SemanticPair> ComplexNounPrefixes { get; } =
    [
        // Example: "ⲡⲓⲙⲁⲛϣⲉⲗⲉⲧ" / "the bridal chamber" ("the place of [the] bride")
        new("ⲙⲁ", _ => new LexemeMeta(new ConceptReference("place", "/c/en/place"), InflectionMeta.Unspecified)),

        // Example: "ϯⲙⲉⲧⲣⲉⲙⲛ̀ⲭⲏⲙⲓ" / "the Coptic language" ("[the language of] person of Egypt")
        new("ⲣⲉⲙ", _ => new LexemeMeta(new ConceptReference("person", "/c/en/person"), InflectionMeta.Unspecified)),
    ];

    public IEnumerable<SemanticPair> Determiners
    {
        get
        {
            return _determiners ??= [
                new("ⲉⲧⲉⲙⲙⲁⲩ", _ => new DeterminerDemonstrativeMeta(DeterminerStrength.Far, InflectionMeta.Unspecified)),
                new("ⲛⲓⲃⲉⲛ", _ => new DeterminerQuantifyingMeta(new(Number: GrammaticalCount.All))),
                .. Articles
            ];
        }
    }

    public IEnumerable<SemanticPair> NounPrefixes
    {
        get
        {
            return _nounPrefixes ??= [
                new("ⲉ", _ => new PrepositionMeta(PrepositionType.To)),
                // TODO: "ⲉⲟⲩ" and its contraction "ⲉⲩ"

                new(new Regex($"(ⲛ){NOT_VILMINOR_REGEX}"), _ => new PrepositionMeta(PrepositionType.Of)),
                new(new Regex($"(ⲙ){VILMINOR_REGEX}"), _ => new PrepositionMeta(PrepositionType.Of)),

                .. ComplexNounPrefixes,
                .. Articles,
            ];
        }
    }

    public IEnumerable<SemanticPair> Prepositions { get; } =
    [
        new("ⲛⲧⲉ", _ => new PrepositionMeta(PrepositionType.Of)),
        new("ϩⲓⲧⲉⲛ", _ => new PrepositionMeta(PrepositionType.Through)),

        new("ⲁⲧϭⲛⲉ", _ => new PrepositionMeta(PrepositionType.With, true)),
        new("ⲥⲁⲃⲟⲗ", _ => new PrepositionMeta(PrepositionType.Away)),
        new("ϣⲁⲉⲃⲟⲗ", _ => new PrepositionMeta(PrepositionType.In, true)),
        new("ⲉⲑⲃⲉ", _ => new PrepositionMeta(PrepositionType.BecauseOf)),
        new("ⲓⲥϫⲉⲛ", _ => new PrepositionMeta(PrepositionType.Since)),
        new("ⲕⲁⲧⲁ", _ => new PrepositionMeta(PrepositionType.AccordingTo)),
        new("ⲗⲟⲓⲡⲟⲛ", _ => new PrepositionMeta(PrepositionType.After)),
        new("ⲙⲉⲛϩⲓ", _ => new PrepositionMeta(PrepositionType.After)),
        new("ⲙⲏⲣ", _ => new PrepositionMeta(PrepositionType.OtherSide)),
        new("ϩⲓⲙⲏⲣ", _ => new PrepositionMeta(PrepositionType.OtherSide)),
        new("ⲉⲧⲙⲏϯ", _ => new PrepositionMeta(PrepositionType.Between)),
        //new("ⲙⲙⲟⲛ", _ => new PrepositionMeta(PrepositionType.X)),
        new("ⲛⲉⲙ", _ => new PrepositionMeta(PrepositionType.With)),
        new("ⲡⲗⲏⲛ", _ => new PrepositionMeta(PrepositionType.Except)),
        new("ⲙⲉⲛⲉⲛⲥⲁ", _ => new PrepositionMeta(PrepositionType.After)),
        new("ⲉⲥⲕⲉⲛ", _ => new PrepositionMeta(PrepositionType.Beside)),
        new("ⲛⲧⲉⲛ", _ => new PrepositionMeta(PrepositionType.BecauseOf)),
        new("ϧⲁⲧⲉⲛ", _ => new PrepositionMeta(PrepositionType.Under)),
        new("ⲟⲩⲃⲉ", _ => new PrepositionMeta(PrepositionType.Contrasting)),
        new("ⲟⲩⲧⲉ", _ => new PrepositionMeta(PrepositionType.Between)),
        //new("ⲛⲟⲩⲉϣⲉⲛ", _ => new PrepositionMeta(PrepositionType.X)),
        new("ⲉⲫⲁϩⲟⲩ", _ => new PrepositionMeta(PrepositionType.Forward, true)),
        new("ⲥⲁⲫⲁϩⲟⲩ", _ => new PrepositionMeta(PrepositionType.Behind)),
        new("ϩⲓⲫⲁϩⲟⲩ", _ => new PrepositionMeta(PrepositionType.Behind)),
        new("ⲭⲱⲣⲓⲥ", _ => new PrepositionMeta(PrepositionType.With, true)),
        new("ϣⲁ", _ => new PrepositionMeta(PrepositionType.To)),
        new("ϧⲁ", _ => new PrepositionMeta(PrepositionType.In)),
        //new("ϣⲁⲧⲉⲛ", _ => new PrepositionMeta(PrepositionType.To)),
        new("ϧⲉⲛ", _ => new PrepositionMeta(PrepositionType.In)),
        new("ϩⲁ", _ => new PrepositionMeta(PrepositionType.To)),
        new("ⲉⲑⲏ", _ => new PrepositionMeta(PrepositionType.Forward)),
        new("ⲥⲁⲧϩⲏ", _ => new PrepositionMeta(PrepositionType.After, true)),
        new("ϩⲁⲑⲏ", _ => new PrepositionMeta(PrepositionType.Behind, true)),
        new("ϩⲓⲑⲏ", _ => new PrepositionMeta(PrepositionType.Forward)),
        new("ϩⲓ", _ => new PrepositionMeta(PrepositionType.Concerning)),
        new("ⲉϩⲣⲉⲛ", _ => new PrepositionMeta(PrepositionType.Facing)),
        new("ⲛⲁϩⲣⲉⲛ", _ => new PrepositionMeta(PrepositionType.PresenceOf)),
        new("ⲉϧⲟⲩⲛ", _ => new PrepositionMeta(PrepositionType.To)),
        new("ϩⲟⲧⲉ", _ => new PrepositionMeta(PrepositionType.Beyond)),
        new("ⲉϩⲣⲏⲓ", _ => new PrepositionMeta(PrepositionType.Above)),
        new("ⲉϫⲉⲛ", _ => new PrepositionMeta(PrepositionType.On)),
        new("ϧⲁϫⲉⲛ", _ => new PrepositionMeta(PrepositionType.After, true)),
        new("ϩⲓϫⲉⲛ", _ => new PrepositionMeta(PrepositionType.On)),
    ];

    public IEnumerable<SemanticPair> IndependentPersonalPronouns { get; } =
    [
        new("ⲁⲛⲟⲕ", _ => new LexemeMeta(new ConceptReference("I", "/c/en/pronoun"), new(Gender.Unspecified, GrammaticalCount.Singular, PointOfView.First))),
        new("ⲛⲑⲟⲕ", _ => new LexemeMeta(new ConceptReference("you", "/c/en/pronoun"), new(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Second))),
        new("ⲛⲑⲟ", _ => new LexemeMeta(new ConceptReference("you", "/c/en/pronoun"), new(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Second))),
        new("ⲛⲑⲟϥ", _ => new LexemeMeta(new ConceptReference("he", "/c/en/pronoun"), new(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Third))),
        new("ⲛⲑⲟⲥ", _ => new LexemeMeta(new ConceptReference("she", "/c/en/pronoun"), new(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Third))),
        new("ⲁⲛⲟⲛ", _ => new LexemeMeta(new ConceptReference("we", "/c/en/pronoun"), new(Gender.Unspecified, GrammaticalCount.Plural, PointOfView.First))),
        new("ⲛⲑⲱⲧⲉⲛ", _ => new LexemeMeta(new ConceptReference("y'all", "/c/en/pronoun"), new(Gender.Unspecified, GrammaticalCount.Plural, PointOfView.Second))),
        new("ⲛⲑⲱⲟⲩ", _ => new LexemeMeta(new ConceptReference("they", "/c/en/pronoun"), new(Gender.Unspecified, GrammaticalCount.Plural, PointOfView.Third))),
    ];

    public IEnumerable<SemanticPair> EmphaticPronouns { get; } =
    [
        new("ϩⲱ", _ => new LexemeMeta(new ConceptReference("also", "/c/en/also"), new(Gender.Unspecified, GrammaticalCount.Singular, PointOfView.First))),
        new("ϩⲱⲕ", _ => new LexemeMeta(new ConceptReference("also", "/c/en/also"), new(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Second))),
        new("ϩⲱⲓ", _ => new LexemeMeta(new ConceptReference("also", "/c/en/also"), new(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Second))),
        new("ϩⲱϥ", _ => new LexemeMeta(new ConceptReference("also", "/c/en/also"), new(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Third))),
        new("ϩⲱⲥ", _ => new LexemeMeta(new ConceptReference("also", "/c/en/also"), new(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Third))),
        new("ϩⲱⲛ", _ => new LexemeMeta(new ConceptReference("also", "/c/en/also"), new(Gender.Unspecified, GrammaticalCount.Plural, PointOfView.First))),
        new("ϩⲱⲧⲉⲛ", _ => new LexemeMeta(new ConceptReference("also", "/c/en/also"), new(Gender.Unspecified, GrammaticalCount.Plural, PointOfView.Second))),
        new("ϩⲱⲟⲩ", _ => new LexemeMeta(new ConceptReference("also", "/c/en/also"), new(Gender.Unspecified, GrammaticalCount.Plural, PointOfView.Third))),
    ];

    public IEnumerable<SemanticPair> Pronouns
    { 
        get
        {
            return _pronouns ??= [
                .. IndependentPersonalPronouns,
                .. EmphaticPronouns,
            ];
        }
    }
}
