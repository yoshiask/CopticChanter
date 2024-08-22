namespace CoptLib.Writing.Linguistics.XBar;

public record TenseElement(Range SourceRange, TenseMeta Meta, NounMeta Actor, NounMeta? Target = null) : StructuralElement(SourceRange);

public interface ITenseMeta
{
}

public record TenseMeta(RelativeTime Start = default, RelativeTime End = default, TenseFlags Flags = default, int Degree = 0) : ITenseMeta
{
    public static readonly TenseMeta Present = new(RelativeTime.Present, Degree: 1);
    public static readonly TenseMeta Future = new(RelativeTime.Future, Degree: 1);
    public static readonly TenseMeta Past = new(RelativeTime.Past, Degree: 1);
    public static readonly TenseMeta Aorist = new(RelativeTime.Aorist, Degree: 1);
}

[System.Flags]
public enum CompletionGrammaticalAspect : byte
{
    Unspecified     = 0x00,
    Perfective      = 0x10,
    Aorist          = 0x11,
    Momentane       = 0x12,
    Imperfective    = 0x20,
    Continuous      = 0x21,
    Progressive     = 0x22,
    Perfect         = 0x40,
}

public enum RelativeTime : sbyte
{
    Unspecified =  0x00,

    Past        = -0x12,
    DistantPast = -0x11,
    NearPast    = -0x10,
    
    Aorist      =  0x01,
    Present     =  0x02,

    NearFuture  =  0x10,
    FarFuture   =  0x11,
    Future      =  0x12,
}

[System.Flags]
public enum TenseFlags : byte
{
    Unspecified,

    Episodic        = 1 << 0,
    Ending          = 1 << 1,
    Prospective     = 1 << 2,
    Circumstantial  = 1 << 3,
    Perfect         = 1 << 4,
    Relative        = 1 << 5,
    Imperative      = 1 << 6,
}