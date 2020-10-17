using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CopticWriter
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        ObservableCollection<CoptLib.Models.Doc> _docs = new ObservableCollection<CoptLib.Models.Doc>();
        ObservableCollection<CoptLib.Models.Doc> Docs {
            get { return _docs; }
            set {
                _docs = value;
                OnPropertyChanged();
            }
        }
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
                switch (Path.GetExtension(file.Name))
                {
                    case ".xml":
                        // Read the file
                        var docXml = CoptLib.CopticInterpreter.ReadDocXml(await file.OpenStreamForReadAsync());
                        HideDocControls();
                        return;

                    case ".zip":
                        // Read the file
                        var set = CoptLib.CopticInterpreter.ReadSet(await file.OpenStreamForReadAsync(), file.Name, Windows.Storage.ApplicationData.Current.TemporaryFolder.Path);
                        ShowDocControls();
                        Docs = new ObservableCollection<CoptLib.Models.Doc>(set.IncludedDocs);
                        CurrentStanza = set.IncludedDocs[0].Translations[0].Text;
                        OnPropertyChanged();
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

        private void HideDocControls()
        {
            DocControlButtons.Visibility = Visibility.Collapsed;
        }
        private void ShowDocControls()
        {
            DocControlButtons.Visibility = Visibility.Visible;
        }
        #endregion

        #region Stanza Controls
        private void StanzaDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void StanzaCreate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void StanzaDecrement_Click(object sender, RoutedEventArgs e)
        {

        }

        private void StanzaIncrement_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
