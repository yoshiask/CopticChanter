using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public string Name { get; set; }

        [XmlElement]
        public string UUID { get; set; }

        [XmlElement]
        public string Parent { get; set; }

        [XmlArray(ElementName = "Content")]
        public List<StanzaXML> Content { get; set; } = new List<StanzaXML>();

        [XmlIgnore]
        public ObservableCollection<StanzaXML> ContentCollection { get; set; } = new ObservableCollection<StanzaXML>();

        [XmlElement]
        public CopticInterpreter.Language Language { get; set; }

        [XmlElement]
        public string NextScript { get; set; }

        //[XmlElement(ElementName = "If", IsNullable = true)]
        //public IfXML Script = new IfXML();

        //[XmlElement(ElementName = "DefaultNext", IsNullable = false)]
        //public string DefaultNextGuid = "ccc91ccc-77ba-45b2-9555-e9f0fe8c10c3";

        public IndexDocXML ToIndexDocXML()
        {
            return new IndexDocXML()
            {
                Name = this.Name,
                UUID = this.UUID,
                Language = this.Language
            };
        }
    }

    public class StanzaXML
    {
        public StanzaXML() { }
        public StanzaXML(string content)
        {
            Text = content;
        }

        [XmlElement]
        public string Text { get; set; }
    }
}
