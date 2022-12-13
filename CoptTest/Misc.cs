using CoptLib.IO;
using CoptLib.Writing;
using System.Collections.Generic;
using Xunit;

using LEO = CoptLib.Writing.LanguageEquivalencyOptions;

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

        [Theory]
        [InlineData("en-US", "en-US", true)]
        [InlineData("en-US", "en", true, LEO.Strict | LEO.TreatNullAsWild)]
        [InlineData("cop-GR", "cop", true, LEO.Strict | LEO.TreatNullAsWild)]
        [InlineData("cop-EG-ALX", "cop", true, LEO.Strict | LEO.TreatNullAsWild)]
        [InlineData("cop-EG-ALX/en-US", "cop", true, LEO.Strict | LEO.TreatNullAsWild)]
        [InlineData("cop-EG-ALX", "cop", false, LEO.Strict)]
        [InlineData("cop-EG-ALX", "cop-EG", false, LEO.Strict)]
        [InlineData("cop-EG-ALX", "cop-EG-ALX", true, LEO.Strict)]
        [InlineData("cop-EG-ALX/en-US", "cop-EG-ALX/en-US", true, LEO.Strict)]
        [InlineData("cop-EG-ALX/en-US", "cop-EG-ALX", true, LEO.Strict | LEO.TreatNullAsWild)]
        [InlineData("cop-EG-ALX/en-US", "cop-EG-AST", false, LEO.Strict | LEO.TreatNullAsWild)]
        [InlineData("cop-EG-ALX", "cop-EG-AST", true, LEO.LanguageRegion)]
        [InlineData("cop-EG-ALX/en-US", "cop-EG-AST", true, LEO.LanguageRegion | LEO.TreatNullAsWild)]
        [InlineData("cop-EG-ALX/en-US", "cop-EG-AST", true, LEO.LanguageRegion)]
        [InlineData("pt-BR", "en-BR", true, LEO.Region)]
        public void LanguageInfo_IsEquivalentTo(string tagA, string tagB, bool expected, LEO options = LEO.Strict)
        {
            var liA = LanguageInfo.Parse(tagA);
            var liB = LanguageInfo.Parse(tagB);

            bool actualAB = liA.IsEquivalentTo(liB, options);
            bool actualBA = liB.IsEquivalentTo(liA, options);

            Assert.Equal(expected, actualAB);
            Assert.Equal(expected, actualBA);
        }

        public static readonly string[] LanguageInfo_Parse_Samples = new string[]
        {
            KnownLanguage.English.ToString(), KnownLanguage.Coptic.ToString(),
            KnownLanguage.Arabic.ToString(), KnownLanguage.Default.ToString(),
            "cop", "cop-GR", "cop-EG-AST", "de", "it-IT", "en-US",
            "cop-GR/en-US"
        };

        public static IEnumerable<object[]> GetLanguageInfo_Parse_Samples()
        {
            foreach (string sample in LanguageInfo_Parse_Samples)
                yield return new object[] { sample };
        }
    }
}
