using CoptLib.IO;
using CoptLib.Writing;
using System.Collections.Generic;
using System.Linq;
using CoptLib.Models;
using Xunit;
using Xunit.Abstractions;
using LEO = CoptLib.Writing.LanguageEquivalencyOptions;

namespace CoptTest
{
    public class Misc
    {
        private readonly ITestOutputHelper _output;

        public Misc(ITestOutputHelper output)
        {
            _output = output;
        }
        
        [Theory]
        [InlineData(25)]
        [InlineData(25, "utf8")]
        public void TasbehaOrg_ConvertLyricsPage(int lyricId, string? encoding = null)
        {
            string fileName = $"TasbehaOrg_{lyricId}";
            if (encoding is not null)
                fileName += $"_{encoding}";
            
            string html = Resource.ReadAllText(fileName + ".html");
            var doc = TasbehaOrg.ConvertLyricsPage(html, lyricId);
            
            Assert.NotNull(doc.Name);
            Assert.NotNull(doc.Key);
            Assert.NotEmpty(doc.Translations.Children);
            Assert.Equal(3, doc.Translations.Children.Count);
            Assert.All(doc.Translations.Children, c =>
            {
                Assert.True(c.IsExplicitlyDefined);
                Assert.NotNull(c.Language);
                Assert.NotEqual(LanguageInfo.Default, c.Language);

                var t = Assert.IsType<Section>(c);
                Assert.True(t.Children.Count > 0);
                if (t.Font is null)
                    Assert.NotNull(t.Title);
            });
            
            string xml = DocWriter.WriteDocXml(doc);
            _output.WriteLine(xml);
        }

        [Theory]
        [MemberData(nameof(GetLanguageInfo_Parse_Samples))]
        public void LanguageInfo_Parse(string tag)
        {
            var li = LanguageInfo.Parse(tag);
        }

        [Theory]
        [InlineData("en-US", KnownLanguage.English)]
        [InlineData("cop", KnownLanguage.Coptic)]
        [InlineData("cop-boh", KnownLanguage.CopticBohairic)]
        [InlineData("ar", KnownLanguage.Arabic)]
        [InlineData("ar-EG", KnownLanguage.Arabic)]
        [InlineData("cop/en", KnownLanguage.Coptic, KnownLanguage.English)]
        [InlineData("cop/en-US", KnownLanguage.Coptic, KnownLanguage.English)]
        [InlineData("cop-boh/en-US", KnownLanguage.CopticBohairic, KnownLanguage.English)]
        public void LanguageInfo_ParseKnownLanguage(string tag, KnownLanguage kLang, KnownLanguage? secKLang = null)
        {
            var li = LanguageInfo.Parse(tag);
            Assert.Equal(kLang, li.Known);
            Assert.Equal(secKLang, li.Secondary?.Known);
        }

        [Theory]
        [InlineData("en-US", "en-US", true)]
        [InlineData("en-US", "en", true, LEO.StrictWithWild)]
        [InlineData("cop-GR", "cop", true, LEO.StrictWithWild)]
        [InlineData("cop-boh", "cop", true, LEO.StrictWithWild)]
        [InlineData("cop-boh/en-US", "cop", true, LEO.StrictWithWild)]
        [InlineData("cop-boh", "cop", false, LEO.Strict)]
        [InlineData("cop-boh", "cop-EG", false, LEO.Strict)]
        [InlineData("cop-boh", "cop-boh", true, LEO.Strict)]
        [InlineData("cop-boh/en-US", "cop-boh/en-US", true, LEO.Strict)]
        [InlineData("cop-boh/en-US", "cop-boh", true, LEO.StrictWithWild)]
        [InlineData("cop-boh/en-US", "cop-sah", false, LEO.StrictWithWild)]//
        [InlineData("cop-boh", "cop-sah", false, LEO.LanguageRegion)]
        [InlineData("cop-boh", "cop-sah", true, LEO.Language)]
        [InlineData("cop-boh", "cop-sah", false, LEO.Region)]
        [InlineData("cop-boh/en-US", "cop-boh-EL", true, LEO.LanguageRegion | LEO.TreatNullAsWild)]
        [InlineData("cop-boh/en-US", "cop-boh", true, LEO.LanguageRegion | LEO.TreatNullAsWild)]
        [InlineData("cop-boh/en-US", "cop-sah", false, LEO.LanguageRegion)]
        [InlineData("cop-boh/en-US", "cop-sah", true, LEO.Language)]
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
                LanguageInfo.Parse("cop-GR/en-US"),
                LanguageInfo.Parse("cop-GR/ar-EG"),
                new LanguageInfo(KnownLanguage.Hebrew),
                new LanguageInfo(KnownLanguage.Arabic),
                new LanguageInfo("syr"),
            };

            HashSet<LanguageInfo> include = new()
            {
                new LanguageInfo(KnownLanguage.English),
                LanguageInfo.Parse("cop-GR/en-US"),
                LanguageInfo.Parse("el"),
                new LanguageInfo(KnownLanguage.Coptic),
                new LanguageInfo("is"),
                new LanguageInfo(KnownLanguage.Arabic),
            };

            var actual = include
                .Intersect(universe, new LanguageInfoEqualityComparer(LEO.StrictWithWild))
                .ToList();

            List<LanguageInfo> expected = new()
            {
                new LanguageInfo(KnownLanguage.English),
                LanguageInfo.Parse("cop-GR/en-US"),
                new LanguageInfo(KnownLanguage.Arabic),
            };

            Assert.Equal(expected.Count, actual.Count);
            
            foreach (var actualItem in actual)
            {
                if (expected.Contains(actualItem, new LanguageInfoEqualityComparer(LEO.Strict)))
                    expected.Remove(actualItem);
            }
            Assert.Empty(expected);
        }

        [Theory]
        [InlineData("Ⲡⲓⲡ\u0300ⲛⲉⲩⲙⲁ", "Noto Sans", "Segoe UI", "Ⲡⲓ\u0300ⲡⲛⲉⲩⲙⲁ")]
        public void SwapJenkims(string originalText, string originalFont, string targetFont, string expectedText)
        {

        }

        public static readonly string[] LanguageInfo_Parse_Samples = new string[]
        {
            KnownLanguage.English.ToString(), KnownLanguage.Coptic.ToString(),
            KnownLanguage.Arabic.ToString(), KnownLanguage.Default.ToString(),
            "cop", "cop-GR", "cop-sah", "de", "it-IT", "en-US",
            "cop-GR/en-US"
        };

        public static IEnumerable<object[]> GetLanguageInfo_Parse_Samples()
        {
            foreach (string sample in LanguageInfo_Parse_Samples)
                yield return new object[] { sample };
        }
    }
}
