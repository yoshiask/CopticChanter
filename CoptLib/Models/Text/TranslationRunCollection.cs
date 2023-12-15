using CoptLib.Writing;
using System;
using System.Collections.Generic;

namespace CoptLib.Models.Text;

/// <summary>
/// Represents a collection of <see cref="Run"/>s for use in places
/// where multiple translations of simple text need to be stored.
/// <para>
/// For example, this collection can be used by scripts that return
/// text intended for embedding directly in <see cref="IContent"/>
/// elements.
/// </para>
/// </summary>
public class TranslationRunCollection : TranslationCollection<Run>
{
    private readonly Dictionary<KnownLanguage, Run> _known = new();

    public TranslationRunCollection(string? key = null, IDefinition? parent = null)
        : base(key, parent)
    {
    }

    /// <summary>
    /// Creates a new <see cref="Run"/> with the given text and language.
    /// </summary>
    /// <param name="text">The text content.</param>
    /// <param name="knownLanguage">The language of the content.</param>
    /// <returns>The <see cref="Run"/> that was created.</returns>
    public Run AddText(string text, KnownLanguage knownLanguage)
    {
        Run run = new(text, this)
        {
            Language = new(knownLanguage)
        };

        AddRun(run);

        return run;
    }

    /// <summary>
    /// Adds the existing <see cref="Run"/> to the list of translations.
    /// </summary>
    /// <param name="run">The run to add.</param>
    public void AddRun(Run run)
    {
        // Add to self, for the complex LanguageInfo comparison
        Add(run);

        // Add to internal dictionary, for fast lookups of known languages
        _known[run.Language.Known] = run;
    }

    /// <summary>
    /// Adds the existing <see cref="Run"/>s to the list of translations.
    /// </summary>
    /// <param name="runs">The runs to add.</param>
    public void AddRuns(IEnumerable<Run> runs)
    {
        foreach (var run in runs)
            AddRun(run);
    }
        
    public override Run GetByLanguage(KnownLanguage knownLanguage, Func<Run, bool>? predicate = null)
    {
        if (!_known.TryGetValue(knownLanguage, out Run run) || (predicate is not null && !predicate(run)))
        {
            if (!_known.TryGetValue(KnownLanguage.Default, out run) || (predicate is not null && !predicate(run)))
                run = base.GetByLanguage(knownLanguage, predicate);
        }

        return run;
    }

    public override string ToString()
    {
        return $"[ \"{string.Join("\", \"", this)}\" ]";
    }
}