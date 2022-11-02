using CopticChanter.Helpers;
using CoptLib.Models;
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
            Background = new SolidColorBrush(args.BackColor);
            Foreground = new SolidColorBrush(args.ForeColor);

            // Populate the columns with translations and rows with stanzas
            foreach (Doc doc in Common.Docs)
            {
                // Apply transforms before display
                CoptLib.IO.DocReader.ApplyDocTransforms(doc);

                Grid MainGrid = DocumentUIFactory.CreateGridFromDoc(doc);
                MainPanel.Children.Add(MainGrid);
            }

            base.OnNavigatedTo(e);
        }
    }
}
