using CoptLib.Models;
using CoptLib.Scripting;
using CoptLib.Writing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Xml.Linq;
using System.Xml.Serialization;

#if DEBUG
using Output = System.Diagnostics.Debug;
#else
using Output = System.Console;
#endif

namespace CoptLib.IO
{
    public static class DocReader
    {
        public static IDictionary<string, Doc> AllDocs = new Dictionary<string, Doc>();

        /// <summary>
        /// Serializes and save a DocXML file
        /// </summary>
        /// <param name="filename">Path to save to, including filename</param>
        /// <param name="content">List of stanzas to save</param>
        /// <param name="coptic">If the input language is Coptic</param>
        /// <param name="name">Name of the reading or hymn</param>
        /// <returns></returns>
        public static bool SaveDocXml(string filename, IEnumerable<Translation> content, string name)
        {
            try
            {
                // Create an instance of the XmlSerializer class;
                // specify the type of object to serialize.
                XmlSerializer serializer = new XmlSerializer(typeof(Doc));
                TextWriter writer = new StreamWriter(new FileStream(filename, FileMode.Create));
                Doc saveX = new Doc();

                saveX.Translations = new List<Translation>(content);
                saveX.Name = name;

                // Serialize the save file, and close the TextWriter.
                serializer.Serialize(writer, saveX);
                writer.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Deserializes and returns a DocXML file. Only use in full .NET Framework
        /// </summary>
        /// <param name="path">The path to the XML file</param>
        /// <returns></returns>
        public static Doc ReadDocXml(string path)
        {
            try
            {
                return ParseDocXml(XDocument.Load(path));
            }
            catch (Exception ex)
            {
                Output.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Deserializes and returns a DocXML file. For use in UWP
        /// </summary>
        /// <param name="file">A Stream of the XML file</param>
        /// <returns></returns>
        public static Doc ReadDocXml(Stream file)
        {
            try
            {
                return ParseDocXml(XDocument.Load(file, LoadOptions.None));
            }
            catch (Exception ex)
            {
                Output.WriteLine(ex.Message);
                return null;
            }
        }

        public static Doc ParseDocXml(XDocument xml)
        {
            // The actual content can't be directly deserialized, so it needs to be manually parsed
            Doc doc = new()
            {
                Name = xml.Root.Element("Name")?.Value,
                Uuid = xml.Root.Element("Uuid")?.Value,
            };

            var defsElem = xml.Root.Element("Definitions");
            if (defsElem != null)
            {
                foreach (XElement defElem in defsElem?.Elements())
                {
                    Definition def = null;
                    string defElemName = defElem.Name.LocalName;

                    if (defElemName == nameof(Script))
                    {
                        Script script = new()
                        {
                            LuaScript = defElem.Value
                        };
                        def = script;
                    }
                    else if (defElemName == nameof(Variable))
                    {
                        Variable variable = new()
                        {
                            Label = defElem.Attribute("Label")?.Value,
                            DefaultValue = defElem.Attribute("DefaultValue")?.Value,
                            Configurable = bool.Parse(defElem.Attribute("Configurable")?.Value),
                        };
                        def = variable;
                    }
                    else if (defElemName == nameof(Models.String))
                    {
                        Models.String _string = new()
                        {
                            Value = defElem.Value,
                        };
                        def = _string;
                    }

                    if (def != null)
                    {
                        ParseCommonXml(def, defElem, doc, null);
                        doc.Definitions.Add(def);
                    }
                }
            }

            var transsElem = xml.Root.Element("Translations");
            if (transsElem != null)
            {
                foreach (XElement transElem in transsElem.Elements())
                {
                    Translation translation = new();
                    ParseCommonXml(translation, transElem, doc, null);
                    doc.Translations.Add(translation);
                } 
            }

            return doc;
        }

        /// <summary>
        /// Parses text commands and applies font conversions in the given document.
        /// </summary>
        /// <param name="doc">The document to apply all transforms on.</param>
        public static void ApplyDocTransforms(Doc doc)
        {
            foreach (Definition def in doc.Definitions)
            {
                if (def is IContent defContent)
                    defContent.ParseCommands();
                if (def is IMultilingual multilingual)
                    multilingual.HandleFont();
            }

            foreach (Translation translation in doc.Translations)
            {
                TransformContentParts(translation.Content, doc);
            }
        }

        private static List<ContentPart> ParseContentParts(IEnumerable<XElement> elements, Doc doc, Definition parent)
        {
            List<ContentPart> content = new();

            foreach (XElement contentElem in elements)
            {
                ContentPart part = null;

                // TODO: Use reflection and interfaces to reduce code duplication?
                if (contentElem.Name == nameof(Stanza))
                {
                    part = new Stanza(parent);
                }
                else if (contentElem.Name == nameof(Section))
                {
                    part = new Section(parent)
                    {
                        Title = contentElem.Attribute("Title")?.Value,
                    };
                }

                if (part != null)
                {
                    ParseCommonXml(part, contentElem, doc, parent);
                    content.Add(part);
                }
            }

            return content;
        }

        private static void TransformContentParts(IEnumerable<ContentPart> parts, Doc doc)
        {
            foreach (ContentPart part in parts)
            {
                if (part is Section section)
                {
                    if (section.Title != null)
                    {
                        _ = Scripting.Scripting.ParseTextCommands(section.Title, doc, out var title);
                        section.Title = title;
                    }
                }

                RecursiveParseCommands(part);
                if (part is IMultilingual multilingual)
                    multilingual.HandleFont();
            }
        }

        private static void ParseCommonXml(object obj, XElement elem, Doc doc, Definition parent)
        {
            if (obj is Definition def)
            {
                def.DocContext = doc;
                def.Parent = parent;
                def.Key = elem.Attribute("Key")?.Value;
            }
            if (obj is IContent content)
            {
                content.SourceText = elem.Value;
            }
            if (obj is IMultilingual multilingual)
            {
                multilingual.Font = elem.Attribute("Font")?.Value;
                if (Enum.TryParse<Language>(elem.Attribute("Language")?.Value, out var lang))
                    multilingual.Language = lang;

                if (parent is IMultilingual parentMultilingual)
                {
                    multilingual.Font ??= parentMultilingual.Font;
                    if (multilingual.Language == Language.Default)
                        multilingual.Language = parentMultilingual.Language;
                }
            }
            if (obj is IContentCollectionContainer contentCollection && obj is Definition defCC)
            {
                contentCollection.Content = ParseContentParts(elem.Elements(), doc, defCC);
                contentCollection.Source = elem.Attribute("Source")?.Value;

                // Handle Source definition
                if (contentCollection.Source != null)
                {

                }
            }
        }

        internal static void RecursiveParseCommands(IEnumerable<ContentPart> parts)
        {
            foreach (ContentPart part in parts)
                RecursiveParseCommands(part);
        }

        internal static void RecursiveParseCommands(ContentPart part)
        {
            if (part is IContent partContent)
                partContent.ParseCommands();
            else if (part is IContentCollectionContainer partCollection)
                partCollection.ParseCommands();
        }
    }
}
