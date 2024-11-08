#nullable enable
using CoptLib.Writing.Linguistics.XBar;
using System.Collections.Generic;

namespace CoptLib.Writing.Lexicon;

public record GrammarGroup(PartOfSpeech PartOfSpeech, Number Number, Gender Gender,
    List<GrammarEntry>? Entries, string? Subclass, string? Note);

public record GrammarEntry(GrammarType Type, string Text);

public enum PartOfSpeech : byte
{
    Unknown,
    Substantive,
    Verb,
    Adjective,
    Adverb,
    Preposition,
    Pronoun,
    PossessivePronoun,
    InterrogativePronoun,
    DemonstrativePronoun,
    PersonalPronoun,
    NumberSign,
    Numeral,
    Conjugation,
    DefiniteArticle,
    Prefix,
    NominalPrefix,
    VerbalPrefix,
    AdjectivePrefix,
    PronounPrefixPresent1,
    OrdinalPrefix,
    PossessivePrefix,
    PossessiveArticle,
    PronounSuffix,
    Conjunctive,
    Particle,
    SentenceConverter,
    Interjection,
    Composite,
    ImpersonalExpression,
}

public enum Number : byte
{
    None,
    Singular,   //sg.
    Plural,     //pl.
}

public enum Gender : byte
{
    Unspecified, Neutral, Masculine, Feminine, Animate, Inanimate
}

public enum GrammarType : byte
{
    CollocPrep,
    CollocAdv,
    CollocParticle,
    CollocNoun,
    CollocConj,
}

public static class GrammarGroupExtensions
{
    public static GrammaticalCount ToGrammaticalCount(this Number number)
    {
        return number switch
        {
            Number.Singular => GrammaticalCount.Singular,
            Number.Plural => GrammaticalCount.Plural,
            _ => GrammaticalCount.Unspecified,
        };
    }
}
