using System;
using System.Collections.Generic;
using System.Linq;
using CoptLib.Writing;

namespace CoptLib.Models;

public class TranslationCollection<T> : List<T>, ITranslationLookup<T>, IDefinition where T : IMultilingual
{
    public TranslationCollection(string key = null, IDefinition parent = null)
    {
        Key = key;
        Parent = parent;
        DocContext = parent?.DocContext;
    }

    public string Key { get; set; }
    public Doc DocContext { get; set; }
    public IDefinition Parent { get; set; }
    public bool IsExplicitlyDefined { get; set; }
    public IList<IDefinition> References { get; } = new List<IDefinition>();
    
    public virtual T GetByLanguage(KnownLanguage knownLanguage, Func<T, bool> predicate = null)
    {
        IEnumerable<T> items = predicate is null
            ? this : this.Where(predicate);

        return items
            .First(t => t.Language?.Known == knownLanguage);
    }

    public virtual T GetByLanguage(LanguageInfo language, Func<T, bool> predicate = null, LanguageEquivalencyOptions options = LanguageInfo.DefaultLEO)
    {
        IEnumerable<T> items = predicate is null
            ? this : this.Where(predicate);

        return items
            .First(t => t.Language?.IsEquivalentTo(language, options) ?? false);
    }
}

public class TranslationCollection : TranslationCollection<IMultilingual>
{
    public TranslationCollection(string key = null, IDefinition parent = null)
        : base(key, parent)
    {
    }
}