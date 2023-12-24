using BenchmarkDotNet.Running;
using System;
using System.IO;
using CoptLib.Hyperspeed.IO;
using CoptLib.Models;
using CoptLib.Writing;

namespace CoptBench;

internal class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.Unicode;
        TestHyperspeed(@"D:\Repos\yoshiask\CopticChanter\CoptTest\bin\Debug\net6.0\Output\hyperspeed_translations.bin");

        return;
        var interpreter = BenchmarkRunner.Run<Interpreter>();
    }

    static void TestHyperspeed(string path)
    {
        var def = new TranslationCollectionSection(null)
        {
            Children =
            {
                new Section(null)
                {
                    Language = new(KnownLanguage.English),
                    Children =
                    {
                        new Stanza(null)
                        {
                            SourceText =
                                "Blessed are you among women, and blessed is your Fruit, O Mary the Mother of God, the undefiled Virgin."
                        },
                        new Stanza(null)
                        {
                            SourceText =
                                "For the Sun of Righteousness, shone unto us from you, with healing under His wings, for He is the Creator."
                        },
                    }
                },
                new Section(null)
                {
                    Language = new(KnownLanguage.Coptic),
                    Font = "CopticStandard",
                    Children =
                    {
                        new Stanza(null)
                        {
                            SourceText =
                                @"Te`cmarwout qen nihiomi@ `f`cmarwout `nje Pekarpoc@ `w Maria `:mau `m`Vnou]@ \}par;enoc `nat;wleb."
                        },
                        new Stanza(null)
                        {
                            SourceText =
                                @"Je afsai nan `ebol `nqy]@ `nje Piry `nte ]me;myi@ `ere pital[o ,y qa neftenh@ je `n;of pe Piref;amio."
                        },
                    }
                }
            }
        };

        using (var outStream = File.OpenWrite(path))
        {
            HyperspeedDocWriter.SerializeDefinition(outStream, def);
        }
        
        using (var inStream = File.OpenRead(path))
        {
            var defAc = HyperspeedDocReader.DeserializeDefinition(inStream);
        }
    }
}