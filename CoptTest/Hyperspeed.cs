using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CoptLib.Hyperspeed.IO;
using CoptLib.IO;
using CoptLib.Models;
using CoptLib.Writing;
using Xunit;
using Xunit.Abstractions;

namespace CoptTest;

public class Hyperspeed
{
    private readonly ITestOutputHelper _output;
    private readonly string[] _testSetFileNames =
    {
        "Let Us Praise the Lord.xml",
        "The First Hoos Lobsh.xml",
        "The Friday Theotokia.xml"
    };

    public Hyperspeed(ITestOutputHelper output)
    {
        _output = output;
    }

    [Theory]
    [MemberData(nameof(HyperspeedDefinitionSamples))]
    public void WriteHyperspeedDefinition(string id, Func<IDefinition> defFact)
    {
        using var outStream = Resource.OpenTestResult($"hyperspeed_{id}.bin");
        HyperspeedBinaryWriter writer = new(outStream);
        writer.WriteObject(defFact());
    }

    [Theory]
    [MemberData(nameof(HyperspeedDefinitionSamples))]
    public void ReadHyperspeedDefinition(string id, Func<IDefinition> defExFact)
    {
        using var inStream = Resource.OpenTestResult($"hyperspeed_{id}.bin", FileMode.Open);
        HyperspeedBinaryReader reader = new(inStream);
        
        var defAc = reader.ReadDefinition();
        _output.WriteLine(defAc?.ToString());
        Helpers.MemberwiseAssertEqual(defExFact(), defAc);
    }

    [Fact]
    public void WriteHyperspeedSet()
    {
        List<Doc> docs = new(_testSetFileNames
            .Select(Resource.ReadAllText)
            .Select(x => DocReader.ParseDocXml(x)));

        DocSet set = new("urn:coptlib-hyper:test-set", "Test Set", docs);
        set.Author = new()
        {
            FullName = "Yoshi Askharoun",
            Email = "jjask7@gmail.com",
            Website = "https://github.com/yoshiask"
        };
        
        using var outStream = Resource.OpenTestResult("hyperspeed_set.bin");
        HyperspeedBinaryWriter writer = new(outStream);
        writer.Write(set);
    }

    public static TheoryData<string, Func<IDefinition>> HyperspeedDefinitionSamples()
    {
        return new()
        {
            {
                "stanza",
                () => new Stanza(null)
                {
                    SourceText = "howdy!",
                }
            },
            {
                "section_en",
                () =>
                {
                    Section root = new(null);
                    
                    root.Children.Add(new Stanza(root)
                    {
                        SourceText = "Glory to the Father and to the Son and to the Holy Spirit."
                    });
                    root.Children.Add(new Stanza(root)
                    {
                        SourceText = "Now and ever and unto the ages of the ages. Amen."
                    });
                    
                    return root;
                }
            },
            {
                "section_cop",
                () =>
                {
                    Section root = new(null)
                    {
                        Language = new(KnownLanguage.Coptic)
                    };
                    
                    root.Children.Add(new Stanza(root)
                    {
                        SourceText = "Ⲁⲙⲏⲛ: ⲁⲗⲗⲏⲗⲟⲩⲓⲁ. Ⲇⲟⲝⲁ Ⲡⲁⲧⲣⲓ ⲕⲉ Ⲩ\u0300ⲓⲱ ⲕⲉ ⲁ\u0300ⲅⲓⲱ Ⲡ\u0300ⲛⲉⲩⲙⲁⲧⲓ."
                    });
                    root.Children.Add(new Stanza(root)
                    {
                        SourceText = "Ⲕⲉ ⲛⲩⲛ ⲕⲉ ⲁ\u0300ⲓ\u0300 ⲕⲉ ⲓⲥ ⲧⲟⲩⲥ ⲉ\u0300ⲱ\u0300ⲛⲁⲥ ⲧⲱⲛ ⲉ\u0300ⲱ\u0300ⲛⲱⲛ: ⲁ\u0300ⲙⲏⲛ."
                    });
                    
                    return root;
                }
            },
            {
                "translations",
                () =>
                {
                    Section translations = new(null);

                    Section en = new(translations)
                    {
                        Language = new(KnownLanguage.English)
                    };
                    en.Children.Add(new Stanza(en)
                    {
                        SourceText = "Blessed are you among women, and blessed is your Fruit, O Mary the Mother of God, the undefiled Virgin."
                    });
                    en.Children.Add(new Stanza(en)
                    {
                        SourceText = "For the Sun of Righteousness, shone unto us from you, with healing under His wings, for He is the Creator."
                    });
                    translations.Children.Add(en);

                    Section cop = new(translations)
                    {
                        Language = new(KnownLanguage.Coptic),
                        Font = "CopticStandard"
                    };
                    cop.Children.Add(new Stanza(cop)
                    {
                        SourceText = @"Te`cmarwout qen nihiomi@ `f`cmarwout `nje Pekarpoc@ `w Maria `:mau `m`Vnou]@ \}par;enoc `nat;wleb."
                    });
                    cop.Children.Add(new Stanza(cop)
                    {
                        SourceText = @"Je afsai nan `ebol `nqy]@ `nje Piry `nte ]me;myi@ `ere pital[o ,y qa neftenh@ je `n;of pe Piref;amio."
                    });
                    translations.Children.Add(cop);

                    translations.HandleFont();
                    return translations;
                }
            },
        };
    }
}