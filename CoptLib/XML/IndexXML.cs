using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CoptLib.XML
{
    [XmlRoot(ElementName = "CopticDocIndex")]
    public class IndexXml
    {
        public string Name;

        public string Uuid;

        [XmlElement]
        public List<IndexDocXml> IncludedDocs = new List<IndexDocXml>();
    }

    [XmlRoot(ElementName = "Doc")]
    public class IndexDocXml
    {
        [XmlAttribute]
        public string Name;

        [XmlAttribute]
        public string Uuid;

        [XmlAttribute]
        public CopticInterpreter.Language Language;
    }
}
