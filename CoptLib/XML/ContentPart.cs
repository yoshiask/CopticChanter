using System.Collections.Generic;
using System.Xml.Serialization;

namespace CoptLib.XML
{
	/// <summary>
	/// A base class for anything that can be placed inside the content of a <see cref="Translation"/>.
	/// </summary>
	public abstract class ContentPart
	{
    }

    public class Stanza : ContentPart
    {
        public Stanza() { }
        public Stanza(string content)
        {
            Text = content;
        }

        [XmlText]
        public string Text { get; set; }

        [XmlAttribute]
        public CopticInterpreter.Language Language { get; set; }
    }

    [XmlRoot("Section")]
    public class SectionHeader : ContentPart
	{
        [XmlAttribute()]
        public string Title { get; set; }
	}
}
