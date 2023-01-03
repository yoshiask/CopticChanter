using CoptLib;
using CoptLib.IO;
using CoptLib.Models;
using CoptLib.Models.Text;
using CoptLib.Scripting;
using CoptLib.Scripting.Commands;
using CoptLib.Writing;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace CoptTest
{
    public class Scripting
    {
        private readonly Doc _doc = new();
        private readonly ITestOutputHelper _output;

        public Scripting(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [MemberData(nameof(GetRunScript_Samples))]
        public void RunCSScript(string script, Func<object> expectedFunc)
        {
            var actual = ScriptingEngine.RunScript(script);
            var expected = expectedFunc();

            // Memberwise comparison
            if (expected is not null)
            {
                Assert.IsType(expected.GetType(), actual);

                var expectedProps = expected.GetType().GetProperties();
                var actualProps = actual.GetType().GetProperties();
                foreach ((var ex, var ac) in expectedProps.Zip(actualProps))
                {
                    Assert.Equal(ex.GetValue(expected), ac.GetValue(actual));
                }
            }
            else
            {
                Assert.Equal(expected, actual);
            }

            _output.WriteLine(actual?.ToString() ?? "{x:Null}");
        }

        [Theory]
        [InlineData("This is some English, {0}with a millisecond offset.")]
        public void ParseTextCommands_TimestampCommand(string text)
        {
            Stanza stanza = new(null)
            {
                SourceText = string.Format(text, @"\ms{0:5:0}"),
                DocContext = _doc
            };
            stanza.HandleCommands();

            Assert.Equal(string.Format(text, string.Empty), stanza.GetText());
            Assert.True(stanza.Commands.Any());
        }

        [Theory]
        [InlineData("this is also some English", null, null, KnownLanguage.English)]
        [InlineData("Ⲉⲩⲗⲟⲅⲟⲛ ⲧⲟⲛ Ⲕⲩⲣⲓⲟⲛ", null, null, KnownLanguage.Coptic)]
        [InlineData("Eulogon ton Kurion", "Ⲉⲩⲗⲟⲅⲟⲛ ⲧⲟⲛ Ⲕⲩⲣⲓⲟⲛ", "CS Avva Shenouda", KnownLanguage.Coptic)]
        public void ParseTextCommands_LanguageCommand(string subtext, string? convSubtext = null, string? font = null, KnownLanguage lang = default)
        {
            convSubtext ??= subtext;

            // Construct command source
            string cmdText = @"\language{" + lang;
            if (font != null)
                cmdText += "|" + font;
            cmdText += "|" + subtext + "}";

            Stanza stanza = new(null)
            {
                SourceText = $"Bless the Lord, {cmdText}.",
                DocContext = _doc
            };
            stanza.HandleCommands();

            Assert.Equal($"Bless the Lord, {convSubtext}.", stanza.GetText());

            var cmd = stanza.Commands.Single();
            var langCmd = Assert.IsType<LanguageCmd>(cmd);
            var langDef = Assert.IsAssignableFrom<Run>(cmd.Output);

            Assert.Equal(langCmd.Language?.Known, lang);
            Assert.Equal(langDef?.ToString(), convSubtext);
            if (font == null)
                Assert.Null(langCmd.Font);
            Assert.Equal(langCmd.Font?.Name, font);
        }

        [Theory]
        [InlineData("engTest", "An English string")]
        [InlineData("coptCSTest", "Nenio] `n`apoctoloc", "Ⲛⲉⲛⲓⲟϯ ⲛ̀ⲁ̀ⲡⲟⲥⲧⲟⲗⲟⲥ", "CS Copt", KnownLanguage.Coptic)]
        [InlineData("coptUniTest", "Ⲛⲉⲛⲓⲟϯ ⲛ̀ⲁ̀ⲡⲟⲥⲧⲟⲗⲟⲥ", null, null, KnownLanguage.Coptic)]
        [InlineData("coptUniTest", "Ⲛⲉⲛⲓⲟϯ ⲛ̀ⲁ̀ⲡⲟⲥⲧⲟⲗⲟⲥ", null, "Coptic Unicode", KnownLanguage.Coptic)]
        public void ParseTextCommands_DefinitionCommand_String(string key, string value, string? parsedValue = null, string? font = null, KnownLanguage lang = default)
        {
            const string preText = "Howdy! Here's some text from a definition: '";
            const string postText = "'.\r\nAlong with some text after.";
            parsedValue ??= value;

            _doc.DirectDefinitions = new()
            {
                new Stanza(null)
                {
                    DocContext = _doc,
                    Key = key,
                    SourceText = value,
                    Font = font,
                    Language = new(lang)
                }
            };
            DocReader.ApplyDocTransforms(_doc);

            Stanza stanza = new(null)
            {
                SourceText = preText + $"\\def{{{key}}}" + postText,
                DocContext = _doc
            };
            stanza.HandleCommands();

            Assert.Equal($"{preText}{parsedValue}{postText}", stanza.GetText());

            var cmd = stanza.Commands.Single();
            var defCmd = Assert.IsType<DefinitionCmd>(cmd);
            var defContent = Assert.IsAssignableFrom<IContent>(defCmd.Output);
            var defMulti = Assert.IsAssignableFrom<IMultilingual>(defCmd.Output);

            Assert.Equal(defContent?.ToString(), parsedValue);
            Assert.Equal(defMulti.Language?.Known, lang);
            Assert.Equal(defMulti.Font, font);
        }

        [Theory]
        [InlineData("A Coptic word, \\lang{Coptic|CS Avva Shenouda|Wcanna}, and its IPA transcription, \\ipa{\\lang{Coptic|CS Avva Shenouda|Wcanna}}.",
                    "A Coptic word, Ⲱⲥⲁⲛⲛⲁ, and its IPA transcription, O\u031Esännä.")]
        [InlineData("IPA transcription of a Coptic word, \\ipa{\\lang{Coptic|CS Avva Shenouda|Wcanna}}.",
                    "IPA transcription of a Coptic word, O\u031Esännä.")]
        [InlineData("English transcription of a Coptic word, \\trslit{English|\\lang{Coptic|CS Avva Shenouda|Wcanna}}.",
                    "English transcription of a Coptic word, Osanna.")]
        public void ParseTextCommands_NestedCommands(string text, string? expectedResult = null)
        {
            expectedResult ??= text;
            Run run = new(text, null);

            var parsedInlines = ScriptingEngine.ParseTextCommands(run);
            Assert.Equal(text, parsedInlines?.ToString());

            var cmds = ScriptingEngine.RunTextCommands(parsedInlines);
            Assert.Equal(expectedResult, parsedInlines?.ToString());
        }

        public static IEnumerable<object[]> GetRunScript_Samples()
        {
            return new object[][]
            {
                new object[]
                {
                    "IDefinition Get() => new SimpleContent(\"Test content\", null);",
                    () => new SimpleContent("Test content", null)
                },
                new object[]
                {
                    """
                    IDefinition GetAkiAktonk()
                    {
                        // https://tasbeha.org/community/discussion/13753/aki-or-aktonk-etc
                        var Today = DateHelper.NowCoptic();
                        int year = Today.YearOfEra;

                        string engText, copText, araText;

                        var resur = CopticCalendar.Resurrection(year);
                        var holyFiftyDays = Period.DaysBetween(resur, Today);
                        bool isHolyFiftyDays = holyFiftyDays > 0 && holyFiftyDays <= 50;

                        var pentecost = CopticCalendar.Pentecost(year);
                        var koiahk = DateHelper.NewCopticDate(year, 4, 1);
                        bool isPentecostKiahk = Today >= pentecost && Today < koiahk;

                        if (isHolyFiftyDays || isPentecostKiahk || Today.Day == 29)
                        {
                            engText = "have risen";
                            copText = "ⲁⲕⲧⲱⲛⲕ";
                            araText = "قمت";
                        }
                        else if (Today == CopticCalendar.Nativity(year))
                        {
                            engText = "were born";
                            copText = "ⲁⲩⲙⲁⲥⲕ";
                            araText = "TODO";
                        }
                        else if (Today == CopticCalendar.Theophany(year))
                        {
                            engText = "were baptised";
                            copText = "ⲁⲕϭⲓⲱⲙⲥ";
                            araText = "TODO";
                        }
                        else if (Today == CopticCalendar.FirstFeastCross(year)
                            || Today == CopticCalendar.SecondFeastCross(year)
                            || CopticCalendar.PaschalPeriod(year).IsDuring(Today))
                        {
                            engText = "were crucified";
                            copText = "ⲁⲩⲁϣⲕ";
                            araText = "TODO";
                        }
                        else
                        {
                            engText = "have come";
                            copText = "ⲁⲕⲓ\u0300";
                            araText = "اتيت";
                        }

                        var res = new TranslationRunCollection("AkiAktonk");
                        res.AddNew(engText, KnownLanguage.English);
                        res.AddNew(copText, KnownLanguage.Coptic);
                        res.AddNew(araText, KnownLanguage.Arabic);

                        return res;
                    }
                    """,
                    GetAkiAktonk
                },
                new object[]
                {
                    """
                    IDefinition GetContent()
                    {
                        var today = DateHelper.NowCoptic();
                        if (today == CopticCalendar.Resurrection(today.YearOfEra))
                            return new SimpleContent("aktonk", null);
                        else
                            return new SimpleContent("aki", null);
                    }
                    """,
                    () =>
                    {
                        var today = DateHelper.NowCoptic();
                        if (today == CopticCalendar.Resurrection(today.YearOfEra))
                            return new SimpleContent("aktonk", null);
                        else
                            return new SimpleContent("aki", null);
                    }
                },
            };
        }

        private static IDefinition GetAkiAktonk()
        {
            // https://tasbeha.org/community/discussion/13753/aki-or-aktonk-etc
            var Today = DateHelper.NowCoptic();
            int year = Today.YearOfEra;

            string engText, copText, araText;

            var resur = CopticCalendar.Resurrection(year);
            var holyFiftyDays = Period.DaysBetween(resur, Today);
            bool isHolyFiftyDays = holyFiftyDays > 0 && holyFiftyDays <= 50;

            var pentecost = CopticCalendar.Pentecost(year);
            var koiahk = DateHelper.NewCopticDate(year, 4, 1);
            bool isPentecostKiahk = Today >= pentecost && Today < koiahk;

            if (isHolyFiftyDays || isPentecostKiahk || Today.Day == 29)
            {
                engText = "have risen";
                copText = "ⲁⲕⲧⲱⲛⲕ";
                araText = "قمت";
            }
            else if (Today == CopticCalendar.Nativity(year))
            {
                engText = "were born";
                copText = "ⲁⲩⲙⲁⲥⲕ";
                araText = "TODO";
            }
            else if (Today == CopticCalendar.Theophany(year))
            {
                engText = "were baptised";
                copText = "ⲁⲕϭⲓⲱⲙⲥ";
                araText = "TODO";
            }
            else if (Today == CopticCalendar.FirstFeastCross(year)
                || Today == CopticCalendar.SecondFeastCross(year)
                || CopticCalendar.PaschalPeriod(year).IsDuring(Today))
            {
                engText = "were crucified";
                copText = "ⲁⲩⲁϣⲕ";
                araText = "TODO";
            }
            else
            {
                engText = "have come";
                copText = "ⲁⲕⲓ\u0300";
                araText = "اتيت";
            }

            var res = new TranslationRunCollection("AkiAktonk");
            res.AddNew(engText, KnownLanguage.English);
            res.AddNew(copText, KnownLanguage.Coptic);
            res.AddNew(araText, KnownLanguage.Arabic);

            return res;
        }
    }
}