using CoptLib.IO;
using CoptLib.Writing;
using System.Collections.Generic;
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

        [Theory]
        [MemberData(nameof(GetLanguageInfo_Parse_Samples))]
        public void LanguageInfo_Parse(string tag)
        {
            var li = LanguageInfo.Parse(tag);
        }

        public static readonly string[] LanguageInfo_Parse_Samples = new string[]
        {
            KnownLanguage.English.ToString(), KnownLanguage.Coptic.ToString(),
            KnownLanguage.Arabic.ToString(), KnownLanguage.Default.ToString(),
            "cop", "cop-GR", "cop-EG-AST", "de", "it-IT", "en-US"
        };

        public static IEnumerable<object[]> GetLanguageInfo_Parse_Samples()
        {
            foreach (string sample in LanguageInfo_Parse_Samples)
                yield return new object[] { sample };
        }
    }
}
