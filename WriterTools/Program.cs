using System;
using System.Collections.Generic;
using System.IO;

namespace WriterTools
{
    class Program
    {
        static void Main(string[] args)
        {
            string logPath = @"C:\Users\jjask\Desktop\log.txt";
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            //Console.SetOut(new StreamWriter(new FileStream(logPath, FileMode.Create)));

            var parameters = ParseArgs(args);
            string convertedText = CoptLib.CopticInterpreter.ConvertFont(
                parameters[""],
                CoptLib.CopticFont.CSAvvaShenouda,
                CoptLib.CopticFont.CopticUnicode
            );
            Console.WriteLine(convertedText);
            File.WriteAllText(logPath, convertedText, System.Text.Encoding.Unicode);
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
                    if (strNext.StartsWith("-"))
                        output.Add(str, null);
                    else
                        output.Add(str, strNext);
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
                        if (strNext.StartsWith("-"))
                            output.Add(flags[0].ToString(), null);
                        else
                            output.Add(flags[0].ToString(), strNext);
                    }
                }
                else if (i == 0)
                {
                    output.Add("", str);
                }
            }
            return output;
        }
    }
}
