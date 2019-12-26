using CoptLib.XML;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CoptLib
{
    public static class CopticInterpreter
    {
        public enum Language
        {
            English,
            Coptic,
            Arabic
        }


        public static IDictionary<string, DocXML> AllDocs = new Dictionary<string, DocXML>();

        /// <summary>
        /// Converts Coptic-Latin to Coptic-Font for displaying
        /// </summary>
        /// <param name="input">Coptic-Latin</param>
        /// <param name="isGreek"></param>
        /// <returns></returns>
        [Obsolete("Please save using Coptic Unicode.")]
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
        [Obsolete("Please save using Coptic Unicode.")]
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

        public static string ConvertToUnicode(string input)
        {
            string output = "";
            Dictionary<string, string> charmap = DictionaryTools.SwitchColumns(CopticFont.CopticUnicode.Charmap);
            var chars = input.ToCharArray();
            foreach (char ch in chars)
            {
                if (charmap.ContainsKey(ch.ToString()))
                    output += charmap[ch.ToString()];
                else
                    output += ch;
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
        public static bool SaveDocXML(string filename, IEnumerable<string> content, Language lang, string name)
        {
            try
            {
                // Create an instance of the XmlSerializer class;
                // specify the type of object to serialize.
                XmlSerializer serializer = new XmlSerializer(typeof(DocXML));
                TextWriter writer = new StreamWriter(new FileStream(filename, FileMode.Create));
                DocXML SaveX = new DocXML();

                SaveX.Language = lang;
                SaveX.Content = new List<StanzaXML>();
                foreach (string s in content)
                {
                    // Replaces c# escaped new lines with XML new lines .Replace("\r\n", "&#xD;")
                    SaveX.Content.Add(new StanzaXML(s));
                }
                SaveX.Name = name;

                // Serialize the save file, and close the TextWriter.
                serializer.Serialize(writer, SaveX);
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
        /// Deserializes and returns a DocXML file. For use in UWP
        /// </summary>
        /// <param name="file">A Stream of the XML file</param>
        /// <returns></returns>
        public static DocXML ReadDocXML(Stream file)
        {
            try
            {
                // Create an instance of the XmlSerializer class;
                // specify the type of object to be deserialized.
                XmlSerializer serializer = new XmlSerializer(typeof(DocXML));
                //If the XML document has been altered with unknown 
                //nodes or attributes, handle them with the 
                //UnknownNode and UnknownAttribute events.

                //Use the Deserialize method to restore the object's state with
                //data from the XML document.
                return (DocXML)serializer.Deserialize(file);
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
        /// Converts CS fonts to Coptic-Font (Coptic1) for displaying
        /// </summary>
        /// <param name="input">Text from Tasbeha.org or other CS font</param>
        /// <returns></returns>
        [Obsolete("Please use ConvertFont() instead.")]
        public static string ConvertFromCS(string input)
        {
            if (CopticStandardAlphabet.Keys.Count == 0)
            {
                InitCopticStandard();
            }
            string output = "";

            bool accent = false;
            foreach (char ch in input.ToCharArray())
            {
                if (CopticStandardAlphabet.Keys.Contains(ch.ToString()))
                {
                    if (ch == '`')
                    {
                        accent = true;
                    }
                    else
                    {
                        output += CopticStandardAlphabet[ch.ToString()];
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
        [Obsolete("Please use ConvertFont() instead.")]
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

        public static string ConvertFont(string input, CopticFont start, CopticFont target)
        {
            string output = "";

            if (start.IsJenkimBefore)
            {
                bool accent = false;
                foreach (char ch in input.ToCharArray())
                {
                    if (start.Charmap.Keys.Contains(ch.ToString()))
                    {
                        if (ch == '`')
                        {
                            accent = true;
                        }
                        else
                        {
                            output += start.Charmap[ch.ToString()];
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
                //return output;
            }
            else
            {
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
                //return output;
            }

            if (target == CopticFont.Coptic1)
                return output;
            else
            {
                return ConvertFromCoptic1(output, target);
            }
        }

        public static string ConvertFromCoptic1(string input, CopticFont target)
        {
            var charmap = DictionaryTools.SwitchColumns(target.Charmap);

            string output = "";
            foreach (char ch in input.ToCharArray())
            {
                if (charmap.ContainsKey(ch.ToString()))
                    output += charmap[ch.ToString()];
                else
                    output += ch;
            }
            return output;
        }

        public static List<CopticFont> CopticFonts = new List<CopticFont>()
        {
            CopticFont.CSAvvaShenouda, CopticFont.CSCopt, CopticFont.CSCopticManuscript,
            CopticFont.CSCoptoManuscript, CopticFont.CSKoptosManuscript, CopticFont.CSNewAthanasius,
            CopticFont.CSPishoi, CopticFont.CopticUnicode
        };

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

        public static Dictionary<string, string> CopticStandardAlphabet = new Dictionary<string, string>();
        private static void InitCopticStandard()
        {
            // Key is Coptic Standard
            // Value is CoptLib

            CopticStandardAlphabet.Add("`", "~");
            CopticStandardAlphabet.Add("@", ":");

            #region Uppercase
            CopticStandardAlphabet.Add("y", "/");
            CopticStandardAlphabet.Add(";", "y");
            #endregion

            #region Lowercase
            CopticStandardAlphabet.Add("Y", "?");
            CopticStandardAlphabet.Add(":", "Y");
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
        private Stream ZipStream {
            get;
        }
        public string Name {
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
            string FolderPath = Path.Combine(tempPath, Name);
            if (Directory.Exists(FolderPath))
            {
                foreach (string file in Directory.EnumerateFiles(FolderPath))
                {
                    File.Delete(file);
                }
                Directory.Delete(FolderPath);
            }
            Directory.CreateDirectory(FolderPath);

            bool isSuccess = ZipStream == null ? UnzipFile(ZipPath, FolderPath) : UnzipFile(ZipStream, FolderPath);
            if (isSuccess)
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
                    TextWriter docWriter = new StreamWriter(new FileStream(Path.Combine(rootPath, indexDoc.Name + ".xml"), FileMode.Create));
                    docSerializer.Serialize(docWriter, CopticInterpreter.AllDocs[indexDoc.UUID]);
                    docWriter.Dispose();
                }

                // Now save the index
                XmlSerializer serializer = new XmlSerializer(typeof(IndexXML));
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
                Console.WriteLine("Index or Content is null");
                return false;
            }
        }

        public void AddContent(DocXML xml)
        {
            Index.IncludedDocs.Add(new IndexDocXML()
            {
                Name = xml.Name,
                UUID = xml.UUID,
                Language = xml.Language,
            });
        }

        public void AddContent(IEnumerable<DocXML> docs)
        {
            foreach (DocXML xml in docs)
            {
                Index.IncludedDocs.Add(new IndexDocXML()
                {
                    Name = xml.Name,
                    UUID = xml.UUID,
                    Language = xml.Language
                });
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

    public static class DictionaryTools
    {
        public static Dictionary<string, string> SwitchColumns(Dictionary<string, string> dictionary)
        {
            Dictionary<string, string> output = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in dictionary)
            {
                output.Add(pair.Value, pair.Key);
            }
            return output;
        }
    }

    public class CopticFont
    {
        public string Name;
        public string FontName;
        public Dictionary<string, string> Charmap = new Dictionary<string, string>();
        public bool IsJenkimBefore = true;
        public bool IsCopticStandard = true;

        public static CopticFont CSAvvaShenouda {
            get {
                return new CopticFont()
                {
                    Name = "CS Avva Shenouda",
                    FontName = "CS Avva Shenouda",
                    IsCopticStandard = true,
                    IsJenkimBefore = true,
                    Charmap = InitCopticStandard()
                };
            }
        }
        public static CopticFont CSCopt {
            get {
                return new CopticFont()
                {
                    Name = "CS Copt",
                    FontName = "CS Copt",
                    IsCopticStandard = true,
                    IsJenkimBefore = true,
                    Charmap = InitCopticStandard()
                };
            }
        }
        public static CopticFont CSCopticManuscript {
            get {
                return new CopticFont()
                {
                    Name = "CS Coptic Manuscript",
                    FontName = "CS Coptic Manuscript",
                    IsCopticStandard = true,
                    IsJenkimBefore = true,
                    Charmap = InitCopticStandard()
                };
            }
        }
        public static CopticFont CSCoptoManuscript {
            get {
                return new CopticFont()
                {
                    Name = "CS Copto Manuscript",
                    FontName = "CS Copto Manuscript",
                    IsCopticStandard = true,
                    IsJenkimBefore = true,
                    Charmap = InitCopticStandard()
                };
            }
        }
        public static CopticFont CSKoptosManuscript {
            get {
                return new CopticFont()
                {
                    Name = "CS Koptos Manuscript",
                    FontName = "CS Koptos Manuscript",
                    IsCopticStandard = true,
                    IsJenkimBefore = true,
                    Charmap = InitCopticStandard()
                };
            }
        }
        public static CopticFont CSNewAthanasius {
            get {
                return new CopticFont()
                {
                    Name = "CS New Athanasius",
                    FontName = "CS New Athanasius",
                    IsCopticStandard = true,
                    IsJenkimBefore = true,
                    Charmap = InitCopticStandard()
                };
            }
        }
        public static CopticFont CSPishoi {
            get {
                return new CopticFont()
                {
                    Name = "CS Pishoi",
                    FontName = "CS Pishoi",
                    IsCopticStandard = true,
                    IsJenkimBefore = true,
                    Charmap = InitCopticStandard()
                };
            }
        }
        public static CopticFont CopticUnicode {
            get {
                return new CopticFont()
                {
                    Name = "Coptic Unicode",
                    FontName = "Segoe UI",
                    IsCopticStandard = false,
                    IsJenkimBefore = false,
                    Charmap = InitCopticUnicode()
                };
            }
        }
        public static CopticFont Coptic1 {
            get {
                return new CopticFont()
                {
                    Name = "Coptic1",
                    FontName = "Coptic1",
                    IsCopticStandard = false,
                    IsJenkimBefore = false,
                    Charmap = new Dictionary<string, string>()
                };
            }
        }

        private static Dictionary<string, string> InitCopticStandard()
        {
            Dictionary<string, string> CopticStandardAlphabet = new Dictionary<string, string>();
            // Key is Tasbeha
            // Value is CoptLib

            CopticStandardAlphabet.Add("`", "~");
            CopticStandardAlphabet.Add("@", ":");

            #region Uppercase
            CopticStandardAlphabet.Add("y", "/");
            CopticStandardAlphabet.Add(";", "y");
            #endregion

            #region Lowercase
            CopticStandardAlphabet.Add("Y", "?");
            CopticStandardAlphabet.Add(":", "Y");
            #endregion

            return CopticStandardAlphabet;
        }
        private static Dictionary<string, string> InitCopticUnicode()
        {
            Dictionary<string, string> CopticUnicodeAlphabet = new Dictionary<string, string>();
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

            return CopticUnicodeAlphabet;
        }

        /// <summary>
        /// Save Font Pack to file. Also adds to imported list of the current instance.
        /// </summary>
        /// <param name="path">Location to save file</param>
        /// <returns></returns>
        public bool SaveFontXML(string path, bool addToList)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(FontXML));
                TextWriter writer = new StreamWriter(new FileStream(path, FileMode.Create));
                serializer.Serialize(writer, FontXML.ToFontXML(this));
                if (addToList)
                    CopticInterpreter.CopticFonts.Add(this);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Reads Font Pack from file. Also adds to imported list of the current instance.
        /// </summary>
        /// <param name="path">File to read</param>
        /// <returns></returns>
        public static CopticFont ReadFontXML(string path, bool addToList = true)
        {
            try
            {
                // Create an instance of the XmlSerializer class;
                // specify the type of object to be deserialized.
                XmlSerializer serializer = new XmlSerializer(typeof(FontXML));
                //If the XML document has been altered with unknown 
                //nodes or attributes, handle them with the 
                //UnknownNode and UnknownAttribute events.

                // A FileStream is needed to read the XML document.
                var text = File.OpenRead(path);

                //Use the Deserialize method to restore the object's state with
                //data from the XML document.
                var data = ((FontXML)serializer.Deserialize(text)).ToCopticFont();
                if (addToList)
                    CopticInterpreter.CopticFonts.Add(data);
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Generates Font Pack from CSV. Does not import generated pack. 
        /// </summary>
        /// <param name="path">File to generate from. Must have comma sparated values.</param>
        /// <returns></returns>
        public static CopticFont GenerateFromCSV(string path)
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

        /// <summary>
        /// Defines the mapping of Coptic Standard to Unicode characters
        /// </summary>
        public static readonly Dictionary<string, string> UnicodeMapping = new Dictionary<string, string>()
        {
            // Lowercase
            { "q", "ϧ" },
            { "w", "ω" },
            { "e", "ε" },
            { "r", "ρ" },
            { "t", "τ" },
            { "y", "η" },
            { "u", "υ" },
            { "i", "ι" },
            { "o", "ο" },
            { "p", "π" },
            { "[", "ϭ" },
            { "]", "ϯ" },
            { "\\", "\\" },

            { "a", "α" },
            { "s", "ϣ" },
            { "d", "δ" },
            { "f", "ϥ" },
            { "g", "γ" },
            { "h", "ϩ" },
            { "j", "ϫ" },
            { "k", "κ" },
            { "l", "λ" },
            { ";", "θ" },
            { "'", "ψ" },

            { "z", "ζ" },
            { "x", "ξ" },
            { "c", "σ" },
            { "v", "φ" },
            { "b", "β" },
            { "n", "ν" },
            { "m", "μ" },
            { ",", "χ" },
            { ".", "." },
            { "/", "/" },

            // Uppercase
            { "Q", "Ϧ" },
            { "W", "Ω" },
            { "E", "Ε" },
            { "R", "Ρ" },
            { "T", "Τ" },
            { "Y", "Ν" },
            { "U", "Υ" },
            { "I", "Ι" },
            { "O", "Ο" },
            { "P", "Π" },
            { "{", "Ϭ" },
            { "}", "Ϯ" },
            { "|", "|" },

            { "A", "Α" },
            { "S", "Ϣ" },
            { "D", "Δ" },
            { "F", "Ϥ" },
            { "G", "Γ" },
            { "H", "Ϩ" },
            { "J", "Ϫ" },
            { "K", "Κ" },
            { "L", "Λ" },
            { ":", "Θ" },
            { "\"", "Ψ" },

            { "Z", "Ζ" },
            { "X", "Ξ" },
            { "C", "Σ" },
            { "V", "Φ" },
            { "B", "Β" },
            { "N", "Ν" },
            { "M", "Μ" },
            { "<", "Χ" },
            { ">", "," },
            { "?", "?" },
        };
    }
}
