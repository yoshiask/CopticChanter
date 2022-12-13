using CoptLib.IO;
using CoptLib.Writing;
using System.Collections.Generic;
using System.Linq;
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
        [InlineData("en-US", "en", true, LEO.StrictWithWild)]
        [InlineData("cop-GR", "cop", true, LEO.StrictWithWild)]
        [InlineData("cop-EG-ALX", "cop", true, LEO.StrictWithWild)]
        [InlineData("cop-EG-ALX/en-US", "cop", true, LEO.StrictWithWild)]
        [InlineData("cop-EG-ALX", "cop", false, LEO.Strict)]
        [InlineData("cop-EG-ALX", "cop-EG", false, LEO.Strict)]
        [InlineData("cop-EG-ALX", "cop-EG-ALX", true, LEO.Strict)]
        [InlineData("cop-EG-ALX/en-US", "cop-EG-ALX/en-US", true, LEO.Strict)]
        [InlineData("cop-EG-ALX/en-US", "cop-EG-ALX", true, LEO.StrictWithWild)]
        [InlineData("cop-EG-ALX/en-US", "cop-EG-AST", false, LEO.StrictWithWild)]//
        [InlineData("cop-EG-ALX", "cop-EG-AST", true, LEO.LanguageRegion)]
        [InlineData("cop-EG-ALX/en-US", "cop-EG-AST", true, LEO.LanguageRegion | LEO.TreatNullAsWild)]
        [InlineData("cop-EG-ALX/en-US", "cop-EG-AST", true, LEO.LanguageRegion)]
        [InlineData("pt-BR", "en-BR", true, LEO.Region)]
        [InlineData("cop-GR/en-US", "ar-EG/en-US", true, LEO.Secondary)]
        [InlineData("cop-GR/en-US", "ar-EG/en", true, LEO.Secondary | LEO.TreatNullAsWild)]
        [InlineData("cop-GR/en-US", "ar-EG/en", false, LEO.Secondary)]
        [InlineData("cop-GR/ar-EG", "cop/ar", true, LEO.StrictWithWild)]
        public void LanguageInfo_IsEquivalentTo(string tagA, string tagB, bool expected, LEO options = LEO.Strict)
        {
            var liA = LanguageInfo.Parse(tagA);
            var liB = LanguageInfo.Parse(tagB);

            bool actualAB = liA.IsEquivalentTo(liB, options);
            bool actualBA = liB.IsEquivalentTo(liA, options);

            Assert.Equal(expected, actualAB);
            Assert.Equal(expected, actualBA);
        }

        [Fact]
        public void LanguageInfoEqualityComparer()
        {
            HashSet<LanguageInfo> universe = new()
            {
                new LanguageInfo(KnownLanguage.English),
                new LanguageInfo(KnownLanguage.Coptic),
                new LanguageInfo("cop-GR/en-US"),
                new LanguageInfo("cop-GR/ar-EG"),
                new LanguageInfo(KnownLanguage.Hebrew),
                new LanguageInfo(KnownLanguage.Arabic),
                new LanguageInfo("syr"),
            };

            HashSet<LanguageInfo> include = new()
            {
                new LanguageInfo(KnownLanguage.English),
                new LanguageInfo("cop-GR/en-US"),
                new LanguageInfo("el"),
                new LanguageInfo(KnownLanguage.Coptic),
                new LanguageInfo("is"),
                new LanguageInfo(KnownLanguage.Arabic),
            };

            var result = universe.Intersect(include, new LanguageInfoEqualityComparer(LEO.StrictWithWild));

            Assert.Equal(4, result.Count());
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
