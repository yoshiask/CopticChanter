using CoptLib.XML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Text;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CopticChanter.Layouts
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DocumentLayout : Page
    {
        public DocumentLayout()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var args = e.Parameter as DocumentLayoutArgs;
            ApplicationView.GetForCurrentView().FullScreenSystemOverlayMode = FullScreenSystemOverlayMode.Minimal;
            ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            MainGrid.Background = new SolidColorBrush(args.BackColor);

            // Create a column for each language requested
            foreach (var lang in args.Languages)
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition());

            // Create rows for each stanza
            // Don't forget one for each header too
            int numRows = Common.Docs.Select(d => d.Content.Max(t => t.Stanzas.Count)).Sum() + Common.Docs.Count;
            for (int i = 0; i <= numRows; i++)
                MainGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            // Populate the columns with translations and rows with stanzas
            // TODO: Multiple docs requires multiple headers, which will offset the following items.
            int lastRow = 0;
            foreach (Doc doc in Common.Docs)
            {
                var headerBlock = new TextBlock
                {
                    Text = doc.Name,
                    FontFamily = Common.Segoe,
                    FontWeight = FontWeights.Bold,
                    FontSize = Common.GetEnglishFontSize() * 1.25,
                    TextWrapping = TextWrapping.Wrap,
                    Foreground = new SolidColorBrush(args.ForeColor)
                };
                if (lastRow != 0)
                {
                    // Every header except for the first one should have a top margin
                    // to distinguish it from the previous document
                    headerBlock.Margin = new Thickness(0, 20, 0, 0);
                }

                MainGrid.Children.Add(headerBlock);
                Grid.SetColumnSpan(headerBlock, args.Languages.Length);
                Grid.SetRow(headerBlock, lastRow);
                lastRow++;

                foreach (Translation translation in doc.Content)
                {
                    int column = args.Languages.ToList().IndexOf(translation.Language);
                    if (column < 0)
                        continue;

                    var blocks = Common.TextBlocksFromTranslation(translation, args.ForeColor);
                    for (int i = 0; i < translation.Stanzas.Count; i++)
                    {
                        MainGrid.Children.Add(blocks[i]);
                        Grid.SetColumn(blocks[i], column);
                        Grid.SetRow(blocks[i], lastRow + i);
                    }
                }

                // Account for the header, subheaders, and stanzas
                lastRow += doc.Content.Max(t => t.Stanzas.Count) + 1;
            }

            base.OnNavigatedTo(e);
        }
    }
}
