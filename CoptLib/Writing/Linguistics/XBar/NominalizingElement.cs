using System;

namespace CoptLib.Writing.Linguistics.XBar;

public record NominalizingElement(Range SourceRange, NominalizingMeta Meta) : StructuralElement(SourceRange)
{
    public override string ToString() => $"Nominalizing{{{SourceRange}, {Meta}}}";
}

/// <summary>
/// See <see href="https://en.wikipedia.org/wiki/Nominalization"/>
/// </summary>
/// <param name="NominalizationType"></param>
public record NominalizingMeta(NominalizingType SourceTypes, NominalizingType DestinationTypes) : IMeta
{
    public override string ToString() => $"NOM{{{SourceTypes} -> {DestinationTypes}}}";
}

[Flags]
public enum NominalizingType : byte
{
    Unspecified     = 0,
    Adjective       = 1 >> 1,
    Noun            = 1 >> 2,
    Verb            = 1 >> 3,
    Adverb          = 1 >> 4,
    Agent           = 1 >> 5,
}
