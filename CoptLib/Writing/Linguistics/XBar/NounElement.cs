using CoptLib.Writing.Lexicon;

namespace CoptLib.Writing.Linguistics.XBar;

public record NounElement(Range SourceRange, NounMeta Meta) : StructuralElement(SourceRange)
{
    public override string ToString() => $"Noun{{{SourceRange}, {Meta}}}";
}

public record NounMeta(ILexemeReference Meaning, InflectionMeta Inflection) : IMeta
{
    public override string ToString() => $"NOUN{{{Meaning}, {Inflection}}}";
}

public interface ILexemeReference
{
    string Orthography { get; }
}

// TODO: What system will be used to map words to meanings?
public record ConceptReference(string Orthography, string ConceptNetId) : ILexemeReference;

public record LexiconEntryReference(LexiconEntry Entry) : ILexemeReference
{
    public string Orthography => Entry.Forms[0].Orthography;
}
