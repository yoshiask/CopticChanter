using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CopticChanter.Helpers;
using CoptLib.Models;
using CoptLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Audio;
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
    [ObservableObject]
    public sealed partial class DocumentLayout : Page
    {
        public DocumentLayout()
        {
            this.InitializeComponent();
        }

        public DocSetViewModel ViewModel { get; } = new DocSetViewModel(Common.Docs);

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var args = e.Parameter as DocumentLayoutArgs;
            ApplicationView.GetForCurrentView().FullScreenSystemOverlayMode = FullScreenSystemOverlayMode.Minimal;
            ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            Background = new SolidColorBrush(args.BackColor);
            Foreground = new SolidColorBrush(args.ForeColor);

            ViewModel.Layout.CollectionChanged += LayoutChanged;
            ViewModel.CreateLayoutCommand.Execute(null);

            base.OnNavigatedTo(e);
        }

        private async void LayoutChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate
            {
                if (e.Action != System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                    return;

                foreach (var item in e.NewItems)
                {
                    var layout = (IList<IList<object>>)((IList<ObservableCollection<object>>)item)
                        .Select(r => (IList<object>)r)
                        .ToList();
                    var grid = DocumentUIFactory.CreateGridFromLayout(layout);
                    MainPanel.Children.Add(grid);
                }
            });
        }
    }
}
