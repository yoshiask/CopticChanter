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
    public string Orthography => GetOrthography();

    private string GetOrthography()
    {
        var definition = Entry.Senses[0].Translations.GetByLanguage(KnownLanguage.English).ToString();
        var translation = definition;
            
        // Remove any alternate translations
        var commaIndex = translation.IndexOf(',');
        if (commaIndex > 0)
            translation = translation[..commaIndex];

        // Remove any comments within parenthesis
        var openBracketIndex = translation.IndexOf('(');
        if (openBracketIndex > 0)
        {
            var closeBracketIndex = translation.IndexOf(')', openBracketIndex);
                
            var commentLength = closeBracketIndex > 0
                ? closeBracketIndex - openBracketIndex + 1
                : translation.Length - openBracketIndex;
            translation = translation.Remove(openBracketIndex, commentLength);
        }
            
        return translation.Trim();
    }

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
