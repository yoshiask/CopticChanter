namespace CoptLib.Writing.Linguistics.XBar;

public record PrepositionElement(Range SourceRange, PrepositionMeta Meta) : StructuralElement(SourceRange);

public record PrepositionMeta(PrepositionType Type, bool Negative = false);

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
