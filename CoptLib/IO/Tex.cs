using CoptLib.Extensions;
using CoptLib.Models;
using System.Collections.Generic;
using System.IO;

namespace CoptLib.IO
{
    /// <summary>
    /// Static helper for generating TeX/LaTeX documents.
    /// </summary>
    public static class Tex
    {
        public static void WriteTex(Doc doc, Stream stream)
        {
            DocLayout layout = new(doc);
            var table = layout.CreateTable();

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
                string shortLang = GetShortLanguage(trans);
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
            bool isNewDoc = false;
            for (int r = 0; r < table.Count; r++)
            {
                var row = table[r];

                if (row.Count == 1 && row[0] is Doc)
                {
                    isNewDoc = true;
                    continue;
                }

                List<string> verse = new(row.Count);

                foreach (var cell in row)
                {
                    object item = cell;
                    string surround = string.Empty;
                    int insertIdx = 0;

                    if (item is Section section && section.Title is not null)
                    {
                        item = section.Title;

                        string shortLang = GetShortLanguage(section.Title);
                        string headerCmd = (isNewDoc ? @"\Large" : @"\large")
                            + @"\role" + shortLang + @"{fore}{}";

                        surround = surround.Insert(insertIdx, headerCmd);
                        insertIdx += headerCmd.Length - 1;
                    }

                    if (item is Comment)
                    {
                        var headerCmd = @"\small\color{note}";
                        surround = surround.Insert(insertIdx, headerCmd);
                        insertIdx += headerCmd.Length;
                    }

                    if (item is IContent content)
                    {
                        // The jenkims need to be swapped because Segoe UI and League Spartan
                        // expect it before the base letter.
                        string text = Writing.CopticFont.SwapJenkimPosition(content.GetText(), '\u0300', false);
                        text = surround.Insert(insertIdx, text);
                        verse.Add(text);
                    }
                }

                if (verse.Count > 0)
                    writer.WriteLine($"\t{verseCmd}{{{string.Join("}{", verse)}}}");

                isNewDoc = false;
            }

            // End translations
            writer.WriteLine(@"\end{tabulary}");

            // End document
            writer.WriteLine(@"\end{document}");
        }

        private static string GetShortLanguage(IDefinition multi)
        {
            var shortLang = multi.GetLanguage().Known.ToString();
            return shortLang.Length > 4 ? shortLang[..4] : shortLang;
        }
    }
}
