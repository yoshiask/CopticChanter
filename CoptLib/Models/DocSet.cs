using CommunityToolkit.Diagnostics;
using CoptLib.IO;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Linq;

namespace CoptLib.Models;

public class DocSet : IContextualLoad
{
    private LoadContextBase _context;

    public DocSet(string key, string name, IEnumerable<Doc>? docs = null, LoadContextBase? context = null)
    {
        Key = key;
        Name = name;
        IncludedDocs = docs?.ToList() ?? new();
        _context = context ?? new LoadContext();
    }

    public string? Key { get; set; }

    public string Name { get; set; }

    public List<Doc> IncludedDocs { get; }

    public Author? Author { get; set; }

    [NotNull]
    public LoadContextBase? Context
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
        setXml.SetAttributeValue(nameof(Key), Key);
        setXml.SetAttributeValue(nameof(Name), Name);

        if (Author != null)
        {
            XElement authorXml = Author.SerializeElement();
            setXml.Add(authorXml);
        }

        xdoc.Add(setXml);
        return xdoc;
    }

    public static DocSet Deserialize(XDocument xdoc, LoadContextBase? context = null)
    {
        var setXml = xdoc.Root;
        Guard.IsNotNull(setXml);

        // BACKCOMPAT: Support documents that use the Uuid element name
        string uuid = (setXml.Attribute(nameof(Key)) ?? setXml.Attribute("Uuid"))?.Value!;
        string name = setXml.Attribute(nameof(Name))?.Value!;

        DocSet set = new(uuid, name, context: context);

        var authorXml = setXml.Elements(nameof(Author)).FirstOrDefault();
        if (authorXml != null)
            set.Author = Author.DeserializeElement(authorXml);

        return set;
    }
}