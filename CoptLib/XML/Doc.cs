using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CoptLib.XML
{
    [XmlRoot(ElementName = "Document")]
    public class Doc
    {
        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public string Uuid { get; set; }

        [XmlElement]
        public string Parent { get; set; }

        [XmlArray("Content")]
        [XmlArrayItem("Translation", typeof(Translation))]
        public List<Translation> Content { get; set; } = new List<Translation>();

        [XmlIgnore]
        public ObservableCollection<Stanza> ContentCollection { get; set; } = new ObservableCollection<Stanza>();

        //[XmlElement]
        //public CopticInterpreter.Language Language { get; set; }

        [XmlElement]
        public string NextScript { get; set; }

        //[XmlElement(ElementName = "If", IsNullable = true)]
        //public IfXML Script = new IfXML();

        //[XmlElement(ElementName = "DefaultNext", IsNullable = false)]
        //public string DefaultNextGuid = "ccc91ccc-77ba-45b2-9555-e9f0fe8c10c3";

        public IndexDoc ToIndexDocXml()
        {
            return new IndexDoc()
            {
                Name = this.Name,
                Uuid = this.Uuid
            };
        }
    }

    public class Translation
	{
        [XmlArray]
        public List<Stanza> Stanzas { get; set; } = new List<Stanza>();

        [XmlElement]
        public CopticInterpreter.Language Language { get; set; }

        [XmlElement]
        public string Font { get; set; }
    }

    public class Stanza
    {
        public Stanza() { }
        public Stanza(string content)
        {
            Text = content;
        }

        [XmlText]
        public string Text { get; set; }

        public CopticInterpreter.Language Language { get; set; }
    }
}
