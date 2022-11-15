using System.Xml.Serialization;

namespace CoptLib.Models
{
    public class Author
    {
        [XmlElement]
        public string FullName { get; set; }

        [XmlElement]
        public string PhoneNumber { get; set; }

        [XmlElement]
        public string Email { get; set; }

        [XmlElement]
        public string Website { get; set; }
    }
}
