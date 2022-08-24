using CoptLib;
using CoptLib.Models;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CopticChanter.Helpers
{
    public static class DocumentUIFactory
    {
        [System.Obsolete]
        public static List<TextBlock> CreateBlocksFromTranslation(Translation translation, Color foreground)
        {
            var blocks = new List<TextBlock>();
            foreach (ContentPart part in translation.Content)
            {
                if (part is Stanza stanza)
                {
                    blocks.Add(CreateBlockFromStanza(stanza, translation.Language, foreground));
                }
            }
            return blocks;
        }

        public static TextBlock CreateBlockFromStanza(Stanza stanza, Language translationLanguage, Color foreground)
        {
            TextBlock contentBlock;

            if (stanza.Language == Language.Default)
                stanza.Language = translationLanguage;

            switch (stanza.Language)
            {
                #region Arabic
                case Language.Arabic:
                    contentBlock = new TextBlock
                    {
                        Text = stanza.Text,
                        FontFamily = Common.Segoe,
                        FontSize = Common.GetEnglishFontSize(),
                        TextWrapping = TextWrapping.Wrap,
                        TextAlignment = TextAlignment.Right,
                        Foreground = new SolidColorBrush(foreground)
                    };
                    break;
                #endregion

                default:
                    contentBlock = new TextBlock
                    {
                        Text = stanza.Text,
                        FontFamily = Common.Segoe,
                        FontSize = Common.GetEnglishFontSize(),
                        TextWrapping = TextWrapping.Wrap,
                        Foreground = new SolidColorBrush(foreground)
                    };
                    break;
            }

            return contentBlock;
        }

        public static List<TextBlock> CreateBlocksFromContentCollectionContainer(IContentCollectionContainer container, Language translationLanguage, Color foreground)
		{
            var blocks = new List<TextBlock>(container.Content.Count + 1);

            if (container is Section section)
			{
                var headerBlock = CreateSubheader(section.Title, foreground);
                blocks.Add(headerBlock);
            }

            foreach (ContentPart part in container.Content)
            {
                switch (part)
                {
                    case Stanza stanza:
                    {
                        var block = CreateBlockFromStanza(stanza, translationLanguage, foreground);
                        blocks.Add(block);
                        break;
                    }
                    case Section subsection:
                    {
                        var subblocks = CreateBlocksFromContentCollectionContainer(subsection, translationLanguage, foreground);
                        blocks.AddRange(subblocks);
                        break;
                    }
                }
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

        public static TextBlock CreateSubheader(string title, Color foreground, bool addPadding = true)
        {
            var headerBlock = new TextBlock
            {
                Text = title,
                FontFamily = Common.Segoe,
                FontWeight = FontWeights.SemiBold,
                FontSize = Common.GetEnglishFontSize(),
                TextWrapping = TextWrapping.Wrap,
                Foreground = new SolidColorBrush(foreground)
            };
            if (addPadding)
            {
                // Every header except for the first one should have a top margin
                // to distinguish it from the previous document
                headerBlock.Margin = new Thickness(0, 10, 0, 0);
            }
            return headerBlock;
        }

        public static Grid CreateGridFromDoc(Doc doc, Color foreground)
		{
            Grid MainGrid = new Grid();
            int lastRow = 0;

            // Create a column for each language requested
            for (int i = 0; i < doc.Translations.Count; i++)
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition());

            // Create rows for each stanza
            // Don't forget one for each header too
            int numRows = doc.Translations.Max(t => t.CountRows());
            for (int i = 0; i <= numRows; i++)
                MainGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            var headerBlock = CreateHeader(doc.Name, foreground, lastRow != 0);
            MainGrid.Children.Add(headerBlock);
            Grid.SetColumnSpan(headerBlock, doc.Translations.Count);
            Grid.SetRow(headerBlock, lastRow++);

            for (int t = 0; t < doc.Translations.Count; t++)
			{
                Translation translation = doc.Translations[t];

                int i = 0;
                foreach (ContentPart part in translation.Content)
                {
                    switch (part)
					{
                        case Stanza stanza:
                            var stanzaBlock = CreateBlockFromStanza(stanza, translation.Language, foreground);
                            MainGrid.Children.Add(stanzaBlock);
                            Grid.SetColumn(stanzaBlock, t);
                            Grid.SetRow(stanzaBlock, lastRow + i++);
                            break;

                        case Section section:
                            var blocks = CreateBlocksFromContentCollectionContainer(section, translation.Language, foreground);
                            foreach (TextBlock block in blocks)
                            {
                                MainGrid.Children.Add(block);
                                Grid.SetColumn(block, t);
                                Grid.SetRow(block, lastRow + i++);
                            }
                            break;
                    }
                }
            }
            return MainGrid;
        }
    }
}
