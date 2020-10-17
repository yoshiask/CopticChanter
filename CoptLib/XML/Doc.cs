using System.Collections.Generic;
using System.Xml.Serialization;

namespace CoptLib.XML
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

        //[XmlElement(ElementName = "If", IsNullable = true)]
        //public IfXML Script = new IfXML();

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

    public class Translation
    {
        [XmlArray]
        public List<ContentPart> Content { get; set; } = new List<ContentPart>();

        [XmlElement]
        public CopticInterpreter.Language Language { get; set; }

        [XmlElement]
        public string Font { get; set; }

        /// <summary>
        /// Returns the number of rows this translation requires to display
        /// all section headers and stanzas
        /// </summary>
        /// <returns></returns>
        public int CountRows()
		{
            int count = 0;
            foreach (ContentPart part in Content)
            {
                if (part is Stanza)
                    count++;
                else if (part is Section section)
                    count += section.CountRows() + 1;
            }
            return count;
        }
    }
}
