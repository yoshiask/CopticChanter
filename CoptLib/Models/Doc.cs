using CommunityToolkit.Diagnostics;
using CoptLib.Extensions;
using CoptLib.IO;
using CoptLib.Scripting;
using CoptLib.Writing;
using OwlCore.Extensions;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Serialization;

namespace CoptLib.Models
{
    [XmlRoot("Document")]
    public class Doc : Definition, IContextualLoad
    {
        private LoadContextBase _context;

        public Doc(LoadContextBase context = null)
        {
            _context = context ?? new();
            Translations = new(null)
            {
                DocContext = this
            };

            Parent = null;
            DocContext = this;
            IsExplicitlyDefined = true;
        }

        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public Author Author { get; set; }

        [XmlArray("Translations")]
        public TranslationCollection Translations { get; set; }

        [XmlArray("Definitions")]
        public IReadOnlyCollection<IDefinition> DirectDefinitions { get; set; } = System.Array.Empty<IDefinition>();

        [XmlElement]
        public string NextScript { get; set; }

        [XmlIgnore]
        [NotNull]
        public LoadContextBase Context
        {
            get => _context;
            set
            {
                Guard.IsNotNull(value);
                _context = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="IDefinition"/> associated with the given key.
        /// </summary>
        /// <param name="key">The key to lookup.</param>
        public IDefinition LookupDefinition(string key) => Context.LookupDefinition(key, this);

        /// <summary>
        /// Adds an <see cref="IDefinition"/> to the current context, scoped to the
        /// this document if another definition with the same key exists.
        /// </summary>
        /// <param name="definition">The definition to add.</param>
        public void AddDefinition(IDefinition definition) => Context.AddDefinition(definition, this);

        public IndexDoc ToIndexDocXml()
        {
            return new IndexDoc()
            {
                Name = this.Name,
                Uuid = this.Key
            };
        }

        /// <summary>
        /// Flattens the document to a list of lists (2D array), where
        /// each list is a single row in the document.
        /// </summary>
        /// <param name="excludedTranslations">
        /// A list of translations to exclude from the layout.
        /// </param>
        /// <param name="includedTranslations">
        /// A list of translations to explicitly include in the layout.
        /// Overrides entries in <paramref name="excludedTranslations"/>.
        /// </param>
        /// <remarks>
        /// Typically used for generating a layout for display purposes.
        /// </remarks>
        public List<List<object>> Flatten(IEnumerable<LanguageInfo> excludedTranslations = null, IEnumerable<LanguageInfo> includedTranslations = null)
        {
            Guard.IsNotNull(Translations);

            // Remove any entries in the excluded translations list
            // to prevent unnecessary equality checks later
            excludedTranslations ??= System.Array.Empty<LanguageInfo>();
            includedTranslations ??= System.Array.Empty<LanguageInfo>();

            var allTranslations = Translations.Children.Select(t => t.Language).ToList();
            includedTranslations = includedTranslations.Intersect(allTranslations, LanguageInfoEqualityComparer.StrictWithWild).ToList();
            var finalTranslations = allTranslations
                .Except(excludedTranslations, LanguageInfoEqualityComparer.StrictWithWild)
                .Union(includedTranslations);

            // Get the number of translations to display
            int translationCount = finalTranslations.Count();

            // Create rows for each stanza
            int numRows = Translations.CountRows();
            List<List<object>> layout = new(numRows + 1)
            {
                // Add Doc to row so consumer can decide whether to show
                // the document name
                this.IntoList<object>()
            };

            for (int i = 0; i < numRows; i++)
                layout.Add(new(translationCount));

            for (int t = 0; t < Translations.Children.Count; t++)
            {
                var translation = Translations.Children[t];

                // Ignore translation if it's in the exclusion list but not the inclusion list
                if (excludedTranslations.Contains(translation.Language, LanguageInfoEqualityComparer.StrictWithWild)
                    && !includedTranslations.Contains(translation.Language, LanguageInfoEqualityComparer.StrictWithWild))
                    continue;

                var flattenedTranslation = translation.Flatten(p => p is IContentCollectionContainer coll ? coll.Children : null)
                    // The row count excludes sections without headers,
                    // so we have to do the same here.
                    .Where(p => p is Section section ? section.Title != null : p != null)
                    .ToList<object>();

                foreach (var (elem, i) in flattenedTranslation.WithIndex())
                    layout[i + 1].Add(elem);
            }

            return layout;
        }

        /// <summary>
        /// Parses text commands and applies font conversions.
        /// </summary>
        public void ApplyTransforms()
        {
            RecursiveTransform(DirectDefinitions);
            RecursiveTransform(Translations.Children);
        }

        internal static void RecursiveTransform(System.Collections.IEnumerable parts)
        {
            foreach (var part in parts)
                Transform(part);
        }

        internal static void Transform(object part)
        {
            if (part is CScript partScript)
                partScript.Run();

            if (part is ICommandOutput<object> partCmdOut && partCmdOut.Output != null)
                part = partCmdOut.Output;

            if (part is IContent partContent)
            {
                if (part is IMultilingual partMulti && partMulti.Language?.Known == KnownLanguage.Coptic)
                    partContent.SourceText = CopticInterpreter.ExpandAbbreviations(partContent.SourceText);

                partContent.HandleCommands();
            }

            if (part is IContentCollectionContainer partCollection)
            {
                var collSrc = partCollection.Source;
                if (collSrc != null && !collSrc.CommandsHandled)
                {
                    // Populate the collection with items from the source.
                    // This is done before commands are parsed, just in
                    // case the generated content contains commands.
                    collSrc.HandleCommands();
                    var cmd = collSrc.Commands.LastOrDefault();

                    if (cmd.Output != null)
                    {
                        bool hasExplicitChildren = partCollection.Children.Count > 0;

                        if (cmd.Output is IContentCollectionContainer cmdOutCollection)
                            partCollection.Children.AddRange(cmdOutCollection.Children);
                        else if (cmd.Output is ContentPart cmdOutPart)
                            partCollection.Children.Add(cmdOutPart);

                        // If the collection doesn't have any explicit elements (in other words,
                        // it's only children came from the source), inherit the command's properties
                        if (!hasExplicitChildren)
                        {
                            if (cmd.Output is IMultilingual cmdMulti && part is IMultilingual partMulti)
                            {
                                partMulti.Language = cmdMulti.Language;
                                partMulti.Font = cmdMulti.Font;
                            }

                            if (cmd.Output is Section cmdSection && part is Section partSection)
                                partSection.Title = cmdSection.Title;
                        }
                    }
                }

                partCollection.HandleCommands();
            }

            if (part is IMultilingual multilingual)
                multilingual.HandleFont();

            if (part is IDefinition def && def.Key != null && def.DocContext is not null)
                def.DocContext.AddDefinition(def);
        }
    }
}
