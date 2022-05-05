using Xunit;

namespace CoptTest
{
    public class Scripting
    {
        [Fact]
        public void ParseTextCommands_EnglishSingleParameter()
        {
            string result = CoptLib.Scripting.ParseTextCommands(@"This is some English, \ms{0:5:0}with a millisecond offset.");

            Assert.Equal("This is some English, with a millisecond offset.", result);
        }
    }
}