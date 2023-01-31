using System.Xml.Linq;

namespace CoptLib.Models
{
    public class Author
    {
        public string FullName { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string Website { get; set; }

        public XElement SerializeElement()
        {
            XElement xml = new(nameof(Author));
            xml.SetAttributeValue(nameof(FullName), FullName);
            xml.SetAttributeValue(nameof(PhoneNumber), PhoneNumber);
            xml.SetAttributeValue(nameof(Email), Email);
            xml.SetAttributeValue(nameof(Website), Website);
            return xml;
        }

        public static Author DeserializeElement(XElement elem)
        {
            return new()
            {
                FullName = elem.Attribute(nameof(FullName))?.Value,
                PhoneNumber = elem.Attribute(nameof(PhoneNumber))?.Value,
                Email = elem.Attribute(nameof(Email))?.Value,
                Website = elem.Attribute(nameof(Website))?.Value
            };
        }
    }
}
