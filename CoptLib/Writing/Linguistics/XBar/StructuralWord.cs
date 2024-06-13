using CoptLib.Writing.Lexicon;
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
    string SourceText { get; }

    IDictionary<string, object> Attributes { get; }

    bool TryGetAttribute<T>(string key, [NotNullWhen(true)] out T? value);
}

public abstract record StructuralElement(string SourceText) : IStructuralElement
{
    public IDictionary<string, object> Attributes { get; } = new Dictionary<string, object>();

    public bool TryGetAttribute<T>(string key, [NotNullWhen(true)] out T? value)
    {
        value = default;
        if (Attributes.TryGetValue(key, out var v))
            value = (T)v;

        return value is not null;
    }
}

public record StructuralPhrase(string SourceText) : StructuralElement(SourceText)
{
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

public record EmptyStructuralElement() : StructuralElement(string.Empty);

public record StructuralLexeme(string SourceText, LexiconEntry Entry, int SenseIndex) : StructuralElement(SourceText)
{
    public Sense Sense => Entry.Senses[SenseIndex];
}

public record StructuralWord(string SourceText, IList<StructuralLexeme> Lexemes) : StructuralElement(SourceText)
{
    public StructuralWord(string sourceText) : this(sourceText, [])
    {
    }
}

public record StructuralSentence(string SourceText, IList<StructuralWord> Words) : StructuralElement(SourceText)
{
    public StructuralSentence(string sourceText) : this(sourceText, [])
    {
    }
}
