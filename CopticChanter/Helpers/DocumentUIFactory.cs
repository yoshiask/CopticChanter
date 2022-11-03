using CoptLib.Models;
using CoptLib.Writing;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CopticChanter.Helpers
{
    public static class DocumentUIFactory
    {
        public static TextBlock CreateBlockFromStanza(Stanza stanza)
        {
            if (stanza.Language == Language.Default && stanza.Parent is IMultilingual multilingual)
                stanza.Language = multilingual.Language;

            var contentBlock = new TextBlock
            {
                Text = stanza.Text,
                FontFamily = Common.GetFontFamily(stanza.Language),
                FontSize = Common.GetFontSize(stanza.Language),
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Left
            };

            switch (stanza.Language)
            {
                case Language.Arabic:
                case Language.Aramaic:
                case Language.Hebrew:
                    contentBlock.TextAlignment = TextAlignment.Right;
                    break;

                case Language.Coptic:
                case Language.Greek:
                    // Font rendering is hard. UWP wants the combining character before,
                    // while certain HTML renderers can't make up their minds.
                    contentBlock.Text = CopticFont.SwapJenkimPosition(contentBlock.Text, CopticFont.CopticUnicode);

                    // TextBlock doesn't seem to know where to break Greek or Coptic Unicode
                    // lines, so insert a zero-width space at every space so
                    // word wrap actually works
                    if (!contentBlock.Text.Contains('\u200B'))
                        contentBlock.Text = contentBlock.Text.Replace(" ", " \u200B");

                    break;
            }

            return contentBlock;
        }

        public static List<TextBlock> CreateBlocksFromContentCollectionContainer(IContentCollectionContainer container)
        {
            var blocks = new List<TextBlock>(container.Content.Count + 1);

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
                            var block = CreateBlockFromStanza(stanza);
                            blocks.Add(block);
                            break;
                        }
                    case Section subsection:
                        {
                            var subblocks = CreateBlocksFromContentCollectionContainer(subsection);
                            blocks.AddRange(subblocks);
                            break;
                        }
                }
            }

            return blocks;
        }

        public static TextBlock CreateHeader(string title, bool addPadding = true)
        {
            var headerBlock = new TextBlock
            {
                Text = title,
                FontFamily = Common.GetFontFamily(Language.Default),
                FontSize = Common.GetFontSize(Language.Default),
                FontWeight = FontWeights.Bold,
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

        public static TextBlock CreateSubheader(string title, bool addPadding = true)
        {
            var headerBlock = new TextBlock
            {
                Text = title,
                FontFamily = Common.GetFontFamily(Language.Default),
                FontSize = Common.GetFontSize(Language.Default),
                FontWeight = FontWeights.SemiBold,
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
                            var stanzaBlock = CreateBlockFromStanza(stanza);
                            MainGrid.Children.Add(stanzaBlock);
                            Grid.SetColumn(stanzaBlock, t);
                            Grid.SetRow(stanzaBlock, lastRow + i++);
                            break;

                        case Section section:
                            var blocks = CreateBlocksFromContentCollectionContainer(section);
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
