using CommunityToolkit.Diagnostics;
using CoptLib.IO;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Linq;

namespace CoptLib.Models
{
    public class DocSet : IContextualLoad
    {
        private LoadContext _context;

        public DocSet(string uuid, string name, IEnumerable<Doc> docs = null, LoadContext context = null)
        {
            Uuid = uuid;
            Name = name;
            IncludedDocs = docs?.ToList() ?? new();
            Context = context ?? new();
        }

        public string Uuid { get; set; }

        public string Name { get; set; }

        public List<Doc> IncludedDocs { get; }

        public Author Author { get; set; }

        [NotNull]
        public LoadContext Context
        {
            get => _context;
            set
            {
                Guard.IsNotNull(value);
                _context = value;
            }
        }

        public XDocument Serialize()
        {
            XDocument xdoc = new();

            XElement setXml = new("Set");
            setXml.SetAttributeValue(nameof(Uuid), Uuid);
            setXml.SetAttributeValue(nameof(Name), Name);

            if (Author != null)
            {
                XElement authorXml = Author.SerializeElement();
                setXml.Add(authorXml);
            }

            xdoc.Add(setXml);
            return xdoc;
        }

        public static DocSet Deserialize(XDocument xdoc)
        {
            var setXml = xdoc.Root;
            string uuid = setXml.Attribute(nameof(Uuid))?.Value;
            string name = setXml.Attribute(nameof(Name))?.Value;

            DocSet set = new(uuid, name);

            var authorXml = setXml.Elements(nameof(Author)).FirstOrDefault();
            if (authorXml != null)
                set.Author = Author.DeserializeElement(authorXml);

            return set;
        }
    }
}
