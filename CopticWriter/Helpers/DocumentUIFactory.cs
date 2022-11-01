using CoptLib.Models;
using CoptLib.Writing;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CopticWriter.Helpers
{
    public static class DocumentUIFactory
    {
        public static TextBox CreateBlockFromStanza(Stanza stanza, Language translationLanguage)
        {
            if (stanza.Language == Language.Default)
                stanza.Language = translationLanguage;

            TextBox contentBlock = new TextBox
            {
                Text = stanza.Text,
                FontFamily = Common.DefaultFont,
                FontSize = Common.DefaultFontSize,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Right
            };

            if (stanza.Language == Language.Arabic)
            {
                contentBlock.TextAlignment = TextAlignment.Right;
            }

            return contentBlock;
        }

        public static List<TextBox> CreateBlocksFromContentCollectionContainer(IContentCollectionContainer container, Language translationLanguage)
        {
            var blocks = new List<TextBox>(container.Content.Count + 1);

            if (container is Section section)
            {
                var headerBlock = CreateSubheader(section.Title);
                blocks.Add(headerBlock);
            }

            foreach (ContentPart part in container.Content)
            {
                switch (part)
                {
                    case Stanza stanza:
                    {
                        var block = CreateBlockFromStanza(stanza, translationLanguage);
                        blocks.Add(block);
                        break;
                    }
                    case Section subsection:
                    {
                        var subblocks = CreateBlocksFromContentCollectionContainer(subsection, translationLanguage);
                        blocks.AddRange(subblocks);
                        break;
                    }
                }
            }

            return blocks;
        }

        public static TextBox CreateHeader(string title, bool addPadding = true)
        {
            var headerBlock = new TextBox
            {
                Text = title,
                FontFamily = Common.DefaultFont,
                FontWeight = FontWeights.Bold,
                FontSize = Common.DefaultFontSize * 1.25,
                TextWrapping = TextWrapping.Wrap
            };
            if (addPadding)
            {
                // Every header except for the first one should have a top margin
                // to distinguish it from the previous document
                headerBlock.Margin = new Thickness(0, 20, 0, 0);
            }
            return headerBlock;
        }

        public static TextBox CreateSubheader(string title, bool addPadding = true)
        {
            var headerBlock = new TextBox
            {
                Text = title,
                FontFamily = Common.DefaultFont,
                FontWeight = FontWeights.SemiBold,
                FontSize = Common.DefaultFontSize,
                TextWrapping = TextWrapping.Wrap
            };
            if (addPadding)
            {
                // Every header except for the first one should have a top margin
                // to distinguish it from the previous document
                headerBlock.Margin = new Thickness(0, 10, 0, 0);
            }
            return headerBlock;
        }

        public static Grid CreateGridFromDoc(Doc doc)
        {
            Grid MainGrid = new Grid();
            MainGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
            int lastRow = 0;

            // Create a column for each language requested
            for (int i = 0; i < doc.Translations.Count; i++)
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition());

            // Create rows for each stanza
            // Don't forget one for each header too
            int numRows = doc.Translations?.Max(t => t.CountRows()) ?? 0;
            for (int i = 0; i <= numRows; i++)
                MainGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            var headerBlock = CreateHeader(doc.Name, lastRow != 0);
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
                            var stanzaBlock = CreateBlockFromStanza(stanza, translation.Language);
                            MainGrid.Children.Add(stanzaBlock);
                            Grid.SetColumn(stanzaBlock, t);
                            Grid.SetRow(stanzaBlock, lastRow + i++);
                            break;

                        case Section section:
                            var blocks = CreateBlocksFromContentCollectionContainer(section, translation.Language);
                            foreach (TextBox block in blocks)
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
