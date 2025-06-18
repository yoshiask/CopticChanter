#nullable enable
using CoptLib.Writing.Linguistics.XBar;
using System.Collections.Generic;

namespace CoptLib.Writing.Lexicon;

public record GrammarGroup(PartOfSpeech PartOfSpeech, Number Number, Gender Gender,
    List<GrammarEntry> Entries, PartOfSpeechSubclass Subclass, string? Note)
{
    public static GrammarGroup Default { get; } =
        new(PartOfSpeech.Unknown, Number.None, Gender.Unspecified, [], PartOfSpeechSubclass.Unknown, null);
}

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

public enum PartOfSpeechSubclass : byte
{
    Unknown,
    Copula,
    Composite,
    Genitive,
    Interrogative,
    Nominal,
    Pronominal,
    
    ConverterFocalization,
    ConverterRelative,
    ParticleNegation,
    Pronoun1stPerson,
    Pronoun2ndPerson,
    Pronoun3rdPerson,
    PronounIndefinite,
    SubstantiveDeityName,
    SubstantivePlaceName,
    SubstantiveThingOrInstitutionName,
    SubstantiveTitle,
    VerbAdjective,
    VerbAuxiliary,
    VerbSuffixConjugation,
    VerbInfinitive,
    VerbImperative,
    VerbQualitative,
    VerbStative,
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

    public static string ToAbbreviation(this Gender gender)
    {
        return gender switch
        {
            Gender.Neutral => "NEUT",
            Gender.Masculine => "MASC",
            Gender.Feminine => "FEM",
            Gender.Animate => "ANI",
            Gender.Inanimate => "INAN",
            _ => "*"
        };
    }

    public static string ToAbbreviation(this GrammaticalCount count)
    {
        return count switch
        {
            GrammaticalCount.Singular => "SG",
            GrammaticalCount.Dual => "2",
            GrammaticalCount.Trial => "3",
            GrammaticalCount.Quadral => "4",
            GrammaticalCount.Pacual => "5",
            GrammaticalCount.Plural => "PL",
            GrammaticalCount.GreaterPlural => "PL+",
            GrammaticalCount.GreatestPlural => "PL++",
            GrammaticalCount.Unspecified => "*",
            _ => count.ToString()
        };
    }
}
