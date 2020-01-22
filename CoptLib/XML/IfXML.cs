using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CoptLib.XML
{
    [XmlRoot(ElementName = "If")]
    public class IfXml
    {
        /// <summary>
        /// Specifies the left hand of a typical if statement.
        /// e.g. the "C" in "if (A > B) then do C"
        /// </summary>
        [XmlElement(ElementName = "If")]
        public IfXml If;

        [XmlElement]
        public string Return = "";

        /// <summary>
        /// Specifies the left hand of a typical if statement.
        /// e.g. the "A" in "if (A > B) then do C"
        /// Type must be specified as "<type>:<variable>"
        /// </summary>
        [XmlAttribute]
        public string LeftHand = "bool:true";

        /// <summary>
        /// Specifies the operation/comparison to test a typical if statement.
        /// e.g. the ">" in "if (A > B) then do C"
        /// Valid: lth, lth=, gth, gth=, ==, !=
        /// </summary>
        [XmlAttribute]
        public string Comparator = "==";

        /// <summary>
        /// Specifies the right hand of a typical if statement.
        /// e.g. the "B" in "if (A > B) then do C"
        /// </summary>
        [XmlAttribute]
        public string RightHand = "bool:true";

        public override string ToString()
        {
            var xml = XDocument.Parse(this.ToXmlString());
            xml.Root.FirstAttribute.Remove();
            xml.Root.FirstAttribute.Remove();
            return xml.Root.ToString();
        }

        public static IfXml FromString(string s)
        {
            if (s == "")
            {
                return new IfXml();
            }
            // Create an instance of the XmlSerializer class;
            // specify the type of object to be deserialized.
            XmlSerializer serializer = new XmlSerializer(typeof(IfXml));

            //Use the Deserialize method to restore the object's state with
            //data from the XML document.
            var reader = XDocument.Parse(s).CreateReader();
            return (IfXml)serializer.Deserialize(reader);
        }
    }
}
