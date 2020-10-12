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
    public class DocXml
    {
        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public string Uuid { get; set; }

        [XmlElement]
        public string Parent { get; set; }

        [XmlArray("Content")]
        [XmlArrayItem("Text", typeof(string))]
        public List<string> Content { get; set; } = new List<string>();

        [XmlIgnore]
        public ObservableCollection<StanzaXml> ContentCollection { get; set; } = new ObservableCollection<StanzaXml>();

        [XmlElement]
        public CopticInterpreter.Language Language { get; set; }

        [XmlElement]
        public string NextScript { get; set; }

        //[XmlElement(ElementName = "If", IsNullable = true)]
        //public IfXML Script = new IfXML();

        //[XmlElement(ElementName = "DefaultNext", IsNullable = false)]
        //public string DefaultNextGuid = "ccc91ccc-77ba-45b2-9555-e9f0fe8c10c3";

        public IndexDocXml ToIndexDocXml()
        {
            return new IndexDocXml()
            {
                Name = this.Name,
                Uuid = this.Uuid,
                Language = this.Language
            };
        }
    }

    public class StanzaXml
    {
        public StanzaXml() { }
        public StanzaXml(string content)
        {
            Text = content;
        }

        [XmlElement]
        public string Text { get; set; }
    }
}
