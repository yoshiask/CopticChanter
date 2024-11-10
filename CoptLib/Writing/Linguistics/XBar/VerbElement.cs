namespace CoptLib.Writing.Linguistics.XBar;

public record VerbElement(Range SourceRange, VerbMeta Meta) : StructuralElement(SourceRange);

public record VerbMeta(TenseMeta Tense, InflectionMeta Actor, InflectionMeta? Target = null) : IMeta
{
    public bool IsTransitive() => Target is not null;
}
