using CoptLib.Models;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using CoptLib.Extensions;

#if DEBUG
using Output = System.Diagnostics.Debug;
#else
using Output = System.Console;
#endif

namespace CoptLib.Writing
{
    public partial class DisplayFont
    {
        public DisplayFont(string displayName, string fontFamily, string characterMapId, DoubleDictionary<KnownCharacter, char> charmap, bool isJenkimBefore)
        {
            DisplayName = displayName;
            FontFamily = fontFamily;
            CharacterMapId = characterMapId;
            Charmap = charmap;
            IsJenkimBefore = isJenkimBefore;
        }

        public string DisplayName { get; set; }
        public string FontFamily { get; set; }
        public string CharacterMapId { get; }
        public DoubleDictionary<KnownCharacter, char> Charmap { get; }
        public bool IsJenkimBefore { get; set; }

        /// <summary>
        /// Converts text to the specified font.
        /// </summary>
        /// <param name="input">The text to convert.</param>
        /// <param name="target">The font to convert to. Defaults to <see cref="Unicode"/>.</param>
        /// <returns>The source text represented with the target font.</returns>
        public string Convert(string input, DisplayFont? target = null)
        {
            // Default to Unicode target
            target ??= Unicode;

            // No need to convert
            if (CharacterMapId == target.CharacterMapId)
                return input;

            var newChars = input
                .Select(ch => Convert(ch, target))
                .ToArray();
            string output = new(newChars);

            // Swap jenkim position if required
            return IsJenkimBefore != target.IsJenkimBefore
                ? SwapJenkimPosition(output, target.Charmap[KnownCharacter.CombiningGraveAccent], IsJenkimBefore)
                : output;
        }

        /// <summary>
        /// Converts text to the specified font.
        /// </summary>
        /// <param name="input">The character to convert.</param>
        /// <param name="target">The font to convert to. Defaults to <see cref="Unicode"/>.</param>
        /// <returns>The source text represented with the target font.</returns>
        public char Convert(char input, DisplayFont? target = null)
        {
            target ??= Unicode;
            
            return Charmap.TryGetLeft(input, out var knownCh) 
                && target.Charmap.TryGetRight(knownCh, out var targetCh)
                    ? targetCh
                    : input;
        }

        /// <summary>
        /// Save Font Pack to file. Also adds to imported list of the current instance.
        /// </summary>
        /// <param name="path">Location to save file</param>
        /// <param name="addToList">Whether to add the created font to the current <see cref="Fonts"/> list.</param>
        /// <returns></returns>
        public bool TrySaveFontXml(string path, bool addToList)
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
        /// <param name="characterMapId">The name of the font to find.</param>
        /// <returns>
        /// The font if found, <see langword="null"/> if no match is found.
        /// </returns>
        public static DisplayFont? FindFontByMapId(string characterMapId)
        {
            return Fonts.Find(f => f.CharacterMapId.Equals(characterMapId, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Attempts to find a font with the specific name in <see cref="Fonts"/>.
        /// </summary>
        /// <param name="characterMapId">The name of the font to find.</param>
        /// <param name="getDefaultFont">A function that returns the font to use if none is found.</param>
        /// <returns>
        /// The font if found, <paramref name="getDefaultFont"/>() if no match is found.
        /// </returns>
        public static DisplayFont FindFontByMapId(string characterMapId, Func<DisplayFont> getDefaultFont)
        {
            return Fonts.Find(f => f.CharacterMapId.Equals(characterMapId, StringComparison.OrdinalIgnoreCase))
                ?? getDefaultFont();
        }

        /// <summary>
        /// Attempts to find a font with the specific name in <see cref="Fonts"/>.
        /// </summary>
        /// <param name="characterMapId">The name of the font to find.</param>
        /// <param name="font">The matching font, if found.</param>
        /// <returns>
        /// <see langword="true"/> if the fond was found, <see langword="false"/> if not.
        /// </returns>
        public static bool TryFindFontByMapId(string characterMapId, [NotNullWhen(true)] out DisplayFont? font)
        {
            font = FindFontByMapId(characterMapId);
            return font != null;
        }

        /// <summary>
        /// Swaps the position of all jenkims, placing them either before or after the target letter.
        /// </summary>
        /// <param name="jenkim">
        /// The character that represents a jenkim.
        /// </param>
        /// <param name="text">
        /// The text to modify.
        /// </param>
        /// <param name="isJenkimBefore">
        /// Whether <paramref name="text"/> has jenkims before the target letter.
        /// </param>
        /// <returns>
        /// Text similar to the input, but with the jenkim position reversed.
        /// </returns>
        public static string SwapJenkimPosition(string text, char jenkim, bool isJenkimBefore)
        {
            // Adjust bounds depending on look-ahead or look-behind
            int i = isJenkimBefore ? 0 : 1;
            int endIdx = isJenkimBefore ? text.Length - 1 : text.Length;
            int peek = isJenkimBefore ? 1 : -1;

            // Save text to char array
            var chars = text.ToCharArray();

            for (; i < endIdx; i++)
            {
                char ch = chars[i];
                if (jenkim != ch)
                    continue;

                // Look at the target character
                char chTarget = chars[i + peek];

                // Swap letters
                chars[i] = chTarget;
                chars[i + peek] = ch;

                if (isJenkimBefore) ++i;
            }

            return new(chars);
        }

        /// <summary>
        /// Swaps the position of all jenkims, placing them either before or after the target letter.
        /// </summary>
        /// <param name="text">
        /// The text to modify.
        /// </param>
        /// <param name="font">
        /// The font to convert with.
        /// </param>
        /// <returns>
        /// Text similar to the input, but with the jenkim position reversed.
        /// </returns>
        /// <remarks>
        /// Note that calling this overload more than once may result in some odd results.
        /// </remarks>
        public static string SwapJenkimPosition(string text, DisplayFont font)
            => SwapJenkimPosition(text, font.Charmap[KnownCharacter.CombiningGraveAccent], font.IsJenkimBefore);

        /// <inheritdoc cref="SwapJenkimPosition(string, DisplayFont)"/>
        public static string SwapJenkimPosition(string text, string fontName)
            => SwapJenkimPosition(text, FindFontByMapId(fontName) ?? Unicode);

        /// <summary>
        /// Reads Font Pack from file. Also adds to imported list of the current instance.
        /// </summary>
        /// <param name="path">File to read</param>
        /// <param name="addToList">Whether to add the created font to the current <see cref="Fonts"/> list.</param>
        /// <returns></returns>
        public static DisplayFont ReadFontXml(string path, bool addToList = true)
        {
            // Create an instance of the XmlSerializer class;
            // specify the type of object to be deserialized.
            XmlSerializer serializer = new XmlSerializer(typeof(Font));

            // A FileStream is needed to read the XML document.
            var text = File.OpenRead(path);

            // Use the Deserialize method to restore the object's state with
            // data from the XML document.
            var data = ((Font)serializer.Deserialize(text)).ToDisplayFont();
            if (addToList)
                Fonts.Add(data);
            return data;
        }

        /// <summary>
        /// Generates a <see cref="DisplayFont"/> from a CSV file. Does not import generated font. 
        /// </summary>
        /// <param name="path">File to generate from. Must have comma separated values.</param>
        /// <returns></returns>
        public static DisplayFont GenerateFromCsv(string path, string displayName, string fontFamily, string charMapId)
        {
            var font = new DisplayFont(displayName, fontFamily, charMapId, new DoubleDictionary<KnownCharacter, char>(), false);
            
            var lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                var columns = line.Split(',');

                switch (columns[0])
                {
                    case "title":
                        font.DisplayName = columns[1];
                        break;
                    case "fontName":
                        font.FontFamily = columns[1];
                        break;
                    default:
                        font.Charmap.Add(EnumExtensions.Parse<KnownCharacter>(columns[0]), columns[1][0]);
                        break;
                }
            }

            return font;
        }
    }
}
