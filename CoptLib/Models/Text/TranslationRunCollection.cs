using CoptLib.Extensions;
using CoptLib.Writing;
using System.Collections.Generic;
using System.Linq;

namespace CoptLib.Models.Text
{
    /// <summary>
    /// Represents a collection of <see cref="Run"/>s for use in places
    /// where multiple translations of simple text need to be stored.
    /// <para>
    /// For example, this collection can be used by scripts that return
    /// text intended for embedding directly in <see cref="IContent"/>
    /// elements.
    /// </para>
    /// </summary>
    public class TranslationRunCollection : List<Run>, ITranslationLookup, IDefinition
    {
        public TranslationRunCollection(string key = null, IDefinition parent = null)
        {
            Key = key;
            Parent = parent;
            DocContext = parent?.DocContext;
        }

        public string Key { get; set; }

        public Doc DocContext { get; set; }

        public IDefinition Parent { get; set; }

        public bool IsExplicitlyDefined { get; set; }

        /// <summary>
        /// Creates a new <see cref="Run"/> with the given text and language.
        /// </summary>
        /// <param name="text">The text content.</param>
        /// <param name="knownLanguage">The language of the content.</param>
        public void AddNew(string text, KnownLanguage knownLanguage)
            => Add(new(text, this) { Language = new(knownLanguage) });

        public TMulti GetByLanguage<TMulti>(KnownLanguage knownLanguage)
            where TMulti : IMultilingual
        {
            return this
                .ElementsAs<Run, TMulti>()
                .First(t => t.Language?.Known == knownLanguage);
        }

        public TMulti GetByLanguage<TMulti>(LanguageInfo language, LanguageEquivalencyOptions options = LanguageEquivalencyOptions.StrictWithWild)
            where TMulti : IMultilingual
        {
            return this
                .ElementsAs<Run, TMulti>()
                .First(t => t.Language?.IsEquivalentTo(language, options) ?? false);
        }
    }
}
