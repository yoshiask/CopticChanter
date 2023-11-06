using CoptLib.Models;
using System.Collections.Generic;

namespace CoptLib.Writing.Lexicon;

public interface ILexiconEntry
{
}

public record LexiconSuperEntry(IEnumerable<LexiconEntry> Entries) : ILexiconEntry;

public record LexiconEntry(string Id, EntryType Type, List<Form> Forms, List<Sense> Senses, GrammarGroup GrammarGroup) : ILexiconEntry;

public enum EntryType : byte
{
    Hom,
    Compound,
    Foreign,
}

/// <summary>
/// Represents a specific form of a lexicon entry.
/// </summary>
/// <param name="Type"></param>
/// <param name="Usage">The language or dialect that uses this form.</param>
/// <param name="Orthography">The orthography of this form.</param>
public record Form(FormType Type, LanguageInfo Usage, string Orthography)
{
}

public enum FormType : byte
{
    Lemma,
    Inflected,
    Compound,
}

public record Sense(TranslationCollection Translations, string Bibliography = "")
{
}
