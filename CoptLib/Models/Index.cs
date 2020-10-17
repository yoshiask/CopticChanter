using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CoptLib.Models
{
    [XmlRoot(ElementName = "CopticDocIndex")]
    public class Index
    {
        public string Name;

        public string Uuid;

        [XmlElement]
        public List<IndexDoc> IncludedDocs = new List<IndexDoc>();
    }

    [XmlRoot(ElementName = "Doc")]
    public class IndexDoc
    {
        [XmlAttribute]
        public string Name;

        [XmlAttribute]
        public string Uuid;

        //[XmlAttribute]
        //public Language Language;
    }
}
