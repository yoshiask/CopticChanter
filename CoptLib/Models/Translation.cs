using CoptLib.IO;
using CoptLib.Writing;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CoptLib.Models
{
    public class Translation : Definition, IContentCollectionContainer, IMultilingual
    {
        [XmlArray]
        public List<ContentPart> Content { get; set; } = new List<ContentPart>();

        [XmlElement]
        public string Source { get; set; }

        [XmlElement]
        public Language Language { get; set; }

        [XmlElement]
        public string Font { get; set; }

        public bool Handled { get; private set; }

        public int CountRows()
        {
            int count = 0;
            foreach (ContentPart part in Content)
            {
                if (part is Stanza)
                    count++;
                else if (part is Section section)
                    count += section.CountRows() + 1;
            }
            return count;
        }

        public void HandleFont()
        {
            foreach (ContentPart part in Content)
                part.HandleFont();
        }

        public void ParseCommands() => DocReader.RecursiveParseCommands(Content);
    }
}
