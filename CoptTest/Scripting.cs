using CoptLib.IO;
using CoptLib.Models;
using CoptLib.Scripting.Commands;
using CoptLib.Writing;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace CoptTest
{
    public class Scripting
    {
        Doc _doc = new Doc();

        private readonly ITestOutputHelper _output;

        public Scripting(ITestOutputHelper output)
        {
            _output = output;
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
            var cmds = CoptLib.Scripting.Scripting.ParseTextCommands(stanza, out var result);

            Assert.Equal(string.Format(text, string.Empty), result);
            Assert.True(cmds.Any());
        }

        [Theory]
        [InlineData("this is also some English", null, null, Language.English)]
        [InlineData("Ⲉⲩⲗⲟⲅⲟⲛ ⲧⲟⲛ Ⲕⲩⲣⲓⲟⲛ", null, null, Language.Coptic)]
        [InlineData("Eulogon ton Kurion", "Ⲉⲩⲗⲟⲅⲟⲛ ⲧⲟⲛ Ⲕⲩⲣⲓⲟⲛ", "CS Avva Shenouda", Language.Coptic)]
        public void ParseTextCommands_LanguageCommand(string subtext, string? convSubtext = null, string? font = null, Language lang = default)
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
            var cmds = CoptLib.Scripting.Scripting.ParseTextCommands(stanza, out var result);

            Assert.Equal($"Bless the Lord, {convSubtext}.", result);

            var cmd = cmds.Single();
            var langCmd = Assert.IsType<LanguageCmd>(cmd);

            Assert.Equal(langCmd.Language, lang);
            Assert.Equal(langCmd.Text, convSubtext);
            if (font == null)
                Assert.Null(langCmd.Font);
            Assert.Equal(langCmd.Font?.Name, font);
        }

        [Theory]
        [InlineData("engTest", "An English string")]
        [InlineData("coptCSTest", "Nenio] `n`apoctoloc", "Ⲛⲉⲛⲓⲟϯ ⲛ̀ⲁ̀ⲡⲟⲥⲧⲟⲗⲟⲥ", "CS Copt", Language.Coptic)]
        [InlineData("coptUniTest", "Ⲛⲉⲛⲓⲟϯ ⲛ̀ⲁ̀ⲡⲟⲥⲧⲟⲗⲟⲥ", null, null, Language.Coptic)]
        [InlineData("coptUniTest", "Ⲛⲉⲛⲓⲟϯ ⲛ̀ⲁ̀ⲡⲟⲥⲧⲟⲗⲟⲥ", null, "Coptic Unicode", Language.Coptic)]
        public void ParseTextCommands_DefinitionCommand_String(string key, string value, string? parsedValue = null, string? font = null, Language lang = default)
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
                    Language = lang
                }
            };
            DocReader.ApplyDocTransforms(_doc);

            Stanza stanza = new(null)
            {
                SourceText = preText + $"\\def{{{key}}}" + postText,
                DocContext = _doc
            };
            var cmds = CoptLib.Scripting.Scripting.ParseTextCommands(stanza, out var result);

            Assert.Equal($"{preText}{parsedValue}{postText}", result);

            var cmd = cmds.Single();
            var defCmd = Assert.IsType<DefinitionCmd>(cmd);
            var def = Assert.IsType<Stanza>(defCmd.Output);

            Assert.Equal(defCmd.Text, parsedValue);
            Assert.Equal(def.Language, lang);
            Assert.Equal(def.Font, font);
        }

        [Theory]
        [InlineData("A Coptic word, \\lang{Coptic|CS Avva Shenouda|Wcanna}, and its IPA transcription, \\ipa{\\lang{Coptic|CS Avva Shenouda|Wcanna}}.",
                    "A Coptic word, Ⲱⲥⲁⲛⲛⲁ, and its IPA transcription, o\u031Esännä.")]
        public void ParseTextCommands_NestedCommands(string text, string? expectedResult = null)
        {
            expectedResult ??= text;

            Doc doc = new();
            Stanza stanza = new(null)
            {
                SourceText = text,
                DocContext = doc
            };
            var cmds = CoptLib.Scripting.Scripting.ParseTextCommands(stanza, out var result);

            Assert.Equal(expectedResult, result);
        }
    }
}