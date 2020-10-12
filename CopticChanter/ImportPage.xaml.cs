using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CopticChanter
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [Obsolete("This page will not import files properly. Use FilesPage instead.", true)]
    public sealed partial class ImportPage : Page
    {
        public ImportPage()
        {
            this.InitializeComponent();
            DirectoryBox.Text = ApplicationData.Current.LocalFolder.Path;
        }

        private async void ImportButton_Click(object sender, RoutedEventArgs e)
        {
			var picker = new Windows.Storage.Pickers.FileOpenPicker
			{
				ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
				SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
			};
			picker.FileTypeFilter.Add(".xml");

            StorageFile file = await picker.PickSingleFileAsync();

            try
            {
                if (file != null)
                {
                    var copy = await file.CopyAsync(ApplicationData.Current.LocalFolder);
                    if (copy != null)
                    {
                        StatusBlock.Visibility = Visibility.Visible;
                        StatusBlock.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 15, 166, 0));
                        StatusBlock.Text = "Successfully copied " + copy.Name;
                    }
                }
            }
            catch
            {
                StatusBlock.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 166, 0, 0));
                StatusBlock.Text = "Error copying " + file.Name;
            }
        }

        private void PresentButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
