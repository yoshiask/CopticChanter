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
        public string Name;

        public string UUID;

        [XmlElement]
        public List<IndexDocXML> IncludedDocs = new List<IndexDocXML>();
    }

    [XmlRoot(ElementName = "Doc")]
    public class IndexDocXML
    {
        [XmlAttribute]
        public string Name;

        [XmlAttribute]
        public string UUID;

        [XmlAttribute]
        public CopticInterpreter.Language Language;
    }
}
