using System.Collections.Generic;
using System.Xml.Serialization;

namespace CoptLib.Models
{
    [XmlRoot(ElementName = "Document")]
    public class Doc
    {
        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public string Uuid { get; set; }

        [XmlElement]
        public string Parent { get; set; }

        [XmlArray("Translations")]
        [XmlArrayItem("Translation", typeof(Translation))]
        public List<Translation> Translations { get; set; } = new List<Translation>();

        [XmlArray]
        public List<Definition> Definitions { get; set; } = new List<Definition>();

        [XmlElement]
        public string NextScript { get; set; }

        //[XmlElement(ElementName = "DefaultNext", IsNullable = false)]
        //public string DefaultNextGuid = "ccc91ccc-77ba-45b2-9555-e9f0fe8c10c3";

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
