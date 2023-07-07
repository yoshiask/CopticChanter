using CoptLib.Models;
using CoptLib.ViewModels;
using OwlCore.Storage;
using OwlCore.Storage.SharpCompress;
using OwlCore.Storage.Uwp;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.MediaProperties;
using Windows.Media.SpeechSynthesis;
using Windows.Media.Transcoding;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CopticChanter
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async Task LoadDocs(bool present = false)
        {
            var roamingFolder = new WindowsStorageFolder(ApplicationData.Current.RoamingFolder);

            try
            {
                DocSetViewModel setVm;

                var meta = await ApplicationData.Current.RoamingFolder.TryGetItemAsync("meta.xml");
                if (meta != null)
                {
                    setVm = await DocSetViewModel.LoadFromFolder(roamingFolder);
                }
                else
                {
                    var setZipItem = await ApplicationData.Current.RoamingFolder.TryGetItemAsync("set.zip");
                    if (setZipItem is IStorageFile setZipStorageFile)
                    {
                        var setZipFile = new WindowsStorageFile(setZipStorageFile);
                        var setZipFolder = new ReadOnlyArchiveFolder(setZipFile);
                        setVm = await DocSetViewModel.LoadFromFolder(setZipFolder);
                    }
                    else
                    {
                        var set = new DocSet("adhoc", "Coptic Chanter");
                        await foreach (var file in roamingFolder.GetFilesAsync())
                        {
                            using var fileStream = await file.OpenStreamAsync();
                            set.IncludedDocs.Add(set.Context.LoadDoc(fileStream));
                        }

                        setVm = new DocSetViewModel(set);
                    }
                }

                if (present)
                    Present(setVm);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);

                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = ex,
                    CloseButtonText = "Ok"
                };

                await errorDialog.ShowAsync();
            }
        }

        private void Present(DocSetViewModel vm)
        {
            Frame.Navigate(typeof(Layouts.DocumentLayout), new Layouts.DocumentLayoutArgs(vm));
        }

        private async void LoadDocsButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await LoadDocs(true);
        }

        private void TtsButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Pages.TextToSpeechPage));
        }
    }
}
