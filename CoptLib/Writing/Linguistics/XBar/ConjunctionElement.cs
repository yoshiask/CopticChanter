using System;

namespace CoptLib.Writing.Linguistics.XBar;

public record ConjunctionElement(Range SourceRange, ConjunctionMeta Meta) : StructuralElement(SourceRange)
{
    public override string ToString() => $"Conjunction{{{SourceRange}, {Meta}}}";
}

public record ConjunctionMeta(ConjunctionType Type, bool Negative = false, InflectionMeta? Inflection = null) : IMeta
{
    public override string ToString()
    {
        var str = $"CONJ{(Negative ? '-' : '+')}{Type}";

        if (Inflection is not null)
            str += $"/{Inflection}";
        
        return str;
    }
}

public enum ConjunctionType
{
    Unspecified,
    And,
    Or,
    ExclusiveOr,
}

public static class ConjunctionElementExtensions
{
    public static string ToAbbreviation(this ConjunctionType type)
    {
        return type switch
        {
            ConjunctionType.And => "AND",
            ConjunctionType.Or => "OR",
            ConjunctionType.ExclusiveOr => "XOR",
            _ => type.ToString()
        };
    }
}