using CoptLib.Extensions;
using CoptLib.Models;
using CoptLib.Writing;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

using CLInline = CoptLib.Models.Text.Inline;
using CLRun = CoptLib.Models.Text.Run;
using CLSpan = CoptLib.Models.Text.Span;

namespace CopticChanter.Helpers
{
    public static class DocumentUIFactory
    {
        public static Grid CreateGridFromDoc(Doc doc) => CreateGridFromLayout(new DocLayout(doc));

        public static Grid CreateGridFromLayout(DocLayout layout) => CreateGridFromTable(layout.CreateTable());
        
        public static Grid CreateGridFromTable(List<List<object>> table)
        {
            Grid mainGrid = new Grid();
            mainGrid.HorizontalAlignment = HorizontalAlignment.Stretch;

            // Create a column for each language
            int columnCount = table.Max(r => r.Count);
            for (int i = 0; i < columnCount; i++)
                mainGrid.ColumnDefinitions.Add(new ColumnDefinition());

            for (int r = 0; r < table.Count; r++)
            {
                var row = table[r];

                // Create new row definition
                mainGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                for (int c = 0; c < row.Count; c++)
                {
                    var item = row[c];
                    var block = new TextBlock()
                    {
                        TextWrapping = TextWrapping.Wrap,
                    };

                    if (item is Doc) continue;

                    if (item is Section section)
                    {
                        if (section.Parent is null)
                            block.FontWeight = FontWeights.Bold;
                        else
                            block.FontWeight = FontWeights.SemiBold;

                        item = section.Title;
                    }

                    if (item is Comment)
                    {
                        block.Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Yellow);
                        block.FontSize *= 0.75;
                    }

                    if (item is RoleInfo role)
                    {
                        var previousItem = (IDefinition)table[r - 1][c];
                        item = role.Names.GetByLanguage(previousItem.GetLanguage());

                        block.FontWeight = FontWeights.Medium;
                        block.Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(0xFF, 0xFF, 0x8C, 0x00));
                    }

                    if (false && item is IContent content)
                    {
                        if (content.Inlines.Count == 1 && content.Inlines[0] is CLRun singleRun)
                        {
                            // Avoid using InlineCollection when Text can be used directly
                            item = singleRun;
                        }
                        else
                        {
                            // Convert the CoptLib InlineCollection to WUX
                            foreach (CLInline inline in content.Inlines)
                            {
                                Inline wuxInline = CreateWuxInline(inline);
                                block.Inlines.Add(wuxInline);
                            }
                        }
                    }
                    
                    block.Text = item.ToString();

                    HandleLanguage(block, item);
                    mainGrid.Children.Add(block);
                    Grid.SetRow(block, r);
                    Grid.SetColumn(block, c);
                }
            }

            return mainGrid;
        }

        private static void HandleLanguage(TextBlock contentBlock, object content)
        {
            LanguageInfo langInfo = (content as IDefinition)?.GetLanguage()
                ?? LanguageInfo.Default;
            KnownLanguage lang = langInfo.Known;

            contentBlock.FontFamily = Settings.GetFontFamily();
            contentBlock.FontSize = Settings.GetFontSize();

            var culture = langInfo.Secondary?.Culture ?? langInfo.Culture;
            if (culture != null)
            {
                contentBlock.TextAlignment = culture.TextInfo.IsRightToLeft
                    ? TextAlignment.Right : TextAlignment.Left;
            }

            contentBlock.Text = DisplayFont.Unicode.Convert(contentBlock.Text, Settings.GetDisplayFont());

            switch (lang)
            {
                case KnownLanguage.Coptic:
                case KnownLanguage.Greek:
                    // TextBlock doesn't seem to know where to break Greek or Coptic Unicode
                    // lines, so insert a zero-width space at every space so
                    // word wrap actually works
                    if (!contentBlock.Text.Contains('\u200B'))
                        contentBlock.Text = contentBlock.Text.Replace(" ", " \u200B");

                    break;
            }
        }

        private static Inline CreateWuxInline(CLInline inline)
        {
            Inline wuxInline = null;

            switch (inline)
            {
                case CLSpan span:
                    var wuxSpan = new Span();
                    foreach (CLInline childInline in span.Inlines)
                        wuxSpan.Inlines.Add(CreateWuxInline(childInline));
                    break;

                default:
                    wuxInline = new Run
                    {
                        Text = inline.ToString().Normalize(),
                    };
                    break;
            }

            return wuxInline;
        }
    }
}
