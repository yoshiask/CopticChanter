using CoptLib.XML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Windows.Storage;

namespace CoptLib
{
    public static class CopticInterpreter
    {
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

        public static async Task<DocXML> ReadDocXML(StorageFile file)
        {
            if (file != null)
            {
                // Create an instance of the XmlSerializer class;
                // specify the type of object to be deserialized.
                XmlSerializer serializer = new XmlSerializer(typeof(DocXML));
                //If the XML document has been altered with unknown 
                //nodes or attributes, handle them with the 
                //UnknownNode and UnknownAttribute events.

                // A FileStream is needed to read the XML document.
                var text = await FileIO.ReadTextAsync(file);

                // Save the doc to AppData for later use
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                StorageFile storeFile = await storageFolder.CreateFileAsync(
                    file.Name,
                    CreationCollisionOption.ReplaceExisting
                );
                await FileIO.WriteTextAsync(storeFile, text);

                //Use the Deserialize method to restore the object's state with
                //data from the XML document.
                return (DocXML)serializer.Deserialize(XDocument.Parse(text).CreateReader());
            }
            else
            {
                return null;
            }
        }

        public static async Task<bool> SaveDocXML()
        {
            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Extensible Markup Language", new List<string>() { ".xml" });
            savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "New Document";

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until
                // we finish making changes and call CompleteUpdatesAsync.
                CachedFileManager.DeferUpdates(file);
                // write to file
                //await FileIO.WriteTextAsync(file, file.Name);

                // Create an instance of the XmlSerializer class;
                // specify the type of object to serialize.
                //XmlSerializer serializer = new XmlSerializer(typeof(DocXML));
                DocXML Doc = new DocXML
                {
                    Parent = "None",
                    Name = "Response",
                    Content = {
                        "D-o-ks-a- -p-a-t-r-i- -k-a-i",
                    },
                };
                await FileIO.WriteTextAsync(file, Doc.ToXmlString(), Windows.Storage.Streams.UnicodeEncoding.Utf8);

                // Let Windows know that we're finished changing the file so
                // the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                Windows.Storage.Provider.FileUpdateStatus status =
                    await CachedFileManager.CompleteUpdatesAsync(file);

                if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
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
}
