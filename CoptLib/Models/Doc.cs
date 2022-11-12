using System.Collections.Generic;
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
        public string Parent { get; set; }

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
    }
}
