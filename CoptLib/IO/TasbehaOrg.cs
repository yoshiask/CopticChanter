using AngleSharp;
using AngleSharp.Dom;
using CoptLib.Models;
using CoptLib.Writing;
using System.Linq;
using System.Threading.Tasks;

namespace CoptLib.IO
{
    /// <summary>
    /// Static helper for creating a <see cref="Doc"/> from the Tasbeha.org
    /// lyrics library.
    /// </summary>
    public static class TasbehaOrg
    {
        private const string TASBEHAORG_HYMNLIBRARY_VIEW = "https://tasbeha.org/hymn_library/view/";

        public static async Task<Doc> CreateDocAsync(int lyricId)
        {
            // Create a new configuration with the default loader
            var config = Configuration.Default.WithDefaultLoader();
            var url = TASBEHAORG_HYMNLIBRARY_VIEW + lyricId.ToString();

            // Create a new context
            var context = BrowsingContext.New(config);
            var address = Url.Create(url);
            var feed = await context.OpenAsync(address);

            Doc doc = new()
            {
                Uuid = url,
            };

            // Set document name
            doc.Name = feed.QuerySelector("h1").TextContent;

            // Set English stanzas
            Section englishTranslation = new(doc.Translations)
            {
                Language = Language.English,
                IsExplicitlyDefined = true
            };
            AddStanzasToTranslation(feed, englishTranslation);
            if (englishTranslation.Children.Count > 0)
                doc.Translations.Children.Add(englishTranslation);

            // Set Coptic stanzas
            Section copticTranslation = new(doc.Translations)
            {
                Language = Language.Coptic,
                Font = CopticFont.CsAvvaShenouda.Name,
                IsExplicitlyDefined = true
            };
            AddStanzasToTranslation(feed, copticTranslation);
            if (copticTranslation.Children.Count > 0)
                doc.Translations.Children.Add(copticTranslation);

            // Set Arabic stanzas
            Section arabicTranslation = new(doc.Translations)
            {
                Language = Language.Arabic,
                IsExplicitlyDefined = true
            };
            AddStanzasToTranslation(feed, arabicTranslation);
            if (arabicTranslation.Children.Count > 0)
                doc.Translations.Children.Add(arabicTranslation);

            return doc;
        }

        private static void AddStanzasToTranslation(IDocument feed, Section translation)
        {
            var htmlStanzas = feed.QuerySelectorAll($"div.{translation.Language.ToString().ToLower()}text");

            if (translation.Language == Language.Coptic)
            {
                // Tasbeha.org obfuscates segments of Coptic text
                EmailDecoder(feed);
            }
            else
            {

            }

            foreach (var stanzaText in htmlStanzas.Select(m => m.FirstChild?.TextContent))
            {
                translation.Children.Add(new Stanza(translation)
                {
                    SourceText = stanzaText,
                    IsExplicitlyDefined = true
                });
            }
        }

        private static void EmailDecoder(IDocument document)
        {
            var anchorElems = document.QuerySelectorAll(".__cf_email__");
            foreach (var o in anchorElems)
            {
                var a = o.Parent;
                var i = o.GetAttribute("data-cfemail");
                if (i != null)
                {
                    var decryptedText = decrypt(i, 0);
                    var d = document.CreateTextNode(decryptedText);
                    a.ReplaceChild(d, o);
                }
            }
        }

        private static int parseHex(string e, int t)
        {
            string r = e.Substring(t, 2);
            return int.Parse(r, System.Globalization.NumberStyles.HexNumber);
        }

        private static string decrypt(string n, int c)
        {
            var o = "";
            var a = parseHex(n, c);
            for (var i = c + 2; i < n.Length; i += 2)
            {
                var l = parseHex(n, i) ^ a;
                o += char.ConvertFromUtf32(l);
            }
            return o;
        }
    }
}
