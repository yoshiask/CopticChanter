using CoptLib.Models;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace CoptLib.IO
{
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

            root.Add(new XElement("Name", doc.Name));
            root.Add(new XElement("Uuid", doc.Uuid));

            XElement transsElem = new("Translations");
            if (doc.Translations.Source != null)
                transsElem.SetAttributeValue("Source", doc.Translations.Source);
            foreach (ContentPart translation in doc.Translations.Children.Where(IsExplicitlyDefined))
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
        /// <param name="obj">The object from CoptLib.Models to serialize.</param>
        /// <param name="xName">
        /// The name of the XML element. Defaults to the name of
        /// the type of <paramref name="obj"/>.
        /// </param>
        /// <returns>
        /// An XML element representing the object.
        /// </returns>
        public static XElement SerializeObject(object obj, string xName = null)
        {
            xName ??= obj.GetType().Name;
            XElement elem = new(xName);
            IDefinition parent = null;

            if (obj is IDefinition def)
            {
                elem.SetAttributeValue("Key", def.Key);
                parent = def.Parent;
            }
            if (obj is IContent content)
            {
                elem.Value = content.SourceText;
            }
            if (obj is IMultilingual multilingual)
            {
                var parentMultilingual = parent as IMultilingual;
                if (parentMultilingual?.Language != multilingual.Language &&
                    !(parentMultilingual == null && multilingual.Language == Writing.KnownLanguage.Default))
                    elem.SetAttributeValue("Language", multilingual.Language);

                if (parentMultilingual?.Font != multilingual.Font)
                    elem.SetAttributeValue("Font", multilingual.Font);
            }
            if (obj is IContentCollectionContainer contentCollection)
            {
                foreach (var child in contentCollection.Children)
                    elem.Add(SerializeObject(child));

                elem.SetAttributeValue("Source", contentCollection.Source);
            }

            // Serialize class-specific properties
            switch (obj)
            {
                case SimpleContent:
                    elem.Name = "String";
                    break;

                case TranslationCollection:
                    elem.Name = "Translations";
                    break;

                case Section section:
                    elem.SetAttributeValue("Title", section.Title);
                    break;

                case Script script:
                    elem.Add(new XCData(script.LuaScript));
                    break;

                case Variable variable:
                    elem.SetAttributeValue("Label", variable.Label);
                    elem.SetAttributeValue("DefaultValue", variable.DefaultValue);
                    elem.SetAttributeValue("Configurable", variable.Configurable);
                    break;
            }

            return elem;
        }

        private static bool IsExplicitlyDefined(IDefinition def) => def.IsExplicitlyDefined;
    }
}
