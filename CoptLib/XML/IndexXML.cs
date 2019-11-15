using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CoptLib.XML
{
    [XmlRoot(ElementName = "CopticDocIndex")]
    public class IndexXML
    {
        [XmlElement(ElementName = "name")]
        public string Name;

        [XmlElement(ElementName = "uuid")]
        public string UUID;

        [XmlElement]
        public List<IndexDocXML> IncludedDocs = new List<IndexDocXML>();
    }

    [XmlRoot(ElementName = "Doc")]
    public class IndexDocXML
    {
        [XmlElement(ElementName = "name")]
        public string Name;

        [XmlElement(ElementName = "uuid")]
        public string UUID;

        [XmlElement]
        public bool hasCoptic;

        [XmlElement]
        public bool hasEnglish;

        [XmlElement]
        public bool hasArabic;
    }
}
