using CoptLib;
using CoptLib.Models;
using CoptLib.Scripting.Commands;
using CoptLib.Writing;
using System.Linq;
using Xunit;

namespace CoptTest
{
    public class Scripting
    {
        Doc _doc = new Doc();

        [Fact]
        public void ParseTextCommands_EnglishSingleParameter()
        {
            var cmds = CoptLib.Scripting.Scripting.ParseTextCommands(@"This is some English, \ms{0:5:0}with a millisecond offset.", _doc, out var result);

            Assert.Equal("This is some English, with a millisecond offset.", result);
            Assert.True(cmds.Any());
        }

        [Fact]
        public void ParseTextCommands_LanguageCommand_English()
        {
            const Language lang = Language.English;
            const string subtext = "this is also some English";
            var cmds = CoptLib.Scripting.Scripting.ParseTextCommands($"This is some English, \\language{{{lang}}}{{{subtext}}}.", _doc, out var result);

            Assert.Equal($"This is some English, {subtext}.", result);

            var cmd = cmds.Single();
            var langCmd = Assert.IsType<LanguageCmd>(cmd);

            Assert.Equal(langCmd.Language, lang);
            Assert.Equal(langCmd.Text, subtext);
            Assert.Null(langCmd.Font);
        }

        [Fact]
        public void ParseTextCommands_LanguageCommand_CopticUnicode()
        {
            const Language lang = Language.Coptic;
            const string subtext = "Ⲉⲩⲗⲟⲅⲟⲛ ⲧⲟⲛ Ⲕⲩⲣⲓⲟⲛ";
            var cmds = CoptLib.Scripting.Scripting.ParseTextCommands($"This is some English, \\language{{{lang}}}{{{subtext}}}.", _doc, out var result);

            Assert.Equal($"Bless the Lord, {subtext}.", result);

            var cmd = cmds.Single();
            var langCmd = Assert.IsType<LanguageCmd>(cmd);

            Assert.Equal(langCmd.Language, lang);
            Assert.Equal(langCmd.Text, subtext);
            Assert.Null(langCmd.Font);
        }

        [Fact]
        public void ParseTextCommands_LanguageCommand_CopticStandard()
        {
            const Language lang = Language.Coptic;
            const string font = "CS Avva Shenouda";
            const string subtext = "Eulogon ton Kurion";
            const string convSubtext = "Ⲉⲩⲗⲟⲅⲟⲛ ⲧⲟⲛ Ⲕⲩⲣⲓⲟⲛ";
            var cmds = CoptLib.Scripting.Scripting.ParseTextCommands($"This is some English, \\language{{{lang}:{font}}}{{{subtext}}}.", _doc, out var result);

            Assert.Equal($"Bless the Lord, {convSubtext}.", result);

            var cmd = cmds.Single();
            var langCmd = Assert.IsType<LanguageCmd>(cmd);

            Assert.Equal(langCmd.Language, lang);
            Assert.Equal(langCmd.Text, convSubtext);
            Assert.Equal(langCmd.Font?.Name, font);
        }

        [Theory]
        [InlineData("engTest", "An English string")]
        [InlineData("coptCSTest", "Nenio] `n`apoctoloc", "Ⲛⲉⲛⲓⲟϯ ⲛ̀ⲁ̀ⲡⲟⲥⲧⲟⲗⲟⲥ", "CS Copt", Language.Coptic)]
        [InlineData("coptUniTest", "Ⲛⲉⲛⲓⲟϯ ⲛ̀ⲁ̀ⲡⲟⲥⲧⲟⲗⲟⲥ", null, null, Language.Coptic)]
        [InlineData("coptUniTest", "Ⲛⲉⲛⲓⲟϯ ⲛ̀ⲁ̀ⲡⲟⲥⲧⲟⲗⲟⲥ", null, "Coptic Unicode", Language.Coptic)]
        public void ParseTextCommands_DefinitionCommand_String(string key, string value, string? parsedValue = null, string? font = null, Language? language = null)
        {
            const string preText = "Howdy! Here's some text from a definition: '";
            const string postText = "'.\r\nAlong with some text after.";
            Language lang = language ?? Language.Default;
            _doc.Definitions = new()
            {
                new String()
                {
                    DocContext = _doc,
                    Key = key,
                    Value = value,
                    Font = font,
                    Language = lang
                }
            };

            var cmds = CoptLib.Scripting.Scripting.ParseTextCommands(preText + $"\\def{{{key}}}" + postText, _doc, out var result);

            Assert.Equal($"{preText}{value}{postText}", result);

            var cmd = cmds.Single();
            var defCmd = Assert.IsType<DefinitionCmd>(cmd);
            var def = Assert.IsType<String>(defCmd.Definition);

            Assert.Equal(defCmd.Text, parsedValue ?? value);
            Assert.Equal(def.Language, lang);
            Assert.Equal(def.Font, font);
        }
    }
}