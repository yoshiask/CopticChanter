using System.Xml.Serialization;

namespace CoptLib.Models
{
    [XmlRoot("Doc")]
    public class DocIndexEntry
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string Uuid { get; set; }

        [XmlAttribute]
        public string RelativePath { get; set; }
    }
}
