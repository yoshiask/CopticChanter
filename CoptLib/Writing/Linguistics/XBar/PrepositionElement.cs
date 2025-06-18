namespace CoptLib.Writing.Linguistics.XBar;

public record PrepositionElement(Range SourceRange, PrepositionMeta Meta) : StructuralElement(SourceRange)
{
    public override string ToString() => $"Preposition{{{SourceRange}, {Meta}}}";
}

public record PrepositionMeta(PrepositionType Type, bool Negative = false, InflectionMeta? Inflection = null) : IMeta
{
    public override string ToString()
    {
        var str = $"PREP{(Negative ? '-' : '+')}{Type}";

        if (Inflection is not null)
            str += $"/{Inflection}";
        
        return str;
    }
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
