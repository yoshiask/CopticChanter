using CoptLib.Writing.Lexicon;
using System.Collections.Generic;

namespace CoptLib.Writing.Linguistics.Structure;

public interface IStructuralElement
{
    string SourceText { get; }
}

public record StructuralLexeme(string SourceText, LexiconEntry Entry, int SenseIndex) : IStructuralElement
{
    public Sense Sense => Entry.Senses[SenseIndex];
}

public class StructuralWord(string sourceText, IEnumerable<StructuralLexeme> lexemes) : List<StructuralLexeme>(lexemes), IStructuralElement
{
    public StructuralWord(string sourceText) : this(sourceText, [])
    {
    }

    public string SourceText { get; } = sourceText;
}

public class StructuralSentence(string sourceText, IEnumerable<StructuralWord> words) : List<StructuralWord>(words), IStructuralElement
{
    public StructuralSentence(string sourceText) : this(sourceText, [])
    {
    }

    public string SourceText { get; } = sourceText;
}
