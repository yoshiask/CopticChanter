using CoptLib.Models;
using CoptLib.Scripting;
using CoptLib.Writing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
        public static bool SaveDocXml(string filename, IEnumerable<ContentPart> content, string name)
        {
            try
            {
                // Create an instance of the XmlSerializer class;
                // specify the type of object to serialize.
                XmlSerializer serializer = new XmlSerializer(typeof(Doc));
                TextWriter writer = new StreamWriter(new FileStream(filename, FileMode.Create));
                Doc saveX = new()
                {
                    Name = name,
                    Translations = new(null)
                };

                saveX.Translations.AddRange(content);

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

        /// <summary>
        /// Parses the XML document tree into a <see cref="Doc"/>.
        /// </summary>
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
                var defs = ParseDefinitionCollection(defsElem?.Elements(), doc, null);
                doc.DirectDefinitions = defs;
            }

            var transsElem = xml.Root.Element("Translations");
            if (transsElem != null)
            {
                foreach (XElement transElem in transsElem.Elements())
                {
                    Section translation = new(doc.Translations);
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
            RecursiveTransform(doc.DirectDefinitions);
            RecursiveTransform(doc.Translations);
        }

        private static List<IDefinition> ParseDefinitionCollection(IEnumerable<XElement> elements, Doc doc, IDefinition parent)
        {
            List<IDefinition> defs = new();

            foreach (XElement defElem in elements)
            {
                IDefinition def = null;
                string defElemName = defElem.Name.LocalName;

                if (defElemName == nameof(Stanza))
                {
                    def = new Stanza(parent);
                }
                else if (defElemName == nameof(Section))
                {
                    Section section = new(parent);

                    string title = defElem.Attribute("Title")?.Value;
                    if (title != null)
                    {
                        section.Title = new Stanza(section)
                        {
                            SourceText = title
                        };
                    }

                    def = section;
                }
                else if (defElemName == nameof(Script))
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
                else if (defElemName == "Translations")
                {
                    def = new TranslationCollection(parent);
                }

                if (def == null)
                    continue;

                ParseCommonXml(def, defElem, doc, def.Parent);

                if (def is ICollection<IDefinition> defCol)
                {
                    foreach (var subdef in ParseDefinitionCollection(defElem.Elements(), doc, def))
                    {
                        defCol.Add(subdef);

                        if (subdef.Key != null)
                            doc.Definitions.Add(subdef.Key, subdef);
                    }
                }

                if (def.Key != null)
                    doc.Definitions.Add(def.Key, def);
                defs.Add(def);
            }

            return defs;
        }

        private static void ParseCommonXml(object obj, XElement elem, Doc doc, IDefinition parent)
        {
            if (obj is IDefinition def)
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
                // Parse elements, remove anything not a ContentPart
                var defColl = ParseDefinitionCollection(elem.Elements(), doc, defCC)
                    .Select(d => d as ContentPart)
                    .Where(d => d is not null);
                contentCollection.AddRange(defColl);

                contentCollection.Source = elem.Attribute("Source")?.Value;
            }
        }

        internal static void RecursiveTransform(System.Collections.IEnumerable parts)
        {
            foreach (var part in parts)
                Transform(part);
        }

        internal static void Transform(object part)
        {
            if (part is IContent partContent)
                partContent.ParseCommands();
            if (part is IContentCollectionContainer partCollection)
                partCollection.ParseCommands();

            if (part is IMultilingual multilingual)
                multilingual.HandleFont();
        }
    }
}
