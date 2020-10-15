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
            switch (translation.Language)
            {
                #region English
                case Language.English:
                    foreach (Stanza content in translation.Stanzas)
                    {
                        var contentBlockE = new TextBlock
                        {
                            Text = content.Text,
                            FontFamily = Common.Segoe,
                            FontSize = Common.GetEnglishFontSize(),
                            TextWrapping = TextWrapping.Wrap,
                            Foreground = new SolidColorBrush(foreground)
                        };
                        blocks.Add(contentBlockE);
                    }
                    break;
                #endregion

                #region Coptic
                case Language.Coptic:
                    foreach (Stanza content in translation.Stanzas)
                    {
                        var contentBlockC = new TextBlock
                        {
                            Text = content.Text.Replace(" ", " \u200B"),
                            FontFamily = Common.Segoe,
                            FontSize = Common.GetCopticFontSize(),
                            TextWrapping = TextWrapping.Wrap,
                            Foreground = new SolidColorBrush(foreground)
                        };
                        blocks.Add(contentBlockC);
                    }
                    break;
                #endregion

                #region Arabic
                // TODO: Support Arabic text
                case Language.Arabic:
                    foreach (Stanza content in translation.Stanzas)
                    {
                        var contentBlockA = new TextBlock
                        {
                            Text = content.Text,
                            FontFamily = Common.Segoe,
                            FontSize = Common.GetEnglishFontSize(),
                            TextWrapping = TextWrapping.Wrap,
                            Foreground = new SolidColorBrush(foreground)
                        };
                        blocks.Add(contentBlockA);
                    }
                    break;
                    #endregion
            }
            return blocks;
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
