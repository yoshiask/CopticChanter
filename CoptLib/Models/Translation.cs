using System.Collections.Generic;
using System.Xml.Serialization;

namespace CoptLib.Models
{
    public class Translation : IContentCollectionContainer
    {
        [XmlArray]
        public List<ContentPart> Content { get; set; } = new List<ContentPart>();

        [XmlElement]
        public Language Language { get; set; }

        [XmlElement]
        public string Font { get; set; }

        [XmlIgnore]
        public Doc Parent { get; set; }

        /// <summary>
        /// Returns the number of rows this translation requires to display
        /// all section headers and stanzas
        /// </summary>
        /// <returns></returns>
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
    }
}
