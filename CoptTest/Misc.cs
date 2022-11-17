using CoptLib.IO;
using Xunit;

namespace CoptTest
{
    public class Misc
    {
        [Theory]
        [InlineData(25)]
        public void TasbehaOrg_ConvertLyricsPage(int lyricId)
        {
            string html = Resource.ReadAllText($"TasbehaOrg_{lyricId}.html");
            var doc = TasbehaOrg.ConvertLyricsPage(html, lyricId);
            string xml = DocWriter.WriteDocXml(doc);
        }
    }
}
