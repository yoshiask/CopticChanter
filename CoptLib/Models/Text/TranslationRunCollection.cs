using CoptLib.Extensions;
using CoptLib.Writing;
using System;
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
        private readonly Dictionary<KnownLanguage, Run> _known = new();

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

        public IList<IDefinition> References { get; } = new List<IDefinition>();

        /// <summary>
        /// Creates a new <see cref="Run"/> with the given text and language.
        /// </summary>
        /// <param name="text">The text content.</param>
        /// <param name="knownLanguage">The language of the content.</param>
        /// <returns>The <see cref="Run"/> that was created.</returns>
        public Run AddNew(string text, KnownLanguage knownLanguage)
        {
            Run run = new(text, this)
            {
                Language = new(knownLanguage)
            };

            // Add to self, for the complex LanguageInfo comparison
            Add(run);

            // Add to internal dictionary, for fast lookups of known languages
            _known.Add(knownLanguage, run);

            return run;
        }

        public TMulti GetByLanguage<TMulti>(KnownLanguage knownLanguage)
            where TMulti : IMultilingual
        {
            var run = _known[knownLanguage];

            if (run is TMulti multi)
                return multi;
            else
                throw new InvalidOperationException($"Element {{ {knownLanguage}, '{run}' }} was not of type '{typeof(TMulti)}'");
        }

        public TMulti GetByLanguage<TMulti>(LanguageInfo language, LanguageEquivalencyOptions options = LanguageEquivalencyOptions.StrictWithWild)
            where TMulti : IMultilingual
        {
            return this
                .ElementsAs<TMulti>()
                .First(t => t.Language?.IsEquivalentTo(language, options) ?? false);
        }

        public override string ToString()
        {
            return $"[ \"{string.Join("\", \"", this)}\" ]";
        }
    }
}
