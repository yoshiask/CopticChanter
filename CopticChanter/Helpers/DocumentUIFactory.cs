using CoptLib.Models;
using CoptLib.Writing;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CopticChanter.Helpers
{
    public static class DocumentUIFactory
    {
        public static void CreateFromContentPart(ContentPart part, Action<TextBlock> action)
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
                    foreach (TextBlock block in blocks)
                        action(block);
                    break;
            }
        }

        public static TextBlock CreateBlockFromContent(IContent contentPart)
        {
            var contentBlock = new TextBlock
            {
                Text = contentPart.Text,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Left
            };

            HandleLanguage(contentBlock, contentPart);

            return contentBlock;
        }

        public static List<TextBlock> CreateBlocksFromContentCollectionContainer(IContentCollectionContainer container)
        {
            var blocks = new List<TextBlock>(container.Children.Count + 1);

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

        public static TextBlock CreateHeader(string title, bool addPadding = true)
        {
            var headerBlock = new TextBlock
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

        public static TextBlock CreateSubheader(IContent title, bool addPadding = true)
        {
            var headerBlock = new TextBlock
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

        public static Grid CreateGridFromDoc(Doc doc) => CreateGridFromLayout((IList<IList<object>>)doc.Flatten());

        public static Grid CreateGridFromLayout(IList<IList<object>> layout)
        {
            Grid MainGrid = new Grid();
            MainGrid.HorizontalAlignment = HorizontalAlignment.Stretch;

            // Create a column for each language
            int columnCount = layout.Max(r => r.Count);
            for (int i = 0; i < columnCount; i++)
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition());

            bool isNewDoc = false;
            for (int r = 0; r < layout.Count; r++)
            {
                var row = layout[r];

                // Create new row definition
                MainGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                if (row.Count == 1 && row[0] is Doc)
                {
                    // Set flag so the title of the translations
                    // can be displayed in bold font
                    isNewDoc = true;
                    continue;
                }

                for (int c = 0; c < row.Count; c++)
                {
                    var item = row[c];
                    var block = new TextBlock()
                    {
                        TextWrapping = TextWrapping.Wrap,
                    };

                    if (item is Section section)
                    {
                        item = section.Title;

                        if (isNewDoc)
                            block.FontWeight = FontWeights.Bold;
                        else
                            block.FontWeight = FontWeights.SemiBold;
                    }

                    if (item is IContent content)
                        block.Text = content.Text;

                    HandleLanguage(block, item);
                    MainGrid.Children.Add(block);
                    Grid.SetRow(block, r);
                    Grid.SetColumn(block, c);
                }

                isNewDoc = false;
            }

            return MainGrid;
        }

        private static void HandleLanguage(TextBlock contentBlock, object content)
        {
            LanguageInfo langInfo = (content as IMultilingual)?.Language
                ?? LanguageInfo.Default;
            KnownLanguage lang = langInfo.Known;

            contentBlock.FontFamily = Common.GetFontFamily(lang);
            contentBlock.FontSize = Common.GetFontSize(lang);

            var culture = langInfo.Secondary?.Culture ?? langInfo.Culture;
            if (culture != null)
            {
                contentBlock.TextAlignment = culture.TextInfo.IsRightToLeft
                    ? TextAlignment.Right : TextAlignment.Left;
            }

            switch (lang)
            {
                case KnownLanguage.Coptic:
                case KnownLanguage.Greek:
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
        }
    }
}
