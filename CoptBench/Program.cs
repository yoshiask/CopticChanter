using BenchmarkDotNet.Running;
using System;
using System.IO;
using CoptLib.Hyperspeed.IO;
using CoptLib.IO;

namespace CoptBench;

internal class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.Unicode;

        foreach (var docXmlPath in Directory.EnumerateFiles(
                     @"C:\Users\jjask\AppData\Local\Packages\52374YoshiAskharoun.CopticChanter_cw7dt8czrsdfe\RoamingState"))
        {
            if (Path.GetExtension(docXmlPath) != ".xml")
                continue;
            
            var docBinPath = Path.ChangeExtension(docXmlPath, ".bin");
            try
            {
                using (var docXmlStream = File.OpenRead(docXmlPath))
                using (var docBinStream = File.OpenWrite(docBinPath))
                {
                    var doc = DocReader.ReadDocXml(docXmlStream);
                    HyperspeedDocWriter.WriteDefinition(docBinStream, doc);
                }

                File.Delete(docXmlPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                File.Delete(docBinPath);
            }
        }

        //var interpreter = BenchmarkRunner.Run<Interpreter>();
    }
}