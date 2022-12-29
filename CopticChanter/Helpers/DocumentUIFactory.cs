using CoptLib.Models;
using CoptLib.Writing;
using System;
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
                    {
                        if (content.Inlines.SingleOrDefault() is CLRun singleRun)
                        {
                            // Avoid using InlineCollection when Text can be used directly
                            block.Text = singleRun.Text;
                        }
                        else
                        {
                            // Convert the CoptLib InlineCollection to WUX
                            foreach (CLInline inline in content.Inlines)
                            {
                                Inline wuxInline = CreateWUXInline(inline);
                                block.Inlines.Add(wuxInline);
                            }
                        }
                    }

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

        private static Inline CreateWUXInline(CLInline inline)
        {
            Inline wuxInline = null;

            switch (inline)
            {
                case CLRun run:
                    wuxInline = new Run
                    {
                        Text = run.Text,
                    };
                    break;

                case CLSpan span:
                    var wuxSpan = new Span();
                    foreach (CLInline childInline in span.Inlines)
                        wuxSpan.Inlines.Add(CreateWUXInline(childInline));
                    break;

                default:
                    throw new NotImplementedException();
            }

            return wuxInline;
        }
    }
}
