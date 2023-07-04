using CommunityToolkit.Diagnostics;
using CoptLib.IO;
using CoptLib.Scripting;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace CoptLib.Models
{
    [XmlRoot("Document")]
    public class Doc : Definition, IContextualLoad
    {
        private LoadContextBase _context;
        private bool _transformed = false;

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
        public TranslationCollectionSection Translations { get; set; }

        [XmlArray("Definitions")]
        public IReadOnlyCollection<IDefinition> DirectDefinitions { get; set; } = System.Array.Empty<IDefinition>();

        [XmlElement]
        public string NextScript { get; set; }

        [XmlIgnore]
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

        /// <summary>
        /// Parses text commands and applies font conversions.
        /// </summary>
        /// <param name="force">
        /// When <see langword="true"/>, the transform will be reapplied
        /// even if the document has already been parsed. Note that this does
        /// not apply recursively, meaning that commands and font conversions
        /// that have already been applied and are unchanged will not be duplicated.
        /// </param>
        public void ApplyTransforms(bool force = false)
        {
            if (_transformed && !force)
                return;

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

            if (part is ICommandOutput<object> {Output: not null} partCmdOut)
                part = partCmdOut.Output;

            if (part is IContent partContent and IMultilingual partContentMulti)
            {
                try
                {
                    var analyzer = Writing.Linguistics.LinguisticLanguageService.Default.GetAnalyzerForLanguage(partContentMulti.Language);
                    partContent.SourceText = analyzer.ExpandAbbreviations(partContent.SourceText);
                }
                catch { }
            }

            if (part is IContentCollectionContainer partCollection)
            {
                var collSrc = partCollection.Source;
                if (collSrc is {CommandsHandled: false})
                {
                    // Populate the collection with items from the source.
                    // This is done before commands are parsed, just in
                    // case the generated content contains commands.
                    collSrc.HandleCommands();
                    var cmd = collSrc.Commands.LastOrDefault();

                    if (cmd?.Output != null)
                    {
                        bool hasExplicitChildren = partCollection.Children.Count > 0;

                        switch (cmd.Output)
                        {
                            case IContentCollectionContainer cmdOutCollection:
                                partCollection.Children.AddRange(cmdOutCollection.Children);
                                break;
                            case ContentPart cmdOutPart:
                                partCollection.Children.Add(cmdOutPart);
                                break;
                        }

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
                                partSection.SetTitle(cmdSection.Title);
                        }
                    }
                }
            }

            if (part is ISupportsTextCommands suppTextCmds)
                suppTextCmds.HandleCommands();

            if (part is IMultilingual multilingual)
                multilingual.HandleFont();

            if (part is IDefinition {Key: not null, DocContext: not null} def)
                def.DocContext.AddDefinition(def);
        }
    }
}
