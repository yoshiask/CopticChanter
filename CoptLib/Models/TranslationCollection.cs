using System;
using System.Collections.Generic;
using System.Linq;
using CoptLib.Writing;

namespace CoptLib.Models;

public class TranslationCollection<T> : List<T>, ITranslationLookup<T>, IDefinition where T : IMultilingual
{
    public TranslationCollection(string? key = null, IDefinition? parent = null)
    {
        Key = key;
        Parent = parent;
        DocContext = parent?.DocContext;
    }

    public string? Key { get; set; }
    public Doc? DocContext { get; set; }
    public IDefinition? Parent { get; set; }
    public bool IsExplicitlyDefined { get; set; }
    public ICollection<IDefinition> References { get; } = new List<IDefinition>();
    
    public virtual T GetByLanguage(KnownLanguage knownLanguage, Func<T, bool>? predicate = null)
    {
        var items = predicate is null
            ? this : this.Where(predicate).ToList();

        var match = items.FirstOrDefault(t => t.Language.Known == knownLanguage);
        return match ?? items.FirstOrDefault(t => t.Language.Known == KnownLanguage.Default);
    }

    public virtual T GetByLanguage(LanguageInfo language, Func<T, bool>? predicate = null, LanguageEquivalencyOptions options = LanguageInfo.DefaultLEO)
    {
        IEnumerable<T> items = predicate is null
            ? this : this.Where(predicate).ToList();

        var match = items.FirstOrDefault(t => t.Language.IsEquivalentTo(language, options));
        return match ?? items.FirstOrDefault(t => t.Language.Known == KnownLanguage.Default);
    }
}

public class TranslationCollection : TranslationCollection<IMultilingual>
{
    public TranslationCollection(string? key = null, IDefinition? parent = null)
        : base(key, parent)
    {
    }
}