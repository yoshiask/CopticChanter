using CoptLib.Writing.Lexicon;

namespace CoptLib.Writing.Linguistics.XBar;

public record DeterminerElement(Range SourceRange, IDeterminerMeta? Meta) : StructuralElement(SourceRange);

public interface IDeterminerMeta;

public record DeterminerArticleMeta(DeterminerStrength Strength, bool Definite, NounMeta Target) : IDeterminerMeta;
public record DeterminerPossessiveMeta(DeterminerStrength Strength, NounMeta Possessor, NounMeta Possessed) : IDeterminerMeta;

// TODO: Support different systems of deixis
public record DeterminerDemonstrativeMeta(DeterminerStrength Strength, NounMeta Target) : IDeterminerMeta;

public record DeterminerQuantifyingMeta(NounMeta Target) : IDeterminerMeta;

// TODO: Add meta for Distributive, Interrogative, and Relative determiners

public record NounMeta(Gender Gender = default, GrammaticalCount Number = default, PointOfView PointOfView = default)
{
    public static readonly NounMeta Unspecified = new();
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
