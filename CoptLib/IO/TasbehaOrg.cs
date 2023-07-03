using System;
using System.Linq;
using System.Text.RegularExpressions;
using AngleSharp.Html.Parser;
using CoptLib.Models;
using CoptLib.Writing;

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
        /// and will not work for such purposes.<br/>The intended use is for users
        /// with more technical experience to copy the full HTML of a lyrics page
        /// after first loading it manually in a full web browser.
        /// </remarks>
        public static Doc ConvertLyricsPage(string html, int lyricId)
        {
            Doc doc = new()
            {
                Key = $"urn:tasbehaorg:{lyricId}",
            };

            // Parse the HTML from Tasbeha.org
            var feed = new HtmlParser().ParseDocument(html);

            // Set document name
            doc.Name = feed.QuerySelector("h1").TextContent;
            
            // Read stanzas
            int parsedRowCount = 0;
            foreach (var row in feed.QuerySelectorAll("div#hymntext div.row"))
            {
                var rxLanguageClass = new Regex(@"(?<lang>\w+)text(?:_(?<enc>\w+))?", RegexOptions.Compiled);
                foreach (var cell in row.Children)
                {
                    var languageClassInfo = rxLanguageClass.Match(cell.ClassName!);
                    if (!languageClassInfo.Success)
                        continue;
                    
                    var knownLang = (KnownLanguage)Enum.Parse(typeof(KnownLanguage), languageClassInfo.Groups["lang"].Value, true);
                    if (doc.Translations.Children.FirstOrDefault(t => t.Language?.Known == knownLang) is not Section translation)
                    {
                        translation = new(null)
                        {
                            Language = new LanguageInfo(knownLang),
                            IsExplicitlyDefined = true
                        };

                        if (translation.Language.Language == "cop" && !languageClassInfo.Groups["enc"].Success)
                            translation.Font = CopticFont.CsAvvaShenouda.Name;

                        if (parsedRowCount != 0)
                        {
                            // Add empty stanzas so this translation aligns with the others
                            Stanza emptyStanza = new(translation);
                            translation.Children.AddRange(Enumerable.Range(0, parsedRowCount).Select(_ => emptyStanza));
                        }
                        
                        doc.Translations.Children.Add(translation);
                    }

                    translation.Children.Add(new Stanza(translation)
                    {
                        SourceText = cell.TextContent.Trim('\n', '\t', ' ', '+'),
                        IsExplicitlyDefined = true
                    });
                }
                
                ++parsedRowCount;
            }

            return doc;
        }
    }
}
