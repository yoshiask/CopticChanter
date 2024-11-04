namespace CoptLib.Writing.Linguistics.XBar;

public record PrepositionElement(Range SourceRange, PrepositionMeta Meta) : StructuralElement(SourceRange)
{
    public override string ToString() => $"Preposition{{{SourceRange}, {Meta}}}";
}

public record PrepositionMeta(PrepositionType Type, bool Negative = false) : IMeta
{
    public override string ToString() => $"PREP{(Negative ? '-' : '+')}{Type}";
}

public enum PrepositionType
{
    Unspecified,
    To,
    From,
    Above,
    Between,
    Beside,
    With,
    In,
    On,
    Under,
    Away,
    Ago,
    Along,
    Through,
    Since,
    For,
    Of,
    BecauseOf,
    AccordingTo,
    After,
    OtherSide,
    Except,
    Contrasting,
    Forward,
    Behind,
    Concerning,
    Facing,
    PresenceOf,
    Beyond,
}
