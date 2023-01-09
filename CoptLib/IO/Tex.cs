using CoptLib.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CoptLib.IO
{
    /// <summary>
    /// Static helper for generating TeX/LaTeX documents.
    /// </summary>
    public static class Tex
    {
        public static void WriteTex(Doc doc, Stream stream)
        {
            var layout = doc.Flatten();
            int numCols = doc.Translations.Children.Count;

            using StreamWriter writer = new(stream);

            // Start document
            writer.WriteLine(@"\documentclass[../document]{subfiles}");
            writer.WriteLine(@"\begin{document}");
            writer.WriteLine(@"\renewcommand{\arraystretch}{1.5}");

            // Set up columns
            string[] tableColDefs = new string[numCols];
            string[] verseParams = new string[numCols];
            for (int t = 0; t < numCols; t++)
            {
                var trans = doc.Translations.Children[t];
                bool isRtl = trans.Language.Culture.TextInfo.IsRightToLeft;
                string shortLang = trans.Language.Known.ToString().Substring(0, 4);
                string fontCmd = $"\\font{shortLang} #{t + 1}";

                tableColDefs[t] = isRtl ? "R" : "L";

                if (isRtl)
                    fontCmd = $"\\RL{{{fontCmd}}}";
                verseParams[t] = fontCmd;
            }

            // Create verse macro
            string verseCmd = $"\\verseRow";
            writer.Write($"\\newcommand{{{verseCmd}}}[");
            writer.Write(numCols);
            writer.WriteLine("]{");
            writer.Write('\t');
            writer.WriteLine($"{string.Join(" & ", verseParams)}\\\\");
            writer.WriteLine('}');

            // Begin translations
            writer.Write(@"\begin{tabulary}{\textwidth}{ ");
            writer.Write(string.Join(" | ", tableColDefs));
            writer.WriteLine(" }");

            // Write rows
            for (int r = 0; r < layout.Count; r++)
            {
                var row = layout[r];

                if (row.Count == 1 && row[0] is Doc)
                    continue;

                List<string> verse = new(row.Count);

                foreach (var cell in row)
                {
                    object item = cell;

                    if (item is Section section)
                        item = section.Title;

                    if (item is IContent content)
                    {
                        string text = Writing.CopticFont.SwapJenkimPosition(content.GetText(), '\u0300', false);
                        verse.Add(text);
                    }
                }

                if (verse.Count > 0)
                    writer.WriteLine($"\t{verseCmd}{{{string.Join("}{", verse)}}}");
            }

            // End translations
            writer.WriteLine(@"\end{tabulary}");

            // End document
            writer.WriteLine(@"\end{document}");
        }
    }
}
