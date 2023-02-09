using CoptLib.ViewModels;
using OwlCore.Storage.Uwp;
using System;
using System.Diagnostics;
using Windows.Storage;
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
            LoadDocs(true);
        }

        private async void LoadDocs(bool present = false)
        {
            var roamingFolder = new WindowsStorageFolder(ApplicationData.Current.RoamingFolder);

            try
            {
                var setVm = await DocSetViewModel.ReadFromFile(roamingFolder);

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
    }
}
