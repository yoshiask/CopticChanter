using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.DependencyInjection;
using CoptLib.Extensions;
using OwlCore.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace CoptLib.Models
{
    [XmlRoot("Document")]
    public class Doc
    {
        public Doc()
        {
            Translations = new(null)
            {
                DocContext = this
            };
        }

        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public string Uuid { get; set; }

        [XmlElement]
        public Author Author { get; set; }

        [XmlArray("Translations")]
        public TranslationCollection Translations { get; set; }

        [XmlArray("Definitions")]
        public List<IDefinition> DirectDefinitions { get; set; } = new();

        [XmlElement]
        public string NextScript { get; set; }

        [XmlIgnore]
        public Dictionary<string, IDefinition> Definitions { get; } = new();

        public void AddDefinition(IDefinition def) => Definitions.Add(def.Key, def);

        public IndexDoc ToIndexDocXml()
        {
            return new IndexDoc()
            {
                Name = this.Name,
                Uuid = this.Uuid
            };
        }

        /// <summary>
        /// Flattens the document to a list of lists (2D array), where
        /// each list is a single row in the document.
        /// </summary>
        /// <remarks>
        /// Typically used for generating a layout for display purposes.
        /// </remarks>
        public List<List<object>> Flatten()
        {
            Guard.IsNotNull(Translations);

            int translationCount = Translations.Children.Count;

            // Create rows for each stanza
            int numRows = Translations.CountRows() + 1;
            List<List<object>> layout = new(numRows);
            for (int i = 1; i <= numRows; i++)
                layout.Add(new(translationCount));

            List<List<object>> columns = new(translationCount);
            for (int t = 0; t < translationCount; t++)
            {
                var flattenedTranslation = Translations[t].Flatten(p => p is IContentCollectionContainer coll ? coll.Children : null).ToList<object>();
                foreach (var (elem, i) in flattenedTranslation.WithIndex())
                    layout[i].Add(elem);
            }

            // Add Doc to row so consumer can decide whether to show
            // the document name
            layout.Insert(0, this.IntoList<object>());

            return layout;
        }
    }
}
