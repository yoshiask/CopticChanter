using CoptLib.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

#if DEBUG
using Output = System.Diagnostics.Debug;
#else
using Output = System.Console;
#endif

namespace CoptLib.Writing
{
    public partial class CopticFont
    {
        public string Name;
        public string FontName;
        public Dictionary<string, string> Charmap = new();
        public char JenkimCharacter;
        public bool IsJenkimBefore = true;
        public bool IsCopticStandard = true;

        /// <summary>
        /// Converts Coptic text to the specified font.
        /// </summary>
        /// <param name="input">The text to convert.</param>
        /// <param name="target">The font to convert to. Defaults to <see cref="CopticUnicode"/>.</param>
        /// <returns>The source text represented with the target font.</returns>
        public string Convert(string input, CopticFont target = null)
        {
            // Default to Unicode target
            target ??= CopticUnicode;

            // No need to convert
            if (this == target)
                return input;

            string output = "";

            // Generate a dictionary that has the start mapping as keys and the target mapping as values
            Dictionary<string, string> mergedMap = new Dictionary<string, string>();
            var sourceMap = DictionaryTools.SwitchColumns(Charmap);
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

            if (IsJenkimBefore)
            {
                bool accent = false;
                foreach (char ch in input)
                {
                    if (mergedMap.ContainsKey(ch.ToString()))
                    {
                        // Find the source character in the target mapping and write it
                        if (ch == JenkimCharacter)
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
                        if (ch == JenkimCharacter)
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
        /// Attempts to find a font with the specific name in <see cref="Fonts"/>.
        /// </summary>
        /// <param name="fontName">The name of the font to find.</param>
        /// <returns>
        /// The font if found, <see langword="null"/> if no match is found.
        /// </returns>
        public static CopticFont FindFont(string fontName)
        {
            return Fonts.Find(f => f.Name.Equals(fontName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Attempts to find a font with the specific name in <see cref="Fonts"/>.
        /// </summary>
        /// <param name="fontName">The name of the font to find.</param>
        /// <param name="font">The matching font, if found.</param>
        /// <returns>
        /// <see langword="true"/> if the fond was found, <see langword="false"/> if not.
        /// </returns>

        public static bool TryFindFont(string fontName, out CopticFont font)
        {
            font = FindFont(fontName);
            return font != null;
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
                string[] columns = line.Split(',');

                if (columns[0] == "title")
                    font.Name = columns[1];
                else if (columns[0] == "fontName")
                    font.FontName = columns[1];
                else
                    font.Charmap.Add(columns[0], columns[1]);
            }

            return font;
        }
    }
}
