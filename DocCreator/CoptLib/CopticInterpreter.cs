﻿using CoptLib.XML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CoptLib
{
    public static class CopticInterpreter
    {
        public static IDictionary<string, DocXML> AllDocs = new Dictionary<string, DocXML>();

        /// <summary>
        /// Converts Coptic-Latin to Coptic-Font for displaying
        /// </summary>
        /// <param name="input">Coptic-Latin</param>
        /// <param name="isGreek"></param>
        /// <returns></returns>
        public static string ConvertFromString(string input, bool isGreek = false)
        {
            // Characters must be separated by a dash
            // For capital Coptic letters, capitalize the first letter of the Latin set
            if (CopticAlphabet.Keys.Count == 0)
            {
                InitAlphabet(isGreek);
            }
            string output = "";

            string[] split = input.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in split)
            {
                string copt;
                if (s.EndsWith("`"))
                {
                    CopticAlphabet.TryGetValue(s, out copt);
                    output += copt;
                    output += "~";
                }
                else if (s == " ")
                {
                    output += " ";
                }
                else if (s == ";")
                {
                    output += ";";
                }
                else if (s == ":")
                {
                    output += ":";
                }
                else if (s == ".")
                {
                    output += ".";
                }
                else
                {
                    CopticAlphabet.TryGetValue(s, out copt);
                    output += copt;
                }
            }

            return output;
        }

        /// <summary>
        /// Converts Coptic-Font to Coptic-Latin for saving
        /// </summary>
        /// <param name="input">Coptin-Font</param>
        /// <param name="isGreek"></param>
        /// <returns></returns>
        public static string ConvertToString(string input, bool isGreek = false)
        {
            if (CopticAlphabet.Keys.Count == 0)
            {
                InitAlphabet(isGreek);
            }

            string output = "";
            foreach (char ch in input.ToList())
            {
                string s = ch.ToString();
                if (s == " ")
                {
                    output += " -";
                }
                else if (s == "~")
                {
                    output += "`-";
                }
                else if (s == ";")
                {
                    output += ";-";
                }
                else if (s == ":")
                {
                    output += ":-";
                }
                else if (s == ".")
                {
                    output += ".-";
                }
                else if (s == "\n")
                {
                    output += "&amp;#xD;-";
                }
                else
                {
                    output += CopticAlphabet.FirstOrDefault(x => x.Value == s).Key + "-";
                }
            }
            return output;
        }

        /// <summary>
        /// Serializes and save a DocXML file
        /// </summary>
        /// <param name="filename">Path to save to, including filename</param>
        /// <param name="content">List of stanzas to save</param>
        /// <param name="coptic">If the input language is Coptic</param>
        /// <param name="name">Name of the reading or hymn</param>
        /// <returns></returns>
        public static bool SaveDocXML(string filename, IEnumerable<string> content, bool coptic, string name)
        {
            try
            {
                // Create an instance of the XmlSerializer class;
                // specify the type of object to serialize.
                XmlSerializer serializer = new XmlSerializer(typeof(DocXML));
                TextWriter writer = new StreamWriter(filename);
                DocXML SaveX = new DocXML();

                SaveX.Coptic = coptic;
                SaveX.Stanzas = new List<DocXML.StanzaXML>();
                if (coptic)
                {
                    foreach (string s in content)
                    {
                        SaveX.Stanzas.Add(new DocXML.StanzaXML(s, "coptic"));
                    }
                }
                else
                {
                    foreach (string s in content)
                    {
                        // Replaces c# escaped new lines with XML new lines
                        SaveX.Stanzas.Add(new DocXML.StanzaXML(s.Replace("\r\n", "&#xD;"), "english"));
                    }
                }
                // Checks if first stanza is empty
                if (SaveX.Stanzas[0].Content == "")
                {
                    SaveX.Stanzas.RemoveAt(0);
                }
                SaveX.Name = name;

                // Serialize the save file, and close the TextWriter.
                serializer.Serialize(writer, SaveX);
                writer.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Deserializes and returns a DocXML file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static DocXML ReadDocXML(string path)
        {
            try
            {
                // Create an instance of the XmlSerializer class;
                // specify the type of object to be deserialized.
                XmlSerializer serializer = new XmlSerializer(typeof(DocXML));
                //If the XML document has been altered with unknown 
                //nodes or attributes, handle them with the 
                //UnknownNode and UnknownAttribute events.

                // A FileStream is needed to read the XML document.
                var text = File.OpenRead(path);

                //Use the Deserialize method to restore the object's state with
                //data from the XML document.
                return (DocXML)serializer.Deserialize(text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Unzips, serializes, and returns an Index and list of Docs
        /// </summary>
        /// <param name="path">Path to zip file</param>
        /// <returns></returns>
        public static DocSetReader.ReadResults ReadSet(string path)
        {
            return (new DocSetReader(path)).Read();
        }

        /// <summary>
        /// Serializes and zips an Index and included docs
        /// </summary>
        /// <param name="filename">Name of file to be saved</param>
        /// <param name="setname">Name of set to be saved</param>
        /// <param name="setUUID">Generated UUID of set</param>
        /// <param name="incdocs">Docs to include in set</param>
        public static void SaveSet(string filename, string setname, string setUUID, IEnumerable<string> incdocs)
        {
            var setX = new IndexXML()
            {
                Name = setname,
                UUID = setUUID
            };
            List<string> incDocs = new List<string>();

            foreach (string docUUID in incdocs)
            {
                setX.IncludedDocs.Add(AllDocs[docUUID].ToIndexDocXML());
            }

            new DocSetWriter(setX).Write(filename);
        }

        /// <summary>
        /// Converts Alt-Coptic-Font (CS Avva Shenouda) to Coptic-Font for displaying
        /// </summary>
        /// <param name="input">Text from Tasbeha.org</param>
        /// <returns></returns>
        public static string ConvertFromTasbeha(string input)
        {
            if (TasbehaAlphabet.Keys.Count == 0)
            {
                InitTasbeha();
            }
            string output = "";

            bool accent = false;
            foreach (char ch in input.ToCharArray())
            {
                if (TasbehaAlphabet.Keys.Contains(ch.ToString()))
                {
                    if (ch == '`')
                    {
                        accent = true;
                    }
                    else
                    {
                        output += TasbehaAlphabet[ch.ToString()];
                        if (accent)
                        {
                            output += "~";
                            accent = false;
                        }
                    }
                }
                else
                {
                    output += ch.ToString();
                    if (accent)
                    {
                        output += "~";
                        accent = false;
                    }
                }
            }
            return output;
        }

        /// <summary>
        /// Converts Coptic-Unicode to Coptic-Font for display
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ConvertFromCopticUnicode(string input)
        {
            if (CopticUnicodeAlphabet.Keys.Count == 0)
            {
                InitCopticUnicode();
            }
            string output = "";

            bool accent = false;
            foreach (char ch in input.ToCharArray())
            {
                if (CopticUnicodeAlphabet.Keys.Contains(ch.ToString()))
                {
                    if (ch == '~')
                    {
                        accent = true;
                    }
                    else
                    {
                        output += CopticUnicodeAlphabet[ch.ToString()];
                        if (accent)
                        {
                            output += "~";
                            accent = false;
                        }
                    }
                }
                else
                {
                    output += ch.ToString();
                    if (accent)
                    {
                        output += "~";
                        accent = false;
                    }
                }
            }
            return output;
        }

        public static Dictionary<string, string> CopticAlphabet = new Dictionary<string, string>();
        private static void InitAlphabet(bool isGreek)
        {
            if (isGreek)
            {
                // Key is Latin phonetic
                // Value is Greek unicode

                #region Uppercase
                CopticAlphabet.Add("A", "Ⲁ");
                CopticAlphabet.Add("B", "Ⲃ");
                CopticAlphabet.Add("G", "Ⲅ");
                CopticAlphabet.Add("D", "Ⲇ");
                CopticAlphabet.Add("Eh", "Ⲉ");
                CopticAlphabet.Add("Ih", "Ⲉ");
                CopticAlphabet.Add("So", "Ⲋ");
                CopticAlphabet.Add("Z", "Ⲍ");
                CopticAlphabet.Add("Ee", "Ⲏ");
                CopticAlphabet.Add("Th", "Ⲑ");
                CopticAlphabet.Add("I", "Ⲓ");
                CopticAlphabet.Add("K", "Ⲕ");
                CopticAlphabet.Add("L", "Ⲗ");
                CopticAlphabet.Add("M", "Ⲙ");
                CopticAlphabet.Add("N", "Ⲛ");
                CopticAlphabet.Add("Ks", "Ⲝ");
                CopticAlphabet.Add("O", "Ⲟ");
                CopticAlphabet.Add("P", "Ⲡ");
                CopticAlphabet.Add("R", "Ⲣ");
                CopticAlphabet.Add("S", "Ⲥ");
                CopticAlphabet.Add("T", "Ⲧ");
                CopticAlphabet.Add("U", "Ⲩ");
                CopticAlphabet.Add("Ou", "Ⲩ");
                CopticAlphabet.Add("Ph", "Ⲫ");
                CopticAlphabet.Add("Kh", "Ⲭ");
                CopticAlphabet.Add("Ps", "Ⲯ");
                CopticAlphabet.Add("Oh", "Ⲱ");
                CopticAlphabet.Add("Sh", "Ϣ");
                CopticAlphabet.Add("F", "Ϥ");
                CopticAlphabet.Add("X", "Ϧ");
                CopticAlphabet.Add("H", "Ϩ");
                CopticAlphabet.Add("J", "Ϫ");
                CopticAlphabet.Add("Q", "Ϭ");
                CopticAlphabet.Add("Tee", "Ϯ");
                #endregion

                #region Lowercase
                CopticAlphabet.Add("a", "ⲁ");
                CopticAlphabet.Add("b", "ⲃ");
                CopticAlphabet.Add("g", "ⲅ");
                CopticAlphabet.Add("d", "ⲇ");
                CopticAlphabet.Add("eh", "ⲉ");
                CopticAlphabet.Add("ih", "ⲉ");
                CopticAlphabet.Add("so", "ⲋ");
                CopticAlphabet.Add("z", "ⲍ");
                CopticAlphabet.Add("ee", "ⲏ");
                CopticAlphabet.Add("th", "ⲑ");
                CopticAlphabet.Add("i", "ⲓ");
                CopticAlphabet.Add("k", "ⲕ");
                CopticAlphabet.Add("l", "ⲗ");
                CopticAlphabet.Add("m", "ⲙ");
                CopticAlphabet.Add("n", "ⲛ");
                CopticAlphabet.Add("ks", "ⲝ");
                CopticAlphabet.Add("o", "ⲟ");
                CopticAlphabet.Add("p", "ⲡ");
                CopticAlphabet.Add("r", "ⲣ");
                CopticAlphabet.Add("s", "ⲥ");
                CopticAlphabet.Add("t", "ⲧ");
                CopticAlphabet.Add("u", "ⲩ");
                CopticAlphabet.Add("ou", "ⲩ");
                CopticAlphabet.Add("ph", "ⲫ");
                CopticAlphabet.Add("kh", "ⲭ");
                CopticAlphabet.Add("ps", "ⲯ");
                CopticAlphabet.Add("oh", "ⲱ");
                CopticAlphabet.Add("sh", "	ϣ");
                CopticAlphabet.Add("f", "ϥ");
                CopticAlphabet.Add("x", "ϧ");
                CopticAlphabet.Add("h", "ϩ");
                CopticAlphabet.Add("j", "ϫ");
                CopticAlphabet.Add("q", "ϭ");
                CopticAlphabet.Add("tee", "ϯ");
                #endregion
            }
            else
            {
                // Key is Latin phonetic
                // Value is Coptic font character

                #region Uppercase
                CopticAlphabet.Add("A", "A");
                CopticAlphabet.Add("B", "B");
                CopticAlphabet.Add("G", "G");
                CopticAlphabet.Add("D", "D");
                CopticAlphabet.Add("Eh", "E");
                CopticAlphabet.Add("Ih", "E");
                CopticAlphabet.Add("So", "^");
                CopticAlphabet.Add("Z", "Z");
                CopticAlphabet.Add("Ee", "?");
                CopticAlphabet.Add("Th", "Y");
                CopticAlphabet.Add("I", "I");
                CopticAlphabet.Add("K", "K");
                CopticAlphabet.Add("L", "L");
                CopticAlphabet.Add("M", "M");
                CopticAlphabet.Add("N", "N");
                CopticAlphabet.Add("Ks", "X");
                CopticAlphabet.Add("O", "O");
                CopticAlphabet.Add("P", "P");
                CopticAlphabet.Add("R", "R");
                CopticAlphabet.Add("S", "C");
                CopticAlphabet.Add("T", "T");
                CopticAlphabet.Add("U", "U");
                CopticAlphabet.Add("Ou", "U");
                CopticAlphabet.Add("Ph", "V");
                CopticAlphabet.Add("Kh", "<");
                CopticAlphabet.Add("Ps", "\"");
                CopticAlphabet.Add("Oh", "W");
                CopticAlphabet.Add("Sh", "S");
                CopticAlphabet.Add("F", "F");
                CopticAlphabet.Add("X", "Q");
                CopticAlphabet.Add("H", "H");
                CopticAlphabet.Add("J", "J");
                CopticAlphabet.Add("Q", "{");
                CopticAlphabet.Add("Tee", "}");
                #endregion

                #region Lowercase
                CopticAlphabet.Add("a", "a");
                CopticAlphabet.Add("b", "b");
                CopticAlphabet.Add("g", "g");
                CopticAlphabet.Add("d", "d");
                CopticAlphabet.Add("eh", "e");
                CopticAlphabet.Add("ih", "e");
                CopticAlphabet.Add("so", "6");
                CopticAlphabet.Add("z", "z");
                CopticAlphabet.Add("ee", "/");
                CopticAlphabet.Add("th", "y");
                CopticAlphabet.Add("i", "i");
                CopticAlphabet.Add("k", "k");
                CopticAlphabet.Add("l", "l");
                CopticAlphabet.Add("m", "m");
                CopticAlphabet.Add("n", "n");
                CopticAlphabet.Add("ks", "x");
                CopticAlphabet.Add("o", "o");
                CopticAlphabet.Add("p", "p");
                CopticAlphabet.Add("r", "r");
                CopticAlphabet.Add("s", "c");
                CopticAlphabet.Add("t", "t");
                CopticAlphabet.Add("u", "u");
                CopticAlphabet.Add("ou", "u");
                CopticAlphabet.Add("ph", "v");
                CopticAlphabet.Add("kh", ",");
                CopticAlphabet.Add("ps", "'");
                CopticAlphabet.Add("oh", "w");
                CopticAlphabet.Add("sh", "	s");
                CopticAlphabet.Add("f", "f");
                CopticAlphabet.Add("x", "q");
                CopticAlphabet.Add("h", "h");
                CopticAlphabet.Add("j", "j");
                CopticAlphabet.Add("q", "[");
                CopticAlphabet.Add("tee", "]");
                #endregion
            }
        }

        public static Dictionary<string, string> TasbehaAlphabet = new Dictionary<string, string>();
        private static void InitTasbeha()
        {
            // Key is Tasbeha
            // Value is CoptLib

            TasbehaAlphabet.Add("`", "~");
            TasbehaAlphabet.Add("@", ":");

            #region Uppercase
            TasbehaAlphabet.Add("y", "/");
            TasbehaAlphabet.Add(";", "y");
            #endregion

            #region Lowercase
            TasbehaAlphabet.Add("Y", "?");
            TasbehaAlphabet.Add(":", "Y");
            #endregion
        }

        public static Dictionary<string, string> CopticUnicodeAlphabet = new Dictionary<string, string>();
        private static void InitCopticUnicode()
        {
            // Key is Greek unicode
            // Value is CopticLib

            #region Uppercase
            CopticUnicodeAlphabet.Add("Ⲁ", "A");
            CopticUnicodeAlphabet.Add("Ⲃ", "B");
            CopticUnicodeAlphabet.Add("Ⲅ", "G");
            CopticUnicodeAlphabet.Add("Ⲇ", "D");
            CopticUnicodeAlphabet.Add("Ⲉ", "E");
            CopticUnicodeAlphabet.Add("Ⲋ", "^");
            CopticUnicodeAlphabet.Add("Ⲍ", "Z");
            CopticUnicodeAlphabet.Add("Ⲏ", "?");
            CopticUnicodeAlphabet.Add("Ⲑ", "Y");
            CopticUnicodeAlphabet.Add("Ⲓ", "I");
            CopticUnicodeAlphabet.Add("Ⲕ", "K");
            CopticUnicodeAlphabet.Add("Ⲗ", "L");
            CopticUnicodeAlphabet.Add("Ⲙ", "M");
            CopticUnicodeAlphabet.Add("Ⲛ", "N");
            CopticUnicodeAlphabet.Add("Ⲝ", "X");
            CopticUnicodeAlphabet.Add("Ⲟ", "O");
            CopticUnicodeAlphabet.Add("Ⲡ", "P");
            CopticUnicodeAlphabet.Add("Ⲣ", "R");
            CopticUnicodeAlphabet.Add("Ⲥ", "C");
            CopticUnicodeAlphabet.Add("Ⲧ", "T");
            CopticUnicodeAlphabet.Add("Ⲩ", "U");
            CopticUnicodeAlphabet.Add("Ⲫ", "V");
            CopticUnicodeAlphabet.Add("Ⲭ", "<");
            CopticUnicodeAlphabet.Add("Ⲯ", "\"");
            CopticUnicodeAlphabet.Add("Ⲱ", "W");
            CopticUnicodeAlphabet.Add("Ϣ", "S");
            CopticUnicodeAlphabet.Add("Ϥ", "F");
            CopticUnicodeAlphabet.Add("Ϧ", "Q");
            CopticUnicodeAlphabet.Add("Ϩ", "H");
            CopticUnicodeAlphabet.Add("Ϫ", "J");
            CopticUnicodeAlphabet.Add("Ϭ", "{");
            CopticUnicodeAlphabet.Add("Ϯ", "}");
            #endregion

            #region Lowercase
            CopticUnicodeAlphabet.Add("ⲁ", "a");
            CopticUnicodeAlphabet.Add("ⲃ", "b");
            CopticUnicodeAlphabet.Add("ⲅ", "g");
            CopticUnicodeAlphabet.Add("ⲇ", "d");
            CopticUnicodeAlphabet.Add("ⲉ", "e");
            CopticUnicodeAlphabet.Add("ⲋ", "6");
            CopticUnicodeAlphabet.Add("ⲍ", "z");
            CopticUnicodeAlphabet.Add("ⲏ", "/");
            CopticUnicodeAlphabet.Add("ⲑ", "y");
            CopticUnicodeAlphabet.Add("ⲓ", "i");
            CopticUnicodeAlphabet.Add("ⲕ", "k");
            CopticUnicodeAlphabet.Add("ⲗ", "l");
            CopticUnicodeAlphabet.Add("ⲙ", "m");
            CopticUnicodeAlphabet.Add("ⲛ", "n");
            CopticUnicodeAlphabet.Add("ⲝ", "x");
            CopticUnicodeAlphabet.Add("ⲟ", "o");
            CopticUnicodeAlphabet.Add("ⲡ", "p");
            CopticUnicodeAlphabet.Add("ⲣ", "r");
            CopticUnicodeAlphabet.Add("ⲥ", "c");
            CopticUnicodeAlphabet.Add("ⲧ", "t");
            CopticUnicodeAlphabet.Add("ⲩ", "u");
            CopticUnicodeAlphabet.Add("ⲫ", "v");
            CopticUnicodeAlphabet.Add("ⲭ", ",");
            CopticUnicodeAlphabet.Add("ⲯ", "'");
            CopticUnicodeAlphabet.Add("ⲱ", "w");
            CopticUnicodeAlphabet.Add("ϣ", "s");
            CopticUnicodeAlphabet.Add("ϥ", "f");
            CopticUnicodeAlphabet.Add("ϧ", "q");
            CopticUnicodeAlphabet.Add("ϩ", "h");
            CopticUnicodeAlphabet.Add("ϫ", "j");
            CopticUnicodeAlphabet.Add("ϭ", "[");
            CopticUnicodeAlphabet.Add("ϯ", "]");
            #endregion
        }
    }

    public class DocSetReader
    {
        private string ZipPath {
            get;
        }

        public DocSetReader(string zipPath)
        {
            ZipPath = zipPath;
        }

        public ReadResults Read()
        {
            string FolderPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Coptic Chanter", "Doc Creator", "temp",
                Path.GetFileNameWithoutExtension(ZipPath)
            );
            if (Directory.Exists(FolderPath))
            {
                foreach (string file in Directory.EnumerateFiles(FolderPath))
                {
                    File.Delete(file);
                }
                Directory.Delete(FolderPath);
            }
            Directory.CreateDirectory(FolderPath);
            
            if (UnzipFile(ZipPath, FolderPath))
            {
                var results = new ReadResults();
                List<string> files = Directory.EnumerateFiles(FolderPath).ToList();
                if (files.Contains(Path.Combine(FolderPath, "index.xml")))
                {
                    // Create an instance of the XmlSerializer class;
                    // specify the type of object to be deserialized.
                    XmlSerializer serializer = new XmlSerializer(typeof(IndexXML));

                    // A FileStream is needed to read the XML document.
                    string text = File.ReadAllText(Path.Combine(FolderPath, "index.xml"));

                    //Use the Deserialize method to restore the object's state with
                    //data from the XML document.
                    results.Index = (IndexXML)serializer.Deserialize(XDocument.Parse(text).CreateReader());

                    foreach (string filename in files)
                    {
                        if (filename != Path.Combine(FolderPath,"index.xml") && !filename.EndsWith(".zip"))
                        {
                            try
                            {
                                var doc = CopticInterpreter.ReadDocXML(filename);
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
                                Console.WriteLine("Error: ", ex.Message);
                                Console.WriteLine("Unexpected file in set");
                            }
                        }
                    }

                    return results;
                }
                else
                {
                    Console.WriteLine("Set file not valid: No index found");
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
                System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, extractPath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public class ReadResults
        {
            public IndexXML Index;
            public List<DocXML> IncludedDocs = new List<DocXML>();
        }
    }

    public class DocSetWriter
    {
        /// <summary>
        /// An Index XML object that contains data to write to set
        /// </summary>
        public IndexXML Index {
            get;
            private set;
        }

        public DocSetWriter(IndexXML index = null)
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
                foreach (IndexDocXML indexDoc in Index.IncludedDocs)
                {
                    XmlSerializer docSerializer = new XmlSerializer(typeof(DocXML));
                    TextWriter docWriter = new StreamWriter(Path.Combine(rootPath, indexDoc.Name + ".xml"));
                    docSerializer.Serialize(docWriter, CopticInterpreter.AllDocs[indexDoc.UUID]);
                    docWriter.Close();
                }

                // Now save the index
                XmlSerializer serializer = new XmlSerializer(typeof(IndexXML));
                TextWriter writer = new StreamWriter(Path.Combine(rootPath, "index.xml"));
                serializer.Serialize(writer, Index);
                writer.Close();

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
                Console.WriteLine("Index or Content is null");
                return false;
            }
        }

        public void AddContent(DocXML xml)
        {
            Index.IncludedDocs.Add(new IndexDocXML() { Name = xml.Name, UUID = xml.UUID, hasEnglish = !xml.Coptic, hasCoptic = xml.Coptic, hasArabic = false });
        }

        public void AddContent(IEnumerable<DocXML> docs)
        {
            foreach (DocXML xml in docs)
            {
                Index.IncludedDocs.Add(new IndexDocXML() { Name = xml.Name, UUID = xml.UUID, hasEnglish = !xml.Coptic, hasCoptic = xml.Coptic, hasArabic = false });
            }
        }

        public void ClearContent()
        {
            Index.IncludedDocs.Clear();
        }

        public void SetIndex(IndexXML index)
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

    public static class XmlTools
    {
        public static string ToXmlString<T>(this T input)
        {
            using (var writer = new StringWriter())
            {
                input.ToXml(writer);
                return writer.ToString();
            }
        }

        public static void ToXml<T>(this T objectToSerialize, Stream stream)
        {
            new XmlSerializer(typeof(T)).Serialize(stream, objectToSerialize);
        }

        public static void ToXml<T>(this T objectToSerialize, StringWriter writer)
        {
            new XmlSerializer(typeof(T)).Serialize(writer, objectToSerialize);
        }
    }

    public static class DateTimeExtensions
    {
        ///<summary>Gets the first week day following a date.</summary>
        ///<param name="date">The date.</param>
        ///<param name="dayOfWeek">The day of week to return.</param>
        ///<returns>The first dayOfWeek day following date, or date if it is on dayOfWeek.</returns>
        public static DateTime Next(this DateTime date, DayOfWeek dayOfWeek)
        {
            return date.AddDays((dayOfWeek < date.DayOfWeek ? 7 : 0) + dayOfWeek - date.DayOfWeek);
        }

        public static CopticDate Next(this CopticDate date, DayOfWeek dayOfWeek)
        {
            return date.AddDays((dayOfWeek < date.DayOfWeek ? 7 : 0) + dayOfWeek - date.DayOfWeek);
        }
    }
}
