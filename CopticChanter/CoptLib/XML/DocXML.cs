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
        public string Parent;

        [XmlElement]
        public List<string> Content;

        [XmlElement]
        public bool Coptic;
    }
}
