using AngleSharp.Dom;
using CoptLib.Models;
using CoptLib.Writing;
using System.Linq;

namespace CoptLib.IO
{
    /// <summary>
    /// Static helper for creating a <see cref="Doc"/> from the Tasbeha.org
    /// lyrics library.
    /// </summary>
    public static class TasbehaOrg
    {
        /// <summary>
        /// Creates a <see cref="Doc"/> from a fully loaded Tasbeha.org lyric page.
        /// </summary>
        /// <param name="html">
        /// The HTML text of the lyric page.
        /// </param>
        /// <param name="lyricId">
        /// The lyric ID of the hymn. For example, <c>https://tasbeha.org/hymn_library/view/103</c>
        /// has and ID of 103.</param>
        /// <returns>
        /// A <see cref="Doc"/> with the content and title of the Tasbeha.org lyrics.
        /// </returns>
        /// <remarks>
        /// This feature is not for scraping the Tasbeha.org website,
        /// and will not work for such purposes.<br/> The intended use is for users
        /// with more technical experience to copy the full HTML of a lyrics page
        /// after first loading it manually in a full web browser.
        /// </remarks>
        public static Doc CreateDocAsync(string html, int lyricId)
        {
            // Parse the HTML from Tasbeha.org
            var feed = new AngleSharp.Html.Parser.HtmlParser().ParseDocument(html);

            Doc doc = new()
            {
                Uuid = $"urn:tasbehaorg:{lyricId}",
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

            foreach (var stanzaText in htmlStanzas.Select(m => m.FirstChild?.TextContent))
            {
                translation.Children.Add(new Stanza(translation)
                {
                    SourceText = stanzaText,
                    IsExplicitlyDefined = true
                });
            }
        }
    }
}
