using System.Text;
using CoptLib.Writing.Lexicon;

namespace CoptLib.Writing.Linguistics.XBar;

public record LexemeElement(Range SourceRange, LexemeMeta Meta) : StructuralElement(SourceRange)
{
    public override string ToString() => $"Lexeme{{{SourceRange}, {Meta}}}";
}

public record LexemeMeta(ILexemeReference Meaning, InflectionMeta Inflection) : IMeta
{
    public override string ToString() => $"LEX{{{Meaning}, {Inflection}}}";
}

public interface ILexemeReference
{
    string Orthography { get; }
}

// TODO: What system will be used to map words to meanings?
public record ConceptReference(string Orthography, string ConceptNetId) : ILexemeReference
{
    public override string ToString() => $"ConceptNet{{{ConceptNetId}}}";
}

public record LexiconEntryReference(LexiconEntry Entry, Form Form) : ILexemeReference
{
    public string Orthography => Entry.Senses[0].Translations.GetByLanguage(KnownLanguage.English).ToString();

    public override string ToString()
    {
        var sb = new StringBuilder("LexEntry{");
        
        sb.Append(Entry.Id);
        sb.Append(' ');
        sb.Append(Form.Orthography);

        var grammar = Entry.GrammarGroup ?? Form.GrammarGroup;
        if (grammar is not null)
        {
            sb.Append(' ');
            sb.Append(grammar.Gender.ToAbbreviation());
            sb.Append('.');
            sb.Append(grammar.Number.ToGrammaticalCount().ToAbbreviation());
            sb.Append(' ');
            sb.Append(grammar.PartOfSpeech);

            if (grammar.Subclass is not PartOfSpeechSubclass.Unknown)
            {
                sb.Append('.');
                sb.Append(grammar.Subclass);
            }
        }
        
        sb.Append('}');
        
        return sb.ToString();
    }
}
