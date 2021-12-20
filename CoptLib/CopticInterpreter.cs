using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using CoptLib.Models;
#if DEBUG
using Output = System.Diagnostics.Debug;
#else
using Output = System.Console;
#endif

namespace CoptLib
{
    public enum Language
    {
        Default,
        English,
        Coptic,
        Arabic,
        Greek,
        Spanish,
        Amharic,
        Armenian
    }

    public static class CopticInterpreter
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
                    if (defElem.Name == nameof(Script))
                    {
                        var script = new Script()
                        {
                            LuaScript = defElem.Value,
                            Key = defElem.Attribute("Key")?.Value
                        };
                        doc.Definitions.Add(script);
                    }
                    else if (defElem.Name == nameof(Variable))
                    {
                        var variable = new Variable()
                        {
                            Label = defElem.Attribute("Label")?.Value,
                            DefaultValue = defElem.Attribute("DefaultValue")?.Value,
                            Configurable = Boolean.Parse(defElem.Attribute("Configurable")?.Value),
                            Key = defElem.Attribute("Key")?.Value
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
                            Key = defElem.Attribute("Key")?.Value
                        };
                        doc.Definitions.Add(_string);
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
                    var stanza = new Stanza()
                    {
                        Language = (Language)Enum.Parse(typeof(Language),
                            contentElem.Attribute("Language")?.Value ?? translation.Language.ToString()),
                        Key = contentElem.Attribute("Key")?.Value
                    };

                    if (stanza.Language == Language.Coptic && translation.Font != null)
                    {
                        // Coptic text needs to be interpreted before it can be displayed
                        var font = CopticFont.Fonts.Find(f => f.Name.ToLower() == translation.Font.ToLower());
                        stanza.Text = ConvertFont(contentElem.Value, font, CopticFont.CopticUnicode);
                    }
                    else
                    {
                        stanza.Text = contentElem.Value;
                    }

                    content.Add(stanza);
                }
                else if (contentElem.Name == nameof(Section))
                {
                    var section = new Section
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
                            var font = CopticFont.Fonts.Find(f => f.Name.ToLower() == refValue.Font.ToLower());
                            return ConvertFont(refValue.Value, font, CopticFont.CopticUnicode);
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

        public static string ConvertFont(string input, CopticFont start, CopticFont target)
        {
            string output = "";

            // Generate a dictionary that has the start mapping as keys and the target mapping as values
            Dictionary<string, string> mergedMap = new Dictionary<string, string>();
            var sourceMap = DictionaryTools.SwitchColumns(start.Charmap);
            foreach (KeyValuePair<string, string> spair in sourceMap)
            {
                if (target.Charmap.ContainsKey(spair.Value))
                {
                    var targetChar = target.Charmap[spair.Value];
                    mergedMap.Add(spair.Key, targetChar);
                }
            }

            /*foreach (KeyValuePair<string, string> tpair in target.Charmap)
            {
                if (!mergedMap.ContainsValue(tpair.Key))
                {
                    var targetChar = target.Charmap[tpair.Key];
                    mergedMap.Add(tpair.Key, targetChar);
                }
            }*/

            if (start.IsJenkimBefore)
            {
                bool accent = false;
                foreach (char ch in input)
                {
                    if (mergedMap.ContainsKey(ch.ToString()))
                    {
                        // Find the source character in the target mapping and write it
                        if (ch == start.JenkimCharacter)
                        {
                            // The character is an accent; save that for later
                            accent = true;
                        }
                        else
                        {
                            output += mergedMap[ch.ToString()];
                            if (accent)
                            {
                                // It's time to write the accent
                                output += target.JenkimCharacter;
                                accent = false;
                            }
                        }
                    }
                    else
                    {
                        // Character is not in the character map, so leave it be
                        output += ch.ToString();
                        if (accent)
                        {
                            output += target.JenkimCharacter;
                            accent = false;
                        }
                    }
                }
                //return output;
            }
            else
            {
                bool accent = false;
                foreach (char ch in input)
                {
                    if (mergedMap.ContainsKey(ch.ToString()))
                    {
                        if (ch == start.JenkimCharacter)
                        {
                            accent = true;
                        }
                        else
                        {
                            output += mergedMap[ch.ToString()];
                            if (accent)
                            {
                                output += target.JenkimCharacter;
                                accent = false;
                            }
                        }
                    }
                    else
                    {
                        output += ch.ToString();
                        if (accent)
                        {
                            output += target.JenkimCharacter;
                            accent = false;
                        }
                    }
                }
                //return output;
            }

            return output;
        }
    }

    public class DocSetReader
    {
        private string ZipPath
        {
            get;
        }
        private Stream ZipStream
        {
            get;
        }
        public string Name
        {
            get;
        }

        public DocSetReader(string name, string zipPath)
        {
            Name = name;
            ZipPath = zipPath;
        }
        public DocSetReader(string name, Stream zipStream)
        {
            Name = name;
            ZipStream = zipStream;
        }

        /// <param name="tempPath">Path.Combine(
        /// Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        ///         "Coptic Chanter", "Doc Creator", "temp")</param>
        /// <returns></returns>
        public ReadResults Read(string tempPath)
        {
            string folderPath = Path.Combine(tempPath, Name);
            if (Directory.Exists(folderPath))
            {
                foreach (string file in Directory.EnumerateFiles(folderPath))
                {
                    File.Delete(file);
                }
                Directory.Delete(folderPath);
            }
            Directory.CreateDirectory(folderPath);

            bool isSuccess = ZipStream == null ? UnzipFile(ZipPath, folderPath) : UnzipFile(ZipStream, folderPath);
            if (isSuccess)
            {
                var results = new ReadResults();
                List<string> files = Directory.EnumerateFiles(folderPath).ToList();
                if (files.Contains(Path.Combine(folderPath, "index.xml")))
                {
                    // Create an instance of the XmlSerializer class;
                    // specify the type of object to be deserialized.
                    XmlSerializer serializer = new XmlSerializer(typeof(Index));

                    // A FileStream is needed to read the XML document.
                    string text = File.ReadAllText(Path.Combine(folderPath, "index.xml"));

                    //Use the Deserialize method to restore the object's state with
                    //data from the XML document.
                    results.Index = (Index)serializer.Deserialize(XDocument.Parse(text).CreateReader());

                    foreach (string filename in files)
                    {
                        if (filename != Path.Combine(folderPath, "index.xml") && !filename.EndsWith(".zip"))
                        {
                            try
                            {
                                var doc = CopticInterpreter.ReadDocXml(filename);
                                results.IncludedDocs.Add(doc);

                                /*// Create an instance of the XmlSerializer class;
                                // specify the type of object to be deserialized.
                                XmlSerializer serializerDoc = new XmlSerializer(typeof(DocXML));

                                // A FileStream is needed to read the XML document.
                                string textDoc = File.ReadAllText(filename);

                                //Use the Deserialize method to restore the object's state with
                                //data from the XML document.
                                var readerDoc = XDocument.Parse(textDoc).CreateReader();
                                var serialDoc = (DocXML)serializerDoc.Deserialize(readerDoc);
                                results.IncludedDocs.Add(serialDoc);*/
                            }
                            catch (Exception ex)
                            {
                                Output.WriteLine("Error: ", ex.Message);
                                Output.WriteLine("Unexpected file in set");
                            }
                        }
                    }

                    return results;
                }
                else
                {
                    Output.WriteLine("Set file not valid: No index found");
                }
            }
            return null;
        }

        /// <summary>
        /// Extracts a zip file in the specified directory
        /// </summary>
        /// <param name="zipPath">Zip file to extract</param>
        /// <param name="extractPath">Path to extract zip file to</param>
        private bool UnzipFile(string zipPath, string extractPath)
        {
            try
            {
                ZipFile.ExtractToDirectory(zipPath, extractPath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Extracts a zip file in the specified directory
        /// </summary>
        /// <param name="zipStream">Zip stream to extract</param>
        /// <param name="extractPath">Path to extract zip file to</param>
        private bool UnzipFile(Stream zipStream, string extractPath)
        {
            try
            {
                new ZipArchive(zipStream).ExtractToDirectory(extractPath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public class ReadResults
        {
            public Index Index;
            public List<Doc> IncludedDocs = new List<Doc>();
        }
    }

    public class DocSetWriter
    {
        /// <summary>
        /// An Index XML object that contains data to write to set
        /// </summary>
        public Index Index
        {
            get;
            private set;
        }

        public DocSetWriter(Index index = null)
        {
            Index = index;
        }

        /// <summary>
        /// Writes the data written to Index and Docs variable
        /// </summary>
        /// <returns></returns>
        public bool Write(string path)
        {
            if (Index != null)
            {
                string rootPath = path.Replace(".zip", "");
                Directory.CreateDirectory(rootPath);

                // Let's sort out the documents first
                foreach (IndexDoc indexDoc in Index.IncludedDocs)
                {
                    XmlSerializer docSerializer = new XmlSerializer(typeof(Doc));
                    TextWriter docWriter = new StreamWriter(new FileStream(Path.Combine(rootPath, indexDoc.Name + ".xml"), FileMode.Create));
                    docSerializer.Serialize(docWriter, CopticInterpreter.AllDocs[indexDoc.Uuid]);
                    docWriter.Dispose();
                }

                // Now save the index
                XmlSerializer serializer = new XmlSerializer(typeof(Index));
                TextWriter writer = new StreamWriter(new FileStream(Path.Combine(rootPath, "index.xml"), FileMode.Create));
                serializer.Serialize(writer, Index);
                writer.Dispose();

                // Files are saved, now compess to zip
                ZipFile(rootPath, Path.Combine(Path.GetDirectoryName(rootPath), Index.Name + ".zip"));

                // Delete temp. directory
                foreach (string filepath in Directory.GetFiles(rootPath))
                {
                    File.Delete(filepath);
                }
                Directory.Delete(rootPath);

                return true;
            }
            else
            {
                Output.WriteLine("Index or Content is null");
                return false;
            }
        }

        public void AddContent(Doc xml)
        {
            Index.IncludedDocs.Add(new IndexDoc()
            {
                Name = xml.Name,
                Uuid = xml.Uuid,
            });
        }

        public void AddContent(IEnumerable<Doc> docs)
        {
            foreach (Doc xml in docs)
            {
                Index.IncludedDocs.Add(new IndexDoc()
                {
                    Name = xml.Name,
                    Uuid = xml.Uuid
                });
            }
        }

        public void ClearContent()
        {
            Index.IncludedDocs.Clear();
        }

        public void SetIndex(Index index)
        {
            Index = index;
        }

        public void ClearIndex()
        {
            Index = null;
        }

        /// <summary>
        /// Creates a zip file in the specified directory
        /// </summary>
        /// <param name="path">Folder to compress</param>
        /// <param name="zipPath">File to write to</param>
        private void ZipFile(string path, string zipPath)
        {
            if (File.Exists(zipPath))
                File.Delete(zipPath);
            System.IO.Compression.ZipFile.CreateFromDirectory(path, zipPath);
        }
    }

    public class CopticFont
    {
        public string Name;
        public string FontName;
        public Dictionary<string, string> Charmap = new Dictionary<string, string>();
        public char JenkimCharacter;
        public bool IsJenkimBefore = true;
        public bool IsCopticStandard = true;

        public static CopticFont CsAvvaShenouda =>
            new CopticFont()
            {
                Name = "CS Avva Shenouda",
                FontName = "CS Avva Shenouda",
                IsCopticStandard = true,
                IsJenkimBefore = true,
                Charmap = InitCopticStandard()
            };

        public static CopticFont CsCopt =>
            new CopticFont()
            {
                Name = "CS Copt",
                FontName = "CS Copt",
                IsCopticStandard = true,
                IsJenkimBefore = true,
                Charmap = InitCopticStandard()
            };

        public static CopticFont CsCopticManuscript =>
            new CopticFont()
            {
                Name = "CS Coptic Manuscript",
                FontName = "CS Coptic Manuscript",
                IsCopticStandard = true,
                IsJenkimBefore = true,
                Charmap = InitCopticStandard()
            };

        public static CopticFont CsCoptoManuscript =>
            new CopticFont()
            {
                Name = "CS Copto Manuscript",
                FontName = "CS Copto Manuscript",
                IsCopticStandard = true,
                IsJenkimBefore = true,
                Charmap = InitCopticStandard()
            };

        public static CopticFont CsKoptosManuscript =>
            new CopticFont()
            {
                Name = "CS Koptos Manuscript",
                FontName = "CS Koptos Manuscript",
                IsCopticStandard = true,
                IsJenkimBefore = true,
                Charmap = InitCopticStandard()
            };

        public static CopticFont CsNewAthanasius =>
            new CopticFont()
            {
                Name = "CS New Athanasius",
                FontName = "CS New Athanasius",
                IsCopticStandard = true,
                IsJenkimBefore = true,
                Charmap = InitCopticStandard()
            };

        public static CopticFont CsPishoi =>
            new CopticFont()
            {
                Name = "CS Pishoi",
                FontName = "CS Pishoi",
                IsCopticStandard = true,
                IsJenkimBefore = true,
                Charmap = InitCopticStandard()
            };

        public static CopticFont CopticUnicode =>
            new CopticFont()
            {
                Name = "Coptic Unicode",
                FontName = "Segoe UI",
                IsCopticStandard = false,
                IsJenkimBefore = false,
                Charmap = InitCopticUnicode()
            };

        public static CopticFont Coptic1 =>
            new CopticFont()
            {
                Name = "Coptic1",
                FontName = "Coptic1",
                IsCopticStandard = false,
                IsJenkimBefore = false,
                Charmap = new Dictionary<string, string>()
            };

        public static CopticFont Athanasius =>
            new CopticFont()
            {
                Name = "Athanasius",
                FontName = "Athanasius Plain",
                IsCopticStandard = false,
                IsJenkimBefore = true,
                Charmap = InitAthanasuis()
            };

        public static CopticFont GreekUnicode =>
            new CopticFont()
            {
                Name = "Greek Unicode",
                FontName = "Segoe UI",
                IsCopticStandard = false,
                IsJenkimBefore = false,
                Charmap = InitGreekUnicode()
            };

        public static List<CopticFont> Fonts = new List<CopticFont>()
        {
            CsAvvaShenouda, CsCopt, CsCopticManuscript, CsCoptoManuscript,
            CsKoptosManuscript, CsNewAthanasius, CsPishoi,
            CopticUnicode, Coptic1, Athanasius,
            GreekUnicode
        };
        private static Dictionary<string, string> InitCopticStandard()
        {
            Dictionary<string, string> alphabet = new Dictionary<string, string>
            {
                // Key is Coptic Standard
                // Value is Coptic Standard

                #region Uppercase
                { "A", "A" },
                { "B", "B" },
                { "G", "G" },
                { "D", "D" },
                { "E", "E" },
                { "^", "^" },
                { "Z", "Z" },
                { "Y", "Y" },
                { ":", ":" },
                { "I", "I" },
                { "K", "K" },
                { "L", "L" },
                { "M", "M" },
                { "N", "N" },
                { "X", "X" },
                { "O", "O" },
                { "P", "P" },
                { "R", "R" },
                { "C", "C" },
                { "T", "T" },
                { "U", "U" },
                { "V", "V" },
                { "<", "<" },
                { "\"", "\"" },
                { "W", "W" },
                { "S", "S" },
                { "F", "F" },
                { "Q", "Q" },
                { "H", "H" },
                { "J", "J" },
                { "{", "{" },
                { "}", "}" },
                #endregion

                #region Lowercase
                { "a", "a" },
                { "b", "b" },
                { "g", "g" },
                { "d", "d" },
                { "e", "e" },
                { "6", "6" },
                { "z", "z" },
                { "y", "y" },
                { ";", ";" },
                { "i", "i" },
                { "k", "k" },
                { "l", "l" },
                { "m", "m" },
                { "n", "n" },
                { "x", "x" },
                { "o", "o" },
                { "p", "p" },
                { "r", "r" },
                { "c", "c" },
                { "t", "t" },
                { "u", "u" },
                { "v", "v" },
                { ",", "," },
                { "'", "'" },
                { "w", "w" },
                { "s", "s" },
                { "f", "f" },
                { "q", "q" },
                { "h", "h" },
                { "j", "j" },
                { "[", "[" },
                { "]", "]" },
                #endregion

                // u0300 is the combining grave accent
                // u200D is the zero-width joiner
                // NOTE: Some text renderers and fonts put the accent on the character before it
                { "`", "`" },
                // u0305 is the combining overline
                { "=", "=" },

                { "@", "@" },
                { "&", "&" },
                { "_", "_" },
            };

            return alphabet;
        }
        private static Dictionary<string, string> InitCopticUnicode()
        {
            Dictionary<string, string> alphabet = new Dictionary<string, string>
            {
                // Key is Coptic Standard
                // Value is Coptic unicode

                #region Uppercase
                { "A", "Ⲁ" },
                { "B", "Ⲃ" },
                { "G", "Ⲅ" },
                { "D", "Ⲇ" },
                { "E", "Ⲉ" },
                { "^", "Ⲋ" },
                { "Z", "Ⲍ" },
                { "Y", "Ⲏ" },
                { ":", "Ⲑ" },
                { "I", "Ⲓ" },
                { "K", "Ⲕ" },
                { "L", "Ⲗ" },
                { "M", "Ⲙ" },
                { "N", "Ⲛ" },
                { "X", "Ⲝ" },
                { "O", "Ⲟ" },
                { "P", "Ⲡ" },
                { "R", "Ⲣ" },
                { "C", "Ⲥ" },
                { "T", "Ⲧ" },
                { "U", "Ⲩ" },
                { "V", "Ⲫ" },
                { "<", "Ⲭ" },
                { "\"", "Ⲯ" },
                { "W", "Ⲱ" },
                { "S", "Ϣ" },
                { "F", "Ϥ" },
                { "Q", "Ϧ" },
                { "H", "Ϩ" },
                { "J", "Ϫ" },
                { "{", "Ϭ" },
                { "}", "Ϯ" },
                #endregion

                #region Lowercase
                { "a", "ⲁ" },
                { "b", "ⲃ" },
                { "g", "ⲅ" },
                { "d", "ⲇ" },
                { "e", "ⲉ" },
                { "6", "ⲋ" },
                { "z", "ⲍ" },
                { "y", "ⲏ" },
                { ";", "ⲑ" },
                { "i", "ⲓ" },
                { "k", "ⲕ" },
                { "l", "ⲗ" },
                { "m", "ⲙ" },
                { "n", "ⲛ" },
                { "x", "ⲝ" },
                { "o", "ⲟ" },
                { "p", "ⲡ" },
                { "r", "ⲣ" },
                { "c", "ⲥ" },
                { "t", "ⲧ" },
                { "u", "ⲩ" },
                { "v", "ⲫ" },
                { ",", "ⲭ" },
                { "'", "ⲯ" },
                { "w", "ⲱ" },
                { "s", "ϣ" },
                { "f", "ϥ" },
                { "q", "ϧ" },
                { "h", "ϩ" },
                { "j", "ϫ" },
                { "[", "ϭ" },
                { "]", "ϯ" },
                #endregion

                // u0300 is the combining grave accent
                // u200D is the zero-width joiner
                // NOTE: Some text renderers and fonts put the accent on the character before it
                { "`", "\u0300\u200D" },
                // u0305 is the combining overline
                { "=", "\u0305\u200D" },

                { "@", ":" },
                { "&", ";" },
                { "_", "=" },
                { "¡", "⳪" }
            };

            return alphabet;
        }
        private static Dictionary<string, string> InitAthanasuis()
        {
            Dictionary<string, string> alphabet = new Dictionary<string, string>
            {
                // Key is Coptic Standard
                // Value is Athanasius

                #region Uppercase
                { "A", "A" },
                { "B", "B" },
                { "G", "G" },
                { "D", "D" },
                { "E", "E" },
                { "^", "," },
                { "Z", "Z" },
                { "Y", "H" },
                { ":", "Q" },
                { "I", "I" },
                { "K", "K" },
                { "L", "L" },
                { "M", "M" },
                { "N", "N" },
                { "X", "{" },
                { "O", "O" },
                { "P", "P" },
                { "R", "R" },
                { "C", "C" },
                { "T", "T" },
                { "U", "U" },
                { "V", "V" },
                { "<", "X" },
                { "\"", "Y" },
                { "W", "W" },
                { "S", "}" },
                { "F", "F" },
                { "Q", "\"" },
                { "H", "|" },
                { "J", "J" },
                { "{", "S" },
                { "}", ":" },
                #endregion

                #region Lowercase
                { "a", "a" },
                { "b", "b" },
                { "g", "g" },
                { "d", "d" },
                { "e", "e" },
                { "6", "6" },
                { "z", "z" },
                { "y", "h" },
                { ";", "q" },
                { "i", "i" },
                { "k", "k" },
                { "l", "l" },
                { "m", "m" },
                { "n", "n" },
                { "x", "[" },
                { "o", "o" },
                { "p", "p" },
                { "r", "r" },
                { "c", "c" },
                { "t", "t" },
                { "u", "u" },
                { "v", "v" },
                { ",", "ⲭ" },
                { "'", "y" },
                { "w", "w" },
                { "s", "]" },
                { "f", "f" },
                { "q", "\'" },
                { "h", "\\" },
                { "j", "j" },
                { "[", "s" },
                { "]", ";" },
                #endregion

                { "`", "`" },

                { "@", ">" },
                { "&", "^" },
                { "¡", "_" },

                { "=", "?" }
            };

            return alphabet;
        }
        private static Dictionary<string, string> InitGreekUnicode()
        {
            Dictionary<string, string> alphabet = new Dictionary<string, string>
            {
                // Key is Coptic Standard
                // Value is Greek unicode

                #region Uppercase
                { "A", "Α" },
                { "B", "Β" },
                { "G", "Γ" },
                { "D", "Δ" },
                { "E", "Ε" },
                { "Z", "Ζ" },
                { "Y", "Η" },
                { ":", "Θ" },
                { "I", "Ι" },
                { "K", "Κ" },
                { "L", "Λ" },
                { "M", "Μ" },
                { "N", "Ν" },
                { "X", "Ξ" },
                { "O", "Ο" },
                { "P", "Π" },
                { "R", "Ρ" },
                { "C", "Σ" },
                { "T", "Τ" },
                { "U", "Υ" },
                { "V", "Φ" },
                { "<", "Χ" },
                { "\"", "Ψ" },
                { "W", "Ω" },

                { "S", "Ϣ" },
                { "F", "Ϥ" },
                { "Q", "Ϧ" },
                { "H", "Ϩ" },
                { "J", "Ϫ" },
                { "{", "Ϭ" },
                { "}", "Ϯ" },
                #endregion

                #region Lowercase
                { "a", "α" },
                { "b", "β" },
                { "g", "γ" },
                { "d", "δ" },
                { "e", "ε" },
                { "z", "ζ" },
                { "y", "η" },
                { ";", "θ" },
                { "i", "ι" },
                { "k", "κ" },
                { "l", "λ" },
                { "m", "μ" },
                { "n", "ν" },
                { "x", "ξ" },
                { "o", "ο" },
                { "p", "π" },
                { "r", "ρ" },
                { "c", "σ" },
                { "t", "τ" },
                { "u", "υ" },
                { "v", "φ" },
                { ",", "χ" },
                { "'", "ψ" },
                { "w", "ω" },
                { "s", "ς" },
                { "f", "ϥ" },
                { "q", "ϧ" },
                { "h", "ϩ" },
                { "j", "ϫ" },
                { "[", "ϭ" },
                { "]", "ϯ" },
                #endregion

                // u0300 is the combining grave accent
                // u200D is the zero-width joiner
                // NOTE: Some text renderers and fonts put the accent on the character before it
                { "`", "\u0300\u200D" },
                // u0305 is the combining overline
                { "=", "\u0305\u200D" },

                { "@", ":" },
                { "&", ";" },
                { "_", "=" },
                { "¡", "⳪" }
            };

            return alphabet;
        }

        /// <summary>
        /// Save Font Pack to file. Also adds to imported list of the current instance.
        /// </summary>
        /// <param name="path">Location to save file</param>
        /// <returns></returns>
        public bool SaveFontXml(string path, bool addToList)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Font));
                TextWriter writer = new StreamWriter(new FileStream(path, FileMode.Create));
                serializer.Serialize(writer, Font.ToFontXml(this));
                if (addToList)
                    Fonts.Add(this);
                return true;
            }
            catch (Exception ex)
            {
                Output.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Reads Font Pack from file. Also adds to imported list of the current instance.
        /// </summary>
        /// <param name="path">File to read</param>
        /// <returns></returns>
        public static CopticFont ReadFontXml(string path, bool addToList = true)
        {
            try
            {
                // Create an instance of the XmlSerializer class;
                // specify the type of object to be deserialized.
                XmlSerializer serializer = new XmlSerializer(typeof(Font));
                //If the XML document has been altered with unknown 
                //nodes or attributes, handle them with the 
                //UnknownNode and UnknownAttribute events.

                // A FileStream is needed to read the XML document.
                var text = File.OpenRead(path);

                //Use the Deserialize method to restore the object's state with
                //data from the XML document.
                var data = ((Font)serializer.Deserialize(text)).ToCopticFont();
                if (addToList)
                    Fonts.Add(data);
                return data;
            }
            catch (Exception ex)
            {
                Output.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Generates Font Pack from CSV. Does not import generated pack. 
        /// </summary>
        /// <param name="path">File to generate from. Must have comma sparated values.</param>
        /// <returns></returns>
        public static CopticFont GenerateFromCsv(string path)
        {
            var font = new CopticFont()
            {
                Name = "Imported Font",
                FontName = "Arial",
                IsCopticStandard = false
            };
            string[] lines = File.ReadAllLines(path);
            foreach (string line in lines)
            {
                font.Charmap.Add(line.Split(',')[0], line.Split(',')[1]);
            }

            return font;
        }
    }
}
