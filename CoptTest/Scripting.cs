using CoptLib;
using CoptLib.Models;
using CoptLib.Scripting.Commands;
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
        public void ParseTextCommands_LanguageCommand()
        {
            Language lang = Language.English;
            string subtext = "this is also some English";
            var cmds = CoptLib.Scripting.Scripting.ParseTextCommands($"This is some English, \\language{{{lang}}}{{{subtext}}}.", _doc, out var result);

            Assert.Equal($"This is some English, {subtext}.", result);

            var cmd = cmds.Single();
            var langCmd = Assert.IsType<LanguageCmd>(cmd);

            Assert.Equal(langCmd.Language, lang);
            Assert.Equal(langCmd.Text, subtext);
            Assert.Null(langCmd.Font);
        }
    }
}