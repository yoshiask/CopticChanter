using CoptLib.Writing.Lexicon;

namespace CoptLib.Writing.Linguistics.XBar;

public record DeterminerElement(Range SourceRange, IDeterminerMeta? Meta) : StructuralElement(SourceRange);

public interface IDeterminerMeta : IMeta;

public record DeterminerArticleMeta(DeterminerStrength Strength, bool Definite, InflectionMeta Target) : IDeterminerMeta;
public record DeterminerPossessiveMeta(DeterminerStrength Strength, InflectionMeta Possessor, InflectionMeta Possessed) : IDeterminerMeta;

// TODO: Support different systems of deixis
public record DeterminerDemonstrativeMeta(DeterminerStrength Strength, InflectionMeta Target) : IDeterminerMeta;

public record DeterminerQuantifyingMeta(InflectionMeta Target) : IDeterminerMeta;

// TODO: Add meta for Distributive, Interrogative, and Relative determiners

public record NounMeta(ConceptReference Meaning, InflectionMeta Inflection) : IMeta
{
    public static readonly NounMeta Unspecified = new(default, InflectionMeta.Unspecified);
}

// TODO: What system will be used to map words to meanings?
public struct ConceptReference(string? conceptNetId)
{
    public string? ConceptNetId = conceptNetId;

    public static implicit operator ConceptReference(string id) => new(id);
}

public record InflectionMeta(Gender Gender = default, GrammaticalCount Number = default, PointOfView PointOfView = default) : IMeta
{
    public static readonly InflectionMeta Unspecified = new();
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
