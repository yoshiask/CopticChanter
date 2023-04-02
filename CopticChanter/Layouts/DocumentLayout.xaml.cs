using CommunityToolkit.Mvvm.ComponentModel;
using CopticChanter.Helpers;
using CoptLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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

        public DocSetViewModel ViewModel
        {
            get => (DocSetViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            nameof(ViewModel), typeof(DocSetViewModel), typeof(DocumentLayout), new PropertyMetadata(null));

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var args = e.Parameter as DocumentLayoutArgs;
            Background = new SolidColorBrush(args.BackColor);
            Foreground = new SolidColorBrush(args.ForeColor);

            ViewModel = args.ViewModel;
            ViewModel.Tables.CollectionChanged += TablesChanged;
            ViewModel.CreateTablesCommand.Execute(null);

            base.OnNavigatedTo(e);

            ApplicationView.GetForCurrentView().FullScreenSystemOverlayMode = FullScreenSystemOverlayMode.Minimal;
            ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
        }

        private async void TablesChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
                {
                    MainPanel.Children.Clear();
                }
                else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    foreach (var item in e.NewItems)
                    {
                        var table = (List<List<object>>)item;
                        var grid = DocumentUIFactory.CreateGridFromTable(table);
                        MainPanel.Children.Add(grid);
                    }
                }
            });
        }
    }
}
