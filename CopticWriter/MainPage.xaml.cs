using CoptLib;
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
        ObservableCollection<Doc> Docs { get; set; } = new ObservableCollection<Doc>();
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
            if (file != null)
            {
                var mru = Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList;
                _ = mru.Add(file, file.Name);

                switch (Path.GetExtension(file.Name))
                {
                    case ".xml":
                        // Read the file
                        var docXml = CopticInterpreter.ReadDocXml(await file.OpenStreamForReadAsync());
                        Docs.Add(docXml);
                        return;

                    case ".zip":
                        // Read the file
                        var set = CopticInterpreter.ReadSet(await file.OpenStreamForReadAsync(), file.Name, Windows.Storage.ApplicationData.Current.TemporaryFolder.Path);
                        Docs = new ObservableCollection<Doc>(set.IncludedDocs);
                        CurrentStanza = (set.IncludedDocs[0].Translations[0].Content[0] as Stanza)?.Text;
                        return;
                }
            }
        }

        private void ConvertTasbehaButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ScriptButton_Click(object sender, RoutedEventArgs e)
        {

        }

        #region Doc Controls
        private void DocDecrement_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DocIncrement_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DocDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DocCreate_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        

		private void MainTabControl_AddTabButtonClick(Microsoft.UI.Xaml.Controls.TabView sender, object args)
		{
            // TODO: Create new doc
		}
	}
}
