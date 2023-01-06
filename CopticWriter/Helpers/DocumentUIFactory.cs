using CoptLib.Extensions;
using CoptLib.Models;
using CoptLib.Writing;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CopticWriter.Helpers
{
    public static class DocumentUIFactory
    {
        public static void CreateFromContentPart(ContentPart part, Action<TextBox> action)
        {
            if (action == null)
                action = _ => { };

            switch (part)
            {
                case IContent content:
                    var contentBlock = CreateBlockFromContent(content);
                    action(contentBlock);
                    break;

                case IContentCollectionContainer contentCollection:
                    var blocks = CreateBlocksFromContentCollectionContainer(contentCollection);
                    foreach (TextBox block in blocks)
                        action(block);
                    break;
            }
        }

        public static TextBox CreateBlockFromContent(IContent contentPart)
        {
            var contentBlock = new TextBox
            {
                Text = contentPart.Text,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Left
            };

            HandleLanguage(contentBlock, contentPart);

            return contentBlock;
        }

        public static List<TextBox> CreateBlocksFromContentCollectionContainer(IContentCollectionContainer container)
        {
            var blocks = new List<TextBox>(container.Children.Count + 1);

            if (container is Section section && section.Title != null)
            {
                var headerBlock = CreateSubheader(section.Title);
                blocks.Add(headerBlock);
            }

            foreach (ContentPart part in container.Children)
            {
                switch (part)
                {
                    case Stanza stanza:
                        {
                            var block = CreateBlockFromContent(stanza);
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

        public static TextBox CreateHeader(string title, bool addPadding = true)
        {
            var headerBlock = new TextBox
            {
                Text = title,
                FontWeight = FontWeights.Bold,
                TextWrapping = TextWrapping.Wrap
            };
            if (addPadding)
            {
                // Every header except for the first one should have a top margin
                // to distinguish it from the previous document
                headerBlock.Margin = new Thickness(0, 20, 0, 0);
            }

            HandleLanguage(headerBlock, title);

            return headerBlock;
        }

        public static TextBox CreateSubheader(IContent title, bool addPadding = true)
        {
            var headerBlock = new TextBox
            {
                Text = title.Text,
                FontWeight = FontWeights.SemiBold,
                TextWrapping = TextWrapping.Wrap
            };
            if (addPadding)
            {
                // Every header except for the first one should have a top margin
                // to distinguish it from the previous document
                headerBlock.Margin = new Thickness(0, 10, 0, 0);
            }

            HandleLanguage(headerBlock, title);

            return headerBlock;
        }

        public static Grid CreateGridFromDoc(Doc doc)
        {
            Grid MainGrid = new Grid();
            MainGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
            int lastRow = 0;
            int translationCount = doc.Translations.Children.Count;

            // Create a column for each language requested
            for (int i = 0; i < translationCount; i++)
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition());

            // Create rows for each stanza
            // Don't forget one for each header too
            int numRows = doc.Translations?.CountRows() ?? 0;
            for (int i = 0; i <= numRows; i++)
                MainGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            var headerBlock = CreateHeader(doc.Name, lastRow != 0);
            MainGrid.Children.Add(headerBlock);
            Grid.SetColumnSpan(headerBlock, translationCount);
            Grid.SetRow(headerBlock, lastRow++);

            for (int t = 0; t < translationCount; t++)
            {
                ContentPart translation = doc.Translations[t];

                int i = 0;
                CreateFromContentPart(translation, block =>
                {
                    MainGrid.Children.Add(block);
                    Grid.SetColumn(block, t);
                    Grid.SetRow(block, lastRow + i++);
                });
            }
            return MainGrid;
        }

        private static void HandleLanguage(TextBox contentBlock, object content)
        {
            KnownLanguage lang = (content as IDefinition)?.GetLanguage()?.Known ?? KnownLanguage.Default;

            contentBlock.FontFamily = Common.DefaultFont;
            contentBlock.FontSize = Common.DefaultFontSize;
            switch (lang)
            {
                case KnownLanguage.Arabic:
                case KnownLanguage.Aramaic:
                case KnownLanguage.Hebrew:
                    contentBlock.TextAlignment = TextAlignment.Right;
                    break;

                case KnownLanguage.Coptic:
                case KnownLanguage.Greek:
                    // Font rendering is hard. UWP wants the combining character before,
                    // while certain HTML renderers can't make up their minds.
                    contentBlock.Text = CopticFont.SwapJenkimPosition(contentBlock.Text, CopticFont.CopticUnicode);

                    // TextBox doesn't seem to know where to break Greek or Coptic Unicode
                    // lines, so insert a zero-width space at every space so
                    // word wrap actually works
                    if (!contentBlock.Text.Contains('\u200B'))
                        contentBlock.Text = contentBlock.Text.Replace(" ", " \u200B");

                    break;
            }
        }
    }
}
