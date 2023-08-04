#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace CoptLib.Extensions;

public static class LinqToXmlExtensions
{
    public static XElement? ElementLocal(this XContainer container, string localName)
        => container.ElementsLocal(localName).FirstOrDefault();

    public static IEnumerable<XElement> ElementsLocal(this XContainer container, string localName)
        => container.Elements().Where(e => e.Name.LocalName == localName);

    public static IEnumerable<XAttribute> AttributesLocal(this XElement element, string localName)
        => element.Attributes().Where(e => e.Name.LocalName == localName);

    public static XAttribute? AttributeLocal(this XElement element, string localName)
        => element.AttributesLocal(localName).FirstOrDefault();
}
