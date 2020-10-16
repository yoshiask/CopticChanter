using CoptLib;
using CoptLib.XML;
using System.Collections.Generic;
using System.Linq;
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
                        Text = Scripting.ParseTextCommands(stanza.Text),
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
                        Text = Scripting.ParseTextCommands(stanza.Text.Replace(" ", " \u200B")),
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

        public static List<TextBlock> CreateBlocksFromSection(Section section, Language translationLanguage, Color foreground)
		{
            var blocks = new List<TextBlock>(section.Content.Count + 1);

            var headerBlock = CreateSubheader(section.Title, foreground);
            blocks.Add(headerBlock);

            foreach (ContentPart part in section.Content)
            {
                if (part is Stanza stanza)
                {
                    var block = CreateBlockFromStanza(stanza, translationLanguage, foreground);
                    blocks.Add(block);
                }
                else if (part is Section subsection)
                {
                    var subblocks = CreateBlocksFromSection(subsection, translationLanguage, foreground);
                    blocks.AddRange(subblocks);
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
            int numRows = doc.Translations.Max(t =>
            {
                int count = 0;
                foreach (ContentPart part in t.Content)
				{
                    if (part is Stanza)
                        count++;
                    else if (part is Section section)
                        count += section.Content.Count + 1;
				}
                return count;
            });
            for (int i = 0; i <= numRows; i++)
                MainGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            var headerBlock = CreateHeader(doc.Name, foreground, lastRow != 0);
            MainGrid.Children.Add(headerBlock);
            Grid.SetColumnSpan(headerBlock, doc.Translations.Count);
            Grid.SetRow(headerBlock, lastRow++);

            for (int t = 0; t < doc.Translations.Count; t++)
			{
                var translation = doc.Translations[t];

                int i = 0;
                foreach (ContentPart part in translation.Content)
                {
                    if (part is Stanza stanza)
                    {
                        var block = CreateBlockFromStanza(stanza, translation.Language, foreground);
                        MainGrid.Children.Add(block);
                        Grid.SetColumn(block, t);
                        Grid.SetRow(block, lastRow + i++);
                    }
                    else if (part is Section section)
					{
                        var blocks = CreateBlocksFromSection(section, translation.Language, foreground);
                        foreach (TextBlock block in blocks)
						{
                            MainGrid.Children.Add(block);
                            Grid.SetColumn(block, t);
                            Grid.SetRow(block, lastRow + i++);
                        }
                    }
                }
            }
            return MainGrid;
        }
    }
}
