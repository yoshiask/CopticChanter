using CoptLib.XML;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using static CoptLib.CopticInterpreter;

namespace CopticChanter.Helpers
{
	public static class DocumentUIFactory
	{
        public static List<TextBlock> CreateBlocksFromTranslation(Translation translation, Color foreground)
        {
            var blocks = new List<TextBlock>();
            foreach (ContentPart part in translation.Content)
			{
                var stanza = part as Stanza;
                if (stanza != null)
				{
                    blocks.Add(CreateBlockFromStanza(stanza, translation.Language, foreground));
				}
            }
            return blocks;
        }

        public static TextBlock CreateBlockFromStanza(Stanza stanza, Language translationLanguage, Color foreground)
		{
            TextBlock contentBlock = null;

            if (stanza.Language == Language.Default)
                stanza.Language = translationLanguage;

            switch (stanza.Language)
            {
                #region English & Arabic
                case Language.English:
                case Language.Arabic:
                    contentBlock = new TextBlock
                    {
                        Text = stanza.Text,
                        FontFamily = Common.Segoe,
                        FontSize = Common.GetEnglishFontSize(),
                        TextWrapping = TextWrapping.Wrap,
                        Foreground = new SolidColorBrush(foreground)
                    };
                    break;
                #endregion

                #region Coptic
                case Language.Coptic:
                    contentBlock = new TextBlock
                    {
                        // TextBlock doesn't seem to know where to break Coptic (Unicode?)
                        // lines, so insert a zero-width space at every space so
                        // word wrap acutally works
                        Text = stanza.Text.Replace(" ", " \u200B"),
                        FontFamily = Common.Segoe,
                        FontSize = Common.GetCopticFontSize(),
                        TextWrapping = TextWrapping.Wrap,
                        Foreground = new SolidColorBrush(foreground)
                    };
                    return contentBlock;
                #endregion
            }

            return contentBlock;
        }

        public static TextBlock CreateHeader(string title, Color foreground, bool addPadding = true)
		{
            var headerBlock = new TextBlock
            {
                Text = title,
                FontFamily = Common.Segoe,
                FontWeight = FontWeights.Bold,
                FontSize = Common.GetEnglishFontSize() * 1.25,
                TextWrapping = TextWrapping.Wrap,
                Foreground = new SolidColorBrush(foreground)
            };
            if (addPadding)
            {
                // Every header except for the first one should have a top margin
                // to distinguish it from the previous document
                headerBlock.Margin = new Thickness(0, 20, 0, 0);
            }
            return headerBlock;
        }
    }
}
