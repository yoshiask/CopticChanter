using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CoptLib.XML
{
    [XmlRoot(ElementName = "CopticDoc")]
    public class DocXML
    {
        [XmlElement]
        public string Name;

        [XmlElement]
        public string UUID;

        [XmlElement]
        public string Parent;
        [XmlElement]
        public string Language;

        [XmlArray]
        public List<StanzaXML> Stanzas = new List<StanzaXML>();

        [XmlElement]
        public bool Coptic;

        [XmlElement(ElementName = "If", IsNullable = true)]
        public ScriptXML.IfXML Script = new ScriptXML.IfXML();

        [XmlElement(ElementName = "DefaultNext", IsNullable = false)]
        public string DefaultNextGuid = "ccc91ccc-77ba-45b2-9555-e9f0fe8c10c3";

        public IndexDocXML ToIndexDocXML()
        {
            switch (Language)
            {
                case "english":
                    return new IndexDocXML()
                    {
                        Name = this.Name,
                        UUID = this.UUID,
                        hasCoptic = false,
                        hasEnglish = true,
                        hasArabic = false
                    };

                case "coptic":
                    return new IndexDocXML()
                    {
                        Name = this.Name,
                        UUID = this.UUID,
                        hasCoptic = true,
                        hasEnglish = false,
                        hasArabic = false
                    };

                case "arabic":
                    return new IndexDocXML()
                    {
                        Name = this.Name,
                        UUID = this.UUID,
                        hasCoptic = false,
                        hasEnglish = false,
                        hasArabic = true
                    };

                default:
                    return new IndexDocXML()
                    {
                        Name = this.Name,
                        UUID = this.UUID,
                        hasCoptic = this.Coptic,
                        hasEnglish = !this.Coptic,
                        hasArabic = false
                    };
            }
        }

        public class StanzaXML
        {
            public StanzaXML() { }
            public StanzaXML(string content, string lang)
            {
                Language = lang;
                Content = content;
            }

            [XmlElement("Language")]
            public string Language;

            [XmlElement]
            public string Content;
        }
    }
}
