using CoptLib.Models;
using CoptLib.Writing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            // The actual content can't be deserialized, so it needs to be manually parsed

            Doc doc = new Doc()
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

                    if (defElem.Name == nameof(Script))
                    {
                        var script = new Script()
                        {
                            LuaScript = defElem.Value
                        };
                        doc.Definitions.Add(script);
                    }
                    else if (defElem.Name == nameof(Variable))
                    {
                        var variable = new Variable()
                        {
                            Label = defElem.Attribute("Label")?.Value,
                            DefaultValue = defElem.Attribute("DefaultValue")?.Value,
                            Configurable = bool.Parse(defElem.Attribute("Configurable")?.Value),
                        };
                        doc.Definitions.Add(variable);
                    }
                    else if (defElem.Name == nameof(Models.String))
                    {
                        var _string = new Models.String()
                        {
                            Value = defElem.Value,
                            Font = defElem.Attribute("Font")?.Value,
                            Language = (Language)Enum.Parse(typeof(Language),
                                defElem.Attribute("Language")?.Value ?? "Default"),
                        };
                        doc.Definitions.Add(_string);
                    }

                    if (def != null)
                    {
                        def.DocContext = doc;
                        def.Key = defElem.Attribute("Key")?.Value;
                    }
                }
            }

            foreach (XElement transElem in xml.Root.Element("Translations").Elements())
            {
                Translation translation = new Translation()
                {
                    Font = transElem.Attribute("Font")?.Value,
                    Language = (Language)Enum.Parse(typeof(Language),
                        transElem.Attribute("Language")?.Value),
                };
                translation.Content = ParseContentParts(transElem.Elements(), translation, doc);
                doc.Translations.Add(translation);
            }

            return doc;
        }

        private static List<ContentPart> ParseContentParts(IEnumerable<XElement> elements, Translation translation, Doc doc)
        {
            var content = new List<ContentPart>(elements.Count());

            foreach (XElement contentElem in elements)
            {
                if (contentElem.Name == nameof(Stanza))
                {
                    var stanza = new Stanza(translation)
                    {
                        Language = (Language)Enum.Parse(typeof(Language),
                            contentElem.Attribute("Language")?.Value ?? translation.Language.ToString()),
                        Key = contentElem.Attribute("Key")?.Value
                    };

                    if (stanza.Language == Language.Coptic && translation.Font != null)
                    {
                        // Coptic text needs to be interpreted before it can be displayed
                        var font = CopticFont.FindFont(translation.Font);
                        stanza.SourceText = font.Convert(contentElem.Value, font);
                        stanza.DocContext = doc;
                    }
                    else
                    {
                        stanza.SourceText = contentElem.Value;
                    }

                    stanza.ParseCommands();

                    content.Add(stanza);
                }
                else if (contentElem.Name == nameof(Section))
                {
                    var section = new Section(translation)
                    {
                        Key = contentElem.Attribute("Key")?.Value,
                        Title = ResolveReference(contentElem.Attribute("Title")?.Value, doc),
                        Content = ParseContentParts(contentElem.Elements(), translation, doc)
                    };
                    content.Add(section);
                }
            }

            return content;
        }

        public static string ResolveReference(string valueString, Doc doc)
        {
            if (valueString.StartsWith("{") && valueString.EndsWith("}"))
            {
                string[] parts = valueString.Substring(1, valueString.Length - 2).Split(' ');
                if (parts.Length < 2)
                {
                    return valueString;
                }
                else
                {
                    // Find the element with the given key in the doc's definitions
                    Models.String refValue = doc.Definitions.Find(def => def.Key == parts[1] && def is Models.String) as Models.String;
                    if (refValue != null)
                    {
                        if (refValue.Language == Language.Coptic && refValue.Font != null)
                        {
                            // Coptic text needs to be interpreted before it can be displayed
                            var font = CopticFont.FindFont(refValue.Font);
                            return font.Convert(refValue.Value);
                        }
                        else
                        {
                            return refValue.Value;
                        }
                    }
                    else
                    {
                        return valueString;
                    }
                }
            }
            else
            {
                return valueString;
            }
        }

        /// <summary>
        /// Unzips, serializes, and returns an Index and list of Docs
        /// </summary>
        /// <param name="path">Path to zip file</param>
        /// <returns></returns>
        public static DocSetReader.ReadResults ReadSet(string path, string tempPath)
        {
            return new DocSetReader(Path.GetFileNameWithoutExtension(path), path).Read(tempPath);
        }
        /// <summary>
        /// Unzips, serializes, and returns an Index and list of Docs
        /// </summary>
        /// <param name="File">Stream of zip file</param>
        /// <returns></returns>
        public static DocSetReader.ReadResults ReadSet(Stream file, string name, string tempPath)
        {
            return (new DocSetReader(name, file)).Read(tempPath);
        }

        /// <summary>
        /// Serializes and zips an Index and included docs
        /// </summary>
        /// <param name="filename">Name of file to be saved</param>
        /// <param name="setname">Name of set to be saved</param>
        /// <param name="setUuid">Generated UUID of set</param>
        /// <param name="incdocs">Docs to include in set</param>
        public static void SaveSet(string filename, string setname, string setUuid, IEnumerable<string> incdocs)
        {
            var setX = new Index()
            {
                Name = setname,
                Uuid = setUuid
            };
            List<string> incDocs = new List<string>();

            foreach (string docUuid in incdocs)
            {
                setX.IncludedDocs.Add(AllDocs[docUuid].ToIndexDocXml());
            }

            new DocSetWriter(setX).Write(filename);
        }
    }
}
