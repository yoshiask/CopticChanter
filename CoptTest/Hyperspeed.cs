using System.IO;
using CoptLib.Hyperspeed.IO;
using CoptLib.Models;
using CoptLib.Writing;
using Xunit;
using Xunit.Abstractions;

namespace CoptTest;

public class Hyperspeed
{
    private readonly ITestOutputHelper _output;

    public Hyperspeed(ITestOutputHelper output)
    {
        _output = output;
    }

    [Theory]
    [MemberData(nameof(HyperspeedDefinitionSamples))]
    public void WriteHyperspeedDefinition(string id, IDefinition def)
    {
        using var outStream = Resource.OpenTestResult($"hyperspeed_{id}.bin");
        HyperspeedDocWriter.WriteDefinition(outStream, def);
    }

    [Theory]
    [MemberData(nameof(HyperspeedDefinitionSamples))]
    public void ReadHyperspeedDefinition(string id, IDefinition defEx)
    {
        using var inStream = Resource.OpenTestResult($"hyperspeed_{id}.bin", FileMode.Open);

        var defAc = HyperspeedDocReader.ReadDefinition(inStream);
        Helpers.MemberwiseAssertEqual(defEx, defAc);
    }

    public static TheoryData<string, IDefinition> HyperspeedDefinitionSamples()
    {
        return new()
        {
            {
                "stanza",
                new Stanza(null)
                {
                    SourceText = "howdy!",
                }
            },
            {
                "section_en",
                new Section(null)
                {
                    Children =
                    {
                        new Stanza(null)
                        {
                            SourceText = "Glory to the Father and to the Son and to the Holy Spirit."
                        },
                        new Stanza(null)
                        {
                            SourceText = "Now and ever and unto the ages of the ages. Amen."
                        },
                    }
                }
            },
            {
                "section_cop",
                new Section(null)
                {
                    Language = new(KnownLanguage.Coptic),
                    Children =
                    {
                        new Stanza(null)
                        {
                            SourceText = "Ⲁⲙⲏⲛ: ⲁⲗⲗⲏⲗⲟⲩⲓⲁ. Ⲇⲟⲝⲁ Ⲡⲁⲧⲣⲓ ⲕⲉ Ⲩ\u0300ⲓⲱ ⲕⲉ ⲁ\u0300ⲅⲓⲱ Ⲡ\u0300ⲛⲉⲩⲙⲁⲧⲓ."
                        },
                        new Stanza(null)
                        {
                            SourceText = "Ⲕⲉ ⲛⲩⲛ ⲕⲉ ⲁ\u0300ⲓ\u0300 ⲕⲉ ⲓⲥ ⲧⲟⲩⲥ ⲉ\u0300ⲱ\u0300ⲛⲁⲥ ⲧⲱⲛ ⲉ\u0300ⲱ\u0300ⲛⲱⲛ: ⲁ\u0300ⲙⲏⲛ."
                        },
                    }
                }
            },
            {
                "translations",
                new Section(null)
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
                                    SourceText = "Blessed are you among women, and blessed is your Fruit, O Mary the Mother of God, the undefiled Virgin."
                                },
                                new Stanza(null)
                                {
                                    SourceText = "For the Sun of Righteousness, shone unto us from you, with healing under His wings, for He is the Creator."
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
                                    SourceText = @"Te`cmarwout qen nihiomi@ `f`cmarwout `nje Pekarpoc@ `w Maria `:mau `m`Vnou]@ \}par;enoc `nat;wleb."
                                },
                                new Stanza(null)
                                {
                                    SourceText = @"Je afsai nan `ebol `nqy]@ `nje Piry `nte ]me;myi@ `ere pital[o ,y qa neftenh@ je `n;of pe Piref;amio."
                                },
                            }
                        }
                    }
                }
            },
        };
    }
}