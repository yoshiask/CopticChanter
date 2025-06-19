using System.Collections.Generic;
using CoptLib.Writing.Lexicon;
using CoptLib.Writing.Linguistics.XBar;

namespace CoptLib.Writing.Linguistics.Analyzers;

public partial class CopticBohairicGrammar
{
    private IEnumerable<SemanticPair>? _prepositions;

    // "to me/you/them" etc.
    public IEnumerable<SemanticPair> PersonalPrepositions { get; } =
    [
        // Object marker
        ..GenerateSemanticPairForPersonalPreposition(new PrepositionMeta(PrepositionType.With), [
            ("ⲙⲙⲟⲓ", new InflectionMeta(Gender.Unspecified, GrammaticalCount.Singular, PointOfView.First)),
            ("ⲙⲙⲟⲕ", new InflectionMeta(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Second)),
            ("ⲙⲙⲟϥ", new InflectionMeta(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Third)),
            ("ⲙⲙⲟ", new InflectionMeta(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Second)),
            ("ⲙⲙⲟⲥ", new InflectionMeta(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Third)),
            ("ⲙⲙⲟⲛ", new InflectionMeta(Gender.Unspecified, GrammaticalCount.Plural, PointOfView.First)),
            ("ⲙⲙⲱⲧⲉⲛ", new InflectionMeta(Gender.Unspecified, GrammaticalCount.Plural, PointOfView.Second)),
            ("ⲙⲙⲱⲟⲩ", new InflectionMeta(Gender.Unspecified, GrammaticalCount.Plural, PointOfView.Third)),
        ]),
        
        // Dative marker "to, for"
        ..GenerateSemanticPairForPersonalPreposition(new PrepositionMeta(PrepositionType.To), [
            ("ⲛⲏⲓ", new InflectionMeta(Gender.Unspecified, GrammaticalCount.Singular, PointOfView.First)),
            ("ⲛⲁⲕ", new InflectionMeta(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Second)),
            ("ⲛⲁϥ", new InflectionMeta(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Third)),
            ("ⲛⲁ", new InflectionMeta(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Second)),
            ("ⲛⲁⲥ", new InflectionMeta(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Third)),
            ("ⲛⲁⲛ", new InflectionMeta(Gender.Unspecified, GrammaticalCount.Plural, PointOfView.First)),
            ("ⲛⲱⲧⲉⲛ", new InflectionMeta(Gender.Unspecified, GrammaticalCount.Plural, PointOfView.Second)),
            ("ⲛⲱⲧⲟⲩ", new InflectionMeta(Gender.Unspecified, GrammaticalCount.Plural, PointOfView.Third)),
        ]),
        
        ..GenerateSemanticPairForPersonalPreposition(new PrepositionMeta(PrepositionType.To), [
            ("ⲉⲣⲟⲓ", new InflectionMeta(Gender.Unspecified, GrammaticalCount.Singular, PointOfView.First)),
            ("ⲉⲣⲟⲕ", new InflectionMeta(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Second)),
            ("ⲉⲣⲟϥ", new InflectionMeta(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Third)),
            ("ⲉⲣⲟ", new InflectionMeta(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Second)),
            ("ⲉⲣⲟⲥ", new InflectionMeta(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Third)),
            ("ⲉⲣⲟⲛ", new InflectionMeta(Gender.Unspecified, GrammaticalCount.Plural, PointOfView.First)),
            ("ⲉⲣⲱⲧⲉⲛ", new InflectionMeta(Gender.Unspecified, GrammaticalCount.Plural, PointOfView.Second)),
            ("ⲉⲣⲱⲟⲩ", new InflectionMeta(Gender.Unspecified, GrammaticalCount.Plural, PointOfView.Third)),
        ]),
    ];
    
    public IEnumerable<SemanticPair> CommonPrepositions { get; } =
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
        new("ϧⲉⲛ", _ => new PrepositionMeta(PrepositionType.With)),
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
    
    public IEnumerable<SemanticPair> Prepositions
    { 
        get
        {
            return _prepositions ??= [
                .. PersonalPrepositions,
                .. CommonPrepositions,
            ];
        }
    }
    
    private static IEnumerable<SemanticPair> GenerateSemanticPairForPersonalPreposition(PrepositionMeta prep,
        List<(string, InflectionMeta)> inflections)
    {
        foreach (var (pattern, inflection) in inflections)
            yield return new SemanticPair(
                new ExactStringPattern(pattern),
                _ => prep with { Inflection = inflection }
            );
    }
}