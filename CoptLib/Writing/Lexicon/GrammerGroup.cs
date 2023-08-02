#nullable enable
using System.Collections.Generic;

namespace CoptLib.Writing.Lexicon;

public record GrammarGroup(PartOfSpeech PartOfSpeech, Number Number, Gender? Gender,
    List<GrammarEntry>? Entries, string? Subclass, string? Note);

public record GrammarEntry(GrammarType Type, string Text);

public enum PartOfSpeech : byte
{
    Subject,    // Subst.
    Verb,       //Vb.
    Adjective,  //Adj.
    Adverb,     //Adv.
    Preposition,//Präp.
    Zahlzeichen,//???
    NominalPrefix,//Nominalpräfix
    PossessiveArticle,//Possessivartikel
}

public enum Number : byte
{
    None,
    Singular,   //sg.
    Plural,     //pl.
}

public enum Gender : byte
{
    Neutral, Masculine, Feminine
}

public enum GrammarType : byte
{
    CollocPrep,
    CollocAdv,
    CollocParticle,
    CollocNoun,
}
