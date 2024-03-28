using CoptLib.Models;
using CoptLib.Models.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using CoptLib.Extensions;
using CoptLib.Scripting;
using CoptLib.Scripting.Typed;
using CoptLib.Writing;

namespace CoptLib.IO;

public static class DocWriter
{
    /// <summary>
    /// Serializes <paramref name="doc"/> as an XML string.
    /// </summary>
    /// <param name="doc">
    /// The document to serialize.
    /// </param>
    public static string WriteDocXml(Doc doc)
    {
        XDocument xdoc = SerializeDocXml(doc);

        using StringWriter writer = new();
        using XmlTextWriter xmlWriter = new(writer);
        xmlWriter.Formatting = Formatting.Indented;
        xmlWriter.Indentation = 4;
        xmlWriter.IndentChar = ' ';

        xdoc.Save(xmlWriter);
        return writer.ToString();
    }

    /// <summary>
    /// Serializes <paramref name="doc"/> to the provided stream.
    /// </summary>
    /// <param name="doc">
    /// The document to serialize.
    /// </param>
    /// <param name="stream">
    /// The stream to write to.
    /// </param>
    public static void WriteDocXml(Doc doc, Stream stream)
    {
        XDocument xdoc = SerializeDocXml(doc);

        using StringWriter writer = new();
        using XmlTextWriter xmlWriter = new(stream, Encoding.Unicode);
        xmlWriter.Formatting = Formatting.Indented;
        xmlWriter.Indentation = 4;
        xmlWriter.IndentChar = ' ';

        xdoc.Save(xmlWriter);
    }

    /// <summary>
    /// Serializes a document to XML.
    /// </summary>
    /// <param name="doc">The document to serialize.</param>
    public static XDocument SerializeDocXml(Doc doc)
    {
        // The actual content can't be directly serialized, so it needs to be done manually
        XDocument xdoc = new();
        XElement root = new("Document");

        root.Add(new XElement(nameof(doc.Name), doc.Name));
        root.Add(new XElement(nameof(doc.Key), doc.Key));

        XElement transsElem = new(nameof(doc.Translations));
        foreach (var translation in doc.Translations.Children.Where(IsExplicitlyDefined))
            transsElem.Add(SerializeObject(translation, "Translation"));

        // Omit Translations element if it doesn't have important data
        if (transsElem.HasElements || transsElem.HasAttributes)
            root.Add(transsElem);

        // Add all explicitly defined document definitions to the XML tree
        XElement defsElem = new("Definitions");
        foreach (var docDef in doc.DirectDefinitions.Where(IsExplicitlyDefined))
            defsElem.Add(SerializeObject(docDef));

        // Omit Definitions element if there aren't any direct definitions
        if (defsElem.HasElements)
            root.Add(defsElem);

        xdoc.Add(root);
        return xdoc;
    }

    /// <summary>
    /// Serializes a CoptLib object to XML.
    /// </summary>
    /// <param name="def">The object from CoptLib.Models to serialize.</param>
    /// <param name="name">
    /// The name of the XML element. Defaults to the name of
    /// the type of <paramref name="def"/>.
    /// </param>
    /// <returns>
    /// An XML element representing the object.
    /// </returns>
    public static XElement SerializeObject(IDefinition def, XName? name = null)
    {
        name ??= def.GetType().Name;
        XElement elem = new(name);
        elem.SetAttributeValue(nameof(def.Key), def.Key);

        if (def is IContent content)
        {
            elem.Value = content.SourceText;
        }
        if (def is IMultilingual multilingual)
        {
            var elemLanguage = multilingual.Language;
            var elemFont = multilingual.Font;

            if (def.Parent is not null)
            {
                var parentLanguage = def.Parent.GetLanguage();
                if (!LanguageInfo.IsNullOrDefault(parentLanguage) && parentLanguage == elemLanguage)
                    elemLanguage = null;
                    
                var parentFont = def.Parent.GetFont();
                if (parentFont is not null && parentFont == elemFont)
                    elemFont = null;
            }
                
            if (elemLanguage is not null)
                elem.SetAttributeValue(nameof(multilingual.Language), multilingual.Language);

            if (elemFont is not null)
                elem.SetAttributeValue(nameof(multilingual.Font), multilingual.Font);
        }
        if (def is IContentCollectionContainer contentCollection)
            foreach (var child in contentCollection.Children)
                elem.Add(SerializeObject(child));

        // Serialize class-specific properties
        switch (def)
        {
            case SimpleContent:
                elem.Name = "String";
                break;

            case TranslationCollection:
            case TranslationCollectionSection:
            case TranslationRunCollection:
                elem.Name = "Translations";
                break;

            case Section section:
                elem.SetAttributeValue(nameof(section.Title), section.Title);
                break;

            case IScript<object> script:
                elem.Name = "Script";
                elem.Add(new XCData(script.ScriptBody));
                elem.SetAttributeValue(nameof(script.TypeId), script.TypeId);
                break;

            case PartReference partRef:
                elem.Name = "Reference";
                elem.Add(partRef.Source?.SourceText);
                break;

            case Variable variable:
                elem.SetAttributeValue(nameof(variable.Label), variable.Label);
                elem.SetAttributeValue(nameof(variable.DefaultValue), variable.DefaultValue);
                elem.SetAttributeValue(nameof(variable.Configurable), variable.Configurable);
                break;
        }

        return elem;
    }
    
    /// <summary>
    /// Serializes a CoptLib object to XML while keeping the applied transforms.
    /// </summary>
    /// <param name="def">The object from CoptLib.Models to serialize.</param>
    /// <param name="name">
    /// The name of the XML element. Defaults to the name of
    /// the type of <paramref name="def"/>.
    /// </param>
    /// <returns>
    /// An XML element representing the object.
    /// </returns>
    public static XElement SerializeTransformedObject(IDefinition def, XName? name = null)
    {
        name ??= def.GetType().Name;
        XElement elem = new(name);
        elem.SetAttributeValue(nameof(def.Key), def.Key);

        if (def is Doc doc)
        {
            elem.SetAttributeValue(nameof(doc.Name), doc.Name);
        }
        if (def is IContent content)
        {
            elem.Value = content.ToString();
        }
        if (def is IMultilingual)
        {
            var elemLanguage = def.GetLanguage();
            var elemFont = def.GetFont();
                
            elem.SetAttributeValue(nameof(IMultilingual.Language), elemLanguage);
            elem.SetAttributeValue(nameof(IMultilingual.Font), elemFont);
        }

        // Serialize class-specific properties
        switch (def)
        {
            case SimpleContent:
                elem.Name = "String";
                break;

            case TranslationCollection:
            case TranslationCollectionSection:
            case TranslationRunCollection:
                elem.Name = "Translations";
                break;

            case Section section:
                elem.SetAttributeValue(nameof(section.Title), section.Title);
                break;

            case IScript<object> script:
                elem.Name = "Script";
                elem.Add(new XCData(script.ScriptBody));
                break;

            case Variable variable:
                elem.SetAttributeValue(nameof(variable.Label), variable.Label);
                elem.SetAttributeValue(nameof(variable.DefaultValue), variable.DefaultValue);
                elem.SetAttributeValue(nameof(variable.Configurable), variable.Configurable);
                break;
        }

        return elem;
    }

    private static bool IsExplicitlyDefined(IDefinition def) => def.IsExplicitlyDefined;
}