using CoptLib.IO;
using Xunit;

namespace CoptTest
{
    public class Misc
    {
        [Theory]
        [InlineData(210)]
        public async void TasbehaOrg_CreateDocAsync(int lyricId)
        {
            var doc = await TasbehaOrg.CreateDocAsync(lyricId);
            string xml = DocWriter.WriteDocXml(doc);
        }
    }
}
