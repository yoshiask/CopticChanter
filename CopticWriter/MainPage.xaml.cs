using CoptLib;
using CoptLib.IO;
using CoptLib.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CopticWriter
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ObservableCollection<Doc> Docs { get; } = new ObservableCollection<Doc>();
        public string CurrentStanza { get; set; }

        public MainPage()
        {
            this.InitializeComponent();

            // Change default title bar
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            Window.Current.SetTitleBar(TitleGrid);

            // Set the caption buttons to transparent
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.InactiveBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            TitleGrid.Height = sender.Height;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileSavePicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeChoices.Add("DocCreator XML Document", new List<string> { ".xml" });
            picker.FileTypeChoices.Add("DocCreator ZIP Set", new List<string> { ".zip" });

            Windows.Storage.StorageFile file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                // Read the file
            }
        }

        private async void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".xml");
            picker.FileTypeFilter.Add(".zip");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file == null) return;

            var mru = Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList;
            _ = mru.Add(file, file.Name);

            var stream = await file.OpenStreamForReadAsync();
            switch (Path.GetExtension(file.Name))
            {
                case ".xml":
                    // Read the file
                    var docXml = DocReader.ReadDocXml(stream);
                    Docs.Add(docXml);
                    MainTabControl.SelectedIndex = Docs.Count - 1;
                    return;

                case ".zip":
                    // Read the set
                    var set = DocSetReader.ReadSet(stream, file.Name, Windows.Storage.ApplicationData.Current.TemporaryFolder.Path);
                    Docs.Clear();
                    set.IncludedDocs.ForEach(d => Docs.Add(d));
                    MainTabControl.SelectedIndex = 0;
                    return;
            }
        }

        private void ConvertTasbehaButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ScriptButton_Click(object sender, RoutedEventArgs e)
        {

        }

		private void MainTabControl_AddTabButtonClick(Microsoft.UI.Xaml.Controls.TabView sender, object args)
		{
            // TODO: Create new doc
            Docs.Add(new Doc()
            {
                Name = "Untitled",
                Key = "42c70071-ce5e-4add-aa5c-d093acfb2784"
            });
        }
	}
}
