using CoptLib.Writing.Lexicon;

namespace CoptLib.Writing.Linguistics.XBar;

public record DeterminerElement(Range SourceRange, IDeterminerMeta? Meta) : StructuralElement(SourceRange)
{
    public override string ToString() => $"Determiner{{{SourceRange}, {Meta}}}";
}

public interface IDeterminerMeta : IMeta;

public record DeterminerArticleMeta(DeterminerStrength Strength, bool Definite, InflectionMeta Target) : IDeterminerMeta
{
    public override string ToString() => $"DET{{{Strength} {(Definite ? "Def" : "Indef")} Article, {Target}}}";
}

public record DeterminerPossessiveMeta(DeterminerStrength Strength, InflectionMeta Possessor, InflectionMeta Possessed) : IDeterminerMeta
{
    public override string ToString() => $"DET{{{Strength} Poss Article, {Possessor}, {Possessed}}}";
}

// TODO: Support different systems of deixis
public record DeterminerDemonstrativeMeta(DeterminerStrength Strength, InflectionMeta Target) : IDeterminerMeta;

public record DeterminerQuantifyingMeta(InflectionMeta Target) : IDeterminerMeta;

// TODO: Add meta for Distributive, Interrogative, and Relative determiners

public record InflectionMeta(Gender Gender = default, GrammaticalCount Number = default, PointOfView PointOfView = default) : IMeta
{
    public static readonly InflectionMeta Unspecified = new();

    public override string ToString()
    {
        var g = Gender.ToAbbreviation();
        var n = Number.ToAbbreviation();
        var p = PointOfView.ToAbbreviation();
        return $"{g}.{n}.{p}";
    }
}

public enum DeterminerStrength : byte
{
    Unspecified, Weak, Strong,

    Far = Weak,
    Near = Strong,
}

/// <summary>
/// Represents the typical three-degree personal deixis.
/// </summary>
public enum PointOfView : byte
{
    Unspecified, First, Second, Third
}

public enum GrammaticalCount : int
{
    Unspecified = int.MinValue,
    Other = -1,
    None = 0,
    Singular = 1,
    Dual = 2,
    Trial = 3,
    Quadral = 4,
    Pacual = 10,
    Many = 20,
    Plural = int.MaxValue - 30,
    GreaterPlural = int.MaxValue - 20,
    GreatestPlural = int.MaxValue - 10,
    All = int.MaxValue,
}

public static class DeterminerElementExtensions
{
    public static string ToAbbreviation(this PointOfView pointOfView)
    {
        return pointOfView switch
        {
            PointOfView.First => "1st",
            PointOfView.Second => "2nd",
            PointOfView.Third => "3rd",
            _ => "*",
        };
    }
}
