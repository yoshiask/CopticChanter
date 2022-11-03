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
                // A FileStream is needed to read the XML document.
                var sr = new StreamReader(path);
                return ParseDocXml(sr.ReadToEnd());
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
                var sr = new StreamReader(file);
                return ParseDocXml(sr.ReadToEnd());
            }
            catch (Exception ex)
            {
                Output.WriteLine(ex.Message);
                return null;
            }
        }

        public static Doc ParseDocXml(string xmlText)
        {
            var xml = XDocument.Parse(xmlText);

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
                            Font = defElem.Attribute("Font")?.Value,
                            Language = (Language)Enum.Parse(typeof(Language),
                                defElem.Attribute("Language")?.Value ?? "Default"),
                        };
                        def = _string;
                    }

                    if (def != null)
                    {
                        def.Key = defElem.Attribute("Key")?.Value;
                        def.DocContext = doc;
                        doc.Definitions.Add(def);
                    }
                }
            }

            var transsElem = xml.Root.Element("Translations");
            if (transsElem != null)
            {
                foreach (XElement transElem in transsElem.Elements())
                {
                    Translation translation = new()
                    {
                        Font = transElem.Attribute("Font")?.Value,
                        Language = (Language)Enum.Parse(typeof(Language),
                            transElem.Attribute("Language")?.Value),
                        Parent = doc,
                        DocContext = doc
                    };

                    translation.Content = ParseContentParts(transElem.Elements(), translation);
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
                TransformContentParts(translation.Content, translation);
            }
        }

        private static List<ContentPart> ParseContentParts(IEnumerable<XElement> elements, Translation translation)
        {
            var doc = translation.DocContext;
            var content = new List<ContentPart>(elements.Count());

            foreach (XElement contentElem in elements)
            {
                ContentPart part = null;

                // TODO: Use reflection and interfaces to reduce code duplication?
                if (contentElem.Name == nameof(Stanza))
                {
                    part = new Stanza(translation)
                    {
                        Language = (Language)Enum.Parse(typeof(Language),
                            contentElem.Attribute("Language")?.Value ?? translation.Language.ToString()),
                        Font = contentElem.Attribute("Font")?.Value ?? translation.Font,
                        Key = contentElem.Attribute("Key")?.Value,
                        SourceText = contentElem.Value
                    };
                }
                else if (contentElem.Name == nameof(Section))
                {
                    part = new Section(translation)
                    {
                        Key = contentElem.Attribute("Key")?.Value,
                        Title = contentElem.Attribute("Title")?.Value,
                        Content = ParseContentParts(contentElem.Elements(), translation)
                    };
                }

                if (part != null)
                {
                    part.DocContext = doc;
                    content.Add(part);
                }
            }

            return content;
        }

        private static void TransformContentParts(IEnumerable<ContentPart> parts, Translation translation)
        {
            var doc = translation.Parent;

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
