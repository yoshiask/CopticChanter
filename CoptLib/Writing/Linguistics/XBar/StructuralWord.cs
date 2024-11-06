using CoptLib.Writing.Lexicon;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CoptLib.Writing.Linguistics.XBar;

public static class CommonAttributes
{
    public const string LexicalCategory = "LexicalCategory";
    public const string FunctionalCategory = "FunctionalCategory";
    public const string PhrasalCategory = "PhrasalCategory";
}

public interface IStructuralElement
{
    /// <summary>
    /// The range in the source text that this element represents.
    /// </summary>
    Range SourceRange { get; }

    IDictionary<string, object> Attributes { get; }

    bool TryGetAttribute<T>(string key, [NotNullWhen(true)] out T? value);
}

public abstract record StructuralElement(Range SourceRange) : IStructuralElement
{
    public IDictionary<string, object> Attributes { get; } = new Dictionary<string, object>();

    public bool TryGetAttribute<T>(string key, [NotNullWhen(true)] out T? value)
    {
        value = default;
        if (Attributes.TryGetValue(key, out var v))
            value = (T)v;

        return value is not null;
    }

    [return: NotNullIfNotNull(nameof(meta))]
    public static StructuralElement? FromMeta(Range range, IMeta? meta)
    {
        return meta switch
        {
            null => null,
            IDeterminerMeta detMeta => new DeterminerElement(range, detMeta),
            PrepositionMeta prepMeta => new PrepositionElement(range, prepMeta),

            _ => throw new NotImplementedException($"Unrecognized meta type: {meta.GetType().Name}")
        };
    }
}

public record StructuralPhrase(Range SourceRange) : StructuralElement(SourceRange)
{
    public StructuralPhrase(Range sourceRange, PhrasalCategory category) : this(sourceRange)
    {
        Category = category;
    }

    public PhrasalCategory Category
    {
        get
        {
            if (Attributes.TryGetValue(CommonAttributes.PhrasalCategory, out var v) && v is PhrasalCategory cat)
                return cat;
            return default;
        }
        set => Attributes[CommonAttributes.PhrasalCategory] = value;
    }
}

public record EmptyStructuralElement() : StructuralElement(Range.Empty);

public record StructuralLexeme(Range SourceRange, LexiconEntry Entry, int SenseIndex) : StructuralElement(SourceRange)
{
    public Sense Sense => Entry.Senses[SenseIndex];

    public override string ToString() => $"Lexeme{{{SourceRange}, {Entry.Id} {Entry.Forms[0].Orthography}, {SenseIndex}}}";
}

public record StructuralWord(Range SourceRange, IList<StructuralLexeme> Lexemes) : StructuralElement(SourceRange)
{
    public StructuralWord(Range sourceRange) : this(sourceRange, [])
    {
    }
}
