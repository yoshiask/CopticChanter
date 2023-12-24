using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CoptLib;
using CoptLib.IO;
using CoptLib.Models;
using CoptLib.Models.Text;
using CoptLib.Scripting;
using CoptLib.Scripting.Commands;
using CoptLib.Scripting.Typed;
using CoptLib.Writing;
using NodaTime;
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
        [MemberData(nameof(GetRunDotNetScript_Samples))]
        public void RunDotNetDefinitionScript(string scriptBody, Func<LoadContextBase?, IDefinition?> expectedFunc)
        {
            LoadContext context = new();
            context.SetDate(new(2023, 1, 7, 11, 00, CalendarSystem.Gregorian));

            DotNetDefinitionScript script = new(scriptBody);
            script.Execute(context);
            
            var actual = script.Output;
            var expected = expectedFunc(context);

            if (actual is not null && expected is not null)
            {
                expected.Parent = actual.Parent;
                expected.DocContext = actual.DocContext;
            }

            Helpers.MemberwiseAssertEqual(expected, actual);

            _output.WriteLine(actual?.ToString() ?? "{x:Null}");
        }

        [Theory]
        [MemberData(nameof(GetRunLuaScript_Samples))]
        public void RunLuaScript(string scriptBody, Func<LoadContextBase?, IDefinition?> expectedFunc)
        {
            LoadContext context = new();
            context.SetDate(new(2023, 1, 7, 11, 00, CalendarSystem.Gregorian));

            LuaScript script = new(scriptBody);
            script.Execute(context);

            var actual = script.Output;
            var expected = expectedFunc(context);
            Helpers.MemberwiseAssertEqual(expected, actual);

            _output.WriteLine(actual?.ToString() ?? "{x:Null}");
        }

        [Theory]
        [InlineData("Hymn of the Intercessions.xml")]
        public void RunScriptInDoc(string file)
        {
            var xmlAc = Resource.ReadAllText(file);
            var docAc = DocReader.ParseDocXml(XDocument.Parse(xmlAc));
            docAc.ApplyTransforms();
            
            Assert.Equal(3, docAc.DirectDefinitions.Count);

            var englishTranslation = (Section) docAc.Translations[KnownLanguage.English];
            foreach (var content in englishTranslation.Children.OfType<IContent>())
                if (content.SourceText.EndsWith(@"\def{Refrain}"))
                    Assert.EndsWith("grant us the forgiveness of our sins.", content.ToString());
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
        [InlineData("en", "\r\n|The Wise Men learned that a star|which shone brighter than the sun|Had been born and taken flesh|By the Word of the Father")]
        [InlineData("en", "|In the name of the Father,|and of the Son,|and of the Holy Spirit,|one true God.")]
        [InlineData("el", "|Ἐν εἰρήνῃ τοῦ Κυρίου δεηθῶμεν.|Κύριε, ἐλέησον.|Ὑπὲρ τῆς ἄνωθεν εἰρήνης καὶ τῆς σωτηρίας τῶν ψυχῶν ἡμῶν τοῦ Κυρίου δεηθῶμεν.")]
        [InlineData("cop", "|Ⲁⲓⲛⲁϩϯ ⲉⲑⲃⲉ ⲫⲁⲓ|ⲁⲓⲥⲁϫⲓ ϧⲉⲛ ⲟⲩϫⲟⲙ|ⲉⲑⲃⲉ ⲡⲉⲕⲛⲓϣϯ ⲛ̀ⲛⲁⲓ|Ⲡ̀ϭⲟⲓⲥ ⲛ̀ⲧⲉ ⲛⲓϫⲟⲙ.")]
        public void ParseTextCommands_LinesCommand(string lang, string text)
        {
            TranslationRunCollection defaultLineSeparator = new("defaultLineSeparator");
            defaultLineSeparator.AddText(" / ", KnownLanguage.Default);
            defaultLineSeparator.AddText(": ", KnownLanguage.Coptic);
            defaultLineSeparator.AddText("\r\n", KnownLanguage.Greek);

            if (!_doc.Context.Definitions.ContainsKey(defaultLineSeparator.Key!))
                _doc.Context.AddDefinition(defaultLineSeparator, null);

            Stanza stanza = new(_doc)
            {
                SourceText = $"\\lines{{{text}}}",
                Language = LanguageInfo.Parse(lang),
            };
            stanza.HandleCommands();

            _output.WriteLine(stanza.GetText());
            Assert.True(stanza.Commands.Any());
        }

        [Fact]
        public void ParseTextCommands_FontLinesCommand()
        {
            Doc doc = new();
            Stanza stanza = new(doc)
            {
                SourceText = @"\lines{|Pi`sbwt `nte `A`arwn|`etafviri `ebol|,wric [o nem `tco|`foi `ntupoc ne.}",
                Font = "CopticStandard",
                Language = new(KnownLanguage.Coptic)
            };
            
            doc.Translations.Children.Add(stanza);
            doc.ApplyTransforms();
            
            _output.WriteLine(stanza.ToString());
        }

        [Theory]
        [InlineData("this is also some English", null, null, KnownLanguage.English)]
        [InlineData("Ⲉⲩⲗⲟⲅⲟⲛ ⲧⲟⲛ Ⲕⲩⲣⲓⲟⲛ", null, null, KnownLanguage.Coptic)]
        [InlineData("Eulogon ton Kurion", "Ⲉⲩⲗⲟⲅⲟⲛ ⲧⲟⲛ Ⲕⲩⲣⲓⲟⲛ", "Coptic Standard", KnownLanguage.Coptic)]
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

            Assert.Equal(langCmd.Language.Known, lang);
            Assert.Equal(langDef.ToString(), convSubtext);
            if (font == null)
                Assert.Null(langCmd.Font);
            Assert.Equal(langCmd.Font?.DisplayName, font);
        }

        [Theory]
        [InlineData("engTest", "An English string")]
        [InlineData("coptCSTest", "Nenio] `n`apoctoloc", "Ⲛⲉⲛⲓⲟϯ ⲛ̀ⲁ̀ⲡⲟⲥⲧⲟⲗⲟⲥ", "CopticStandard", KnownLanguage.Coptic)]
        [InlineData("coptUniTest", "Ⲛⲉⲛⲓⲟϯ ⲛ̀ⲁ̀ⲡⲟⲥⲧⲟⲗⲟⲥ", null, null, KnownLanguage.Coptic)]
        [InlineData("coptUniTest", "Ⲛⲉⲛⲓⲟϯ ⲛ̀ⲁ̀ⲡⲟⲥⲧⲟⲗⲟⲥ", null, "Coptic Unicode", KnownLanguage.Coptic)]
        public void ParseTextCommands_DefinitionCommand_String(string key, string value, string? parsedValue = null, string? font = null, KnownLanguage lang = default)
        {
            const string preText = "Howdy! Here's some text from a definition: '";
            const string postText = "'.\r\nAlong with some text after.";
            parsedValue ??= value;

            _doc.DirectDefinitions = new List<IDefinition>
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
            _doc.ApplyTransforms();

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

            Assert.Equal(defContent.ToString(), parsedValue);
            Assert.Equal(defMulti.Language.Known, lang);
            Assert.Equal(defMulti.Font, font);
        }

        [Theory]
        [InlineData("Ⲁⲙⲏⲛ {0}.", "al", "ⲁⲗⲗⲏⲗⲟⲩⲓⲁ", "ⲁ\u035Eⲗ", KnownLanguage.Coptic)]
        [InlineData("Ⲁⲙⲏⲛ {0}.", "Al", "Ⲁⲗⲗⲏⲗⲟⲩⲓⲁ", "Ⲁ\u035Eⲗ", KnownLanguage.Coptic)]
        [InlineData("{0} ⲁⲣⲓϩ̀ⲙⲟⲧ ⲛⲁⲛ ⲙ̀ⲡⲓⲭⲱ ⲉ̀ⲃⲟⲗ ⲛ̀ⲧⲉ ⲛⲉⲛⲛⲟⲃⲓ.",
            "Poc", "Ⲡ\u0300ϭⲟⲓⲥ", "Ⲡ\u0300ⲟ\u035Eⲥ", KnownLanguage.Coptic)]
        [InlineData("Ⲧⲉⲛⲟⲩⲱϣⲧ ⲙ̀ⲙⲟⲕ ⲱ̀ {0}: ⲛⲉⲙ Ⲡⲉⲕⲓⲱⲧ ⲛ̀ⲁ̀ⲅⲁⲑⲟⲥ",
            "Pxc", "Ⲡⲓⲭ\u0300ⲣⲓⲥⲧⲟⲥ", "Ⲡⲓⲭ\u035Eⲥ", KnownLanguage.Coptic)]
        [InlineData("Ϩⲓⲧⲉⲛ ⲛⲓⲉ̀ⲩⲭⲏ: ⲛ̀ⲧⲉ ⲛⲏ{0} ⲛ̀ⲧⲉ ⲡⲁⲓⲉ̀ϩⲟⲟⲩ",
            "ethu", "ⲉⲑⲟⲩⲁⲃ", "ⲉ\u035Eⲑ\u035Eⲩ", KnownLanguage.Coptic)]
        public void ParseTextCommands_AbbreviationCommand(string template, string abvKey, string abvEx, string abvSh,
            KnownLanguage lang)
        {
            Stanza stanza = new(null)
            {
                SourceText = string.Format(template, $"\\abv{{{abvKey}}}"),
                Language = new(lang)
            };
            stanza.HandleCommands();
            Assert.Equal(string.Format(template, abvEx), stanza.ToString());
            _output.WriteLine($"Expanded: {stanza}");
            
            stanza.SourceText = string.Format(template, $"\\abvsh{{{abvKey}}}");
            stanza.HandleCommands();
            Assert.Equal(string.Format(template, abvSh), stanza.ToString());
            _output.WriteLine($"Shortened: {stanza}");
        }
        
        [Theory]
        [InlineData("A Coptic word, \\lang{Coptic|CS Avva Shenouda|Wcanna}, and its IPA transcription, \\ipa{\\lang{Coptic|CS Avva Shenouda|Wcanna}}.",
                    "A Coptic word, Ⲱⲥⲁⲛⲛⲁ, and its IPA transcription, Oˌsɑnˌnɑ.")]
        [InlineData("IPA transcription of a Coptic word, \\ipa{\\lang{Coptic|CS Avva Shenouda|Wcanna}}.",
                    "IPA transcription of a Coptic word, Oˌsɑnˌnɑ.")]
        [InlineData("English transcription of a Coptic word, \\trslit{English||\\lang{Coptic|CS Avva Shenouda|Wcanna}}.",
                    "English transcription of a Coptic word, Osănnă.")]
        [InlineData("\\lines{|Seven times everyday,|I will praise Your Name|with all my heart,|O God of everyone.}",
                    "Seven times everyday, / I will praise Your Name / with all my heart, / O God of everyone.")]
        public void ParseTextCommands_NestedCommands(string text, string? expectedResult = null)
        {
            expectedResult ??= text;
            Run run = new(text, null);

            var parsedInlines = ScriptingEngine.ParseTextCommands(run);
            var actual = parsedInlines.ToString();
            Assert.Equal(text, actual);

            _ = ScriptingEngine.RunTextCommands(parsedInlines);
            actual = parsedInlines.ToString();
            _output.WriteLine(actual);
            Assert.Equal(expectedResult, actual);
        }

        public static IEnumerable<object[]> GetRunDotNetScript_Samples()
        {
            return new[]
            {
                new object[]
                {
                    "return new SimpleContent(\"Test content\", null);",
                    (ILoadContext? _) => new SimpleContent("Test content", null)
                },
                new object[]
                {
                    """
                    // https://tasbeha.org/community/discussion/13753/aki-or-aktonk-etc
                    var today = context!.CurrentDate;
                    int year = today.YearOfEra;

                    string engText, copText, araText;

                    if (today == CopticCalendar.Nativity(year))
                    {
                        engText = "were born";
                        copText = "ⲁⲩⲙⲁⲥⲕ";
                        araText = "TODO";
                    }
                    else if (today == CopticCalendar.Theophany(year))
                    {
                        engText = "were baptised";
                        copText = "ⲁⲕϭⲓⲱⲙⲥ";
                        araText = "TODO";
                    }
                    else if (today == CopticCalendar.FirstFeastCross(year)
                        || today == CopticCalendar.SecondFeastCross(year)
                        || CopticCalendar.PaschalPeriod(year).IsDuring(today))
                    {
                        engText = "were crucified";
                        copText = "ⲁⲩⲁϣⲕ";
                        araText = "TODO";
                    }
                    else
                    {
                        var resur = CopticCalendar.Resurrection(year);
                        var holyFiftyDays = Period.DaysBetween(resur, today);
                        bool isHolyFiftyDays = holyFiftyDays > 0 && holyFiftyDays <= 50;

                        var pentecost = CopticCalendar.Pentecost(year);
                        var koiahk = DateHelper.NewCopticDate(year, 4, 1);
                        bool isPentecostKiahk = today >= pentecost && today < koiahk;

                        if (isHolyFiftyDays || isPentecostKiahk || today.Day == 29)
                        {
                            engText = "have risen";
                            copText = "ⲁⲕⲧⲱⲛⲕ";
                            araText = "قمت";
                        }
                        else
                        {
                            engText = "have come";
                            copText = "ⲁⲕⲓ\u0300";
                            araText = "اتيت";
                        }
                    }

                    var res = new TranslationRunCollection("AkiAktonk");
                    res.AddText(engText, KnownLanguage.English);
                    res.AddText(copText, KnownLanguage.Coptic);
                    res.AddText(araText, KnownLanguage.Arabic);

                    return res;
                    """,
                    GetAkiAktonk
                },
                new object[]
                {
                    """
                    var today = context!.CurrentDate;
                    if (today == CopticCalendar.Resurrection(today.YearOfEra))
                        return new SimpleContent("aktonk", null);
                    else
                        return new SimpleContent("aki", null);
                    """,
                    (ILoadContext? context) =>
                    {
                        var today = context!.CurrentDate;
                        if (today == CopticCalendar.Resurrection(today.YearOfEra))
                            return new SimpleContent("aktonk", null);
                        return new SimpleContent("aki", null);
                    }
                },
            };
        }

        public static IEnumerable<object[]> GetRunLuaScript_Samples()
        {
            return new[]
            {
                new object[]
                {
                    "return SimpleContent('Test content', nil)",
                    (ILoadContext? _) => new SimpleContent("Test content", null)
                },
                new object[]
                {
                    """
                    local today = context.CurrentDate
                    if today == CopticCalendar.Resurrection(today.YearOfEra) then
                        return SimpleContent("aktonk", nil)
                    else
                        return SimpleContent("aki", nil)
                    end
                    """,
                    (ILoadContext? context) =>
                    {
                        var today = context!.CurrentDate;
                        if (today == CopticCalendar.Resurrection(today.YearOfEra))
                            return new SimpleContent("aktonk", null);
                        return new SimpleContent("aki", null);
                    }
                },
            };
        }

        private static IDefinition? GetAkiAktonk(ILoadContext? context)
        {
            // https://tasbeha.org/community/discussion/13753/aki-or-aktonk-etc
            var today = context!.CurrentDate;
            int year = today.YearOfEra;

            string engText, copText, araText;

            if (today == CopticCalendar.Nativity(year))
            {
                engText = "were born";
                copText = "ⲁⲩⲙⲁⲥⲕ";
                araText = "TODO";
            }
            else if (today == CopticCalendar.Theophany(year))
            {
                engText = "were baptised";
                copText = "ⲁⲕϭⲓⲱⲙⲥ";
                araText = "TODO";
            }
            else if (today == CopticCalendar.FirstFeastCross(year)
                || today == CopticCalendar.SecondFeastCross(year)
                || CopticCalendar.PaschalPeriod(year).IsDuring(today))
            {
                engText = "were crucified";
                copText = "ⲁⲩⲁϣⲕ";
                araText = "TODO";
            }
            else
            {
                var resur = CopticCalendar.Resurrection(year);
                var holyFiftyDays = Period.DaysBetween(resur, today);
                bool isHolyFiftyDays = holyFiftyDays > 0 && holyFiftyDays <= 50;

                var pentecost = CopticCalendar.Pentecost(year);
                var koiahk = DateHelper.NewCopticDate(year, 4, 1);
                bool isPentecostKiahk = today >= pentecost && today < koiahk;

                if (isHolyFiftyDays || isPentecostKiahk || today.Day == 29)
                {
                    engText = "have risen";
                    copText = "ⲁⲕⲧⲱⲛⲕ";
                    araText = "قمت";
                }
                else
                {
                    engText = "have come";
                    copText = "ⲁⲕⲓ\u0300";
                    araText = "اتيت";
                }
            }

            var res = new TranslationRunCollection("AkiAktonk");
            res.AddText(engText, KnownLanguage.English);
            res.AddText(copText, KnownLanguage.Coptic);
            res.AddText(araText, KnownLanguage.Arabic);

            return res;
        }
    }
}