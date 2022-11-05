using System.Collections.Generic;
using System.Xml.Serialization;

namespace CoptLib.Models
{
    [XmlRoot("Document")]
    public class Doc
    {
        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public string Uuid { get; set; }

        [XmlElement]
        public string Parent { get; set; }

        [XmlArray("Translations")]
        public TranslationCollection Translations { get; set; } = new(null);

        [XmlArray("Definitions")]
        public List<IDefinition> DirectDefinitions { get; set; } = new();

        [XmlElement]
        public string NextScript { get; set; }

        [XmlIgnore]
        public Dictionary<string, IDefinition> Definitions { get; } = new();

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
