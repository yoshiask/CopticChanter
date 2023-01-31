using CoptLib.IO;
using System;
using System.Collections.Generic;
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
            IReadOnlyList<StorageFile> files = await ApplicationData.Current.RoamingFolder.GetFilesAsync();
            try
            {
                if (files != null && files.Count > 0)
                {
                    Common.CurrentLoadContext.Clear();
                    foreach (StorageFile file in files)
                    {
                        Debug.WriteLine(file.Path);
                        var doc = Common.CurrentLoadContext.LoadDoc(file.Path);
                        Debug.WriteLine(doc.Name);
                    }

                    if (present)
                        Present();
                }
                else
                {
                    Frame.Navigate(typeof(FilesPage));
                }
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

                Frame.Navigate(typeof(FilesPage));
            }
        }

        private void Present()
        {
            Frame.Navigate(typeof(Layouts.DocumentLayout), new Layouts.DocumentLayoutArgs());
        }
    }
}
