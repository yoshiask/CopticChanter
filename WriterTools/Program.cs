using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using CoptLib;

namespace WriterTools
{
    class Program
    {
        static void Main(string[] args)
        {
            string logPath = @"C:\Users\jjask\Desktop\log.txt";
            Console.OutputEncoding = System.Text.Encoding.Unicode;

            var parameters = ParseArgs(args);
            CopticFont sourceFont = CopticFont.CsAvvaShenouda;
            CopticFont targetFont = CopticFont.CopticUnicode;
            if (parameters.ContainsKey("source-font"))
            {
                sourceFont = CopticFont.Fonts.Find((f) => f.Name == parameters["source-font"]);
            }
            if (parameters.ContainsKey("target-font"))
            {
                targetFont = CopticFont.Fonts.Find((f) => f.Name == parameters["target-font"]);
            }

            string output = "";
            if (parameters.ContainsKey("source-txt"))
            {
                try
                {
                    output = CopticInterpreter.ConvertFont(
                        File.ReadAllText(parameters["source-file"]), sourceFont, targetFont
                    );
                }
                catch (FileNotFoundException)
                {
                    Console.Error.WriteLine("Unable to read the source file");
                }
            }
            else if (parameters.ContainsKey(""))
            {
                output = CopticInterpreter.ConvertFont(
                    parameters[""], sourceFont, targetFont
                );
            }
            else if (parameters.ContainsKey("source-csv"))
            {
                try
                {
                    // Get the column to convert
                    int columnNum = 0;
                    if (parameters.ContainsKey("csv-column"))
                        columnNum = Int32.Parse(parameters["csv-column"]);

                    // Read the specified column
                    List<string> inputText = new List<string>();
                    var table = ReadCsvFile(parameters["source-csv"]);
                    foreach (List<string> row in table)
                    {
                        inputText.Add(row[columnNum]);
                    }

                    // Convert each line
                    foreach (string input in inputText)
                    {
                        output += CopticInterpreter.ConvertFont(
                            input, sourceFont, targetFont
                        );
                        output += "\r\n";
                    }
                }
                catch (FileNotFoundException)
                {
                    Console.Error.WriteLine("Unable to read the source file");
                }
            }

            Console.WriteLine(output);
            File.WriteAllText(logPath, output, System.Text.Encoding.Unicode);
        }

        static Dictionary<string, string> ParseArgs(string[] args)
        {
            var output = new Dictionary<string, string>();
            for (int i = 0; i < args.Length; i++)
            {
                string str = args[i];
                string strNext = "";
                if (i + 1 < args.Length)
                {
                    strNext = args[i + 1];
                }
                if (str.StartsWith("--"))
                {
                    // This is a full flag. Check if it has a parameter or not
                    output.Add(str.Remove(0, 2), strNext.StartsWith("-") ? null : strNext);
                }
                else if (str.StartsWith("-"))
                {
                    // This is a single-character flag, split into individual characters and treat
                    // each one as if it were separate
                    char[] flags = str.Remove(0, 1).ToCharArray();
                    if (flags.Length > 1)
                    {
                        foreach (char fl in flags)
                        {
                            output.Add(fl.ToString(), null);
                        }
                    }
                    else
                    {
                        output.Add(flags[0].ToString(), strNext.StartsWith("-") ? null : strNext);
                    }
                }
                else if (i == 0)
                {
                    output.Add("", str);
                }
            }
            return output;
        }

        /// <summary>
        /// Reads the given CSV file and returns a list of rows
        /// </summary>
        static List<List<string>> ReadCsvFile(string path)
        {
            var output = new List<List<string>>();

            foreach (string line in File.ReadAllLines(path))
            {
                output.Add(line.Split(",").ToList());
            }

            return output;
        }
    }
}
