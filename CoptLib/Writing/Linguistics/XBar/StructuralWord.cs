using CoptLib.Writing.Lexicon;
using System.Collections.Generic;

namespace CoptLib.Writing.Linguistics.XBar;

public interface IStructuralElement
{
    string SourceText { get; }

    IDictionary<string, object> Attributes { get; }
}

public abstract record StructuralElement(string SourceText) : IStructuralElement
{
    public IDictionary<string, object> Attributes { get; } = new Dictionary<string, object>();
}

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
