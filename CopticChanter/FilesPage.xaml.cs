using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.Storage;
using System.Diagnostics;
using CoptLib;
using Windows.UI;
using CoptLib.IO;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CopticChanter
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FilesPage : Page
    {
        Color[] _langColors = {
            Windows.UI.Color.FromArgb(255, 139, 133, 191), // English
            Windows.UI.Color.FromArgb(255, 153, 148, 98), // Coptic
            Windows.UI.Color.FromArgb(255, 74, 128, 151) // Arabic
        };
        IList<StorageFile> _files;

        public FilesPage()
        {
            this.InitializeComponent();
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
                    var copy = await file.CopyAsync(ApplicationData.Current.RoamingFolder);
                    if (copy != null)
                    {
                        ImportDoc(copy);
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

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DirectoryBox.Text = ApplicationData.Current.RoamingFolder.Path;

            // Begin loading files
            var readFiles = await ApplicationData.Current.RoamingFolder.GetFilesAsync();
            _files = readFiles.ToList();
            try
            {
                if (_files != null)
                {
                    if (_files.Count > 0)
                    {
                        Common.CurrentLoadContext.Clear();
                        foreach (StorageFile file in _files)
                        {
                            ImportDoc(file);
                        }
                    }
                    else
                    {
                        ContentDialog dialog = new ContentDialog()
                        {
                            Title = "Error",
                            Content = "No files to open",
                            CloseButtonText = "Ok"
                        };
                        await dialog.ShowAsync();
                    }
                }
                else
                {
                    ContentDialog dialog = new ContentDialog()
                    {
                        Title = "Error",
                        Content = "Something went wrong when loading files",
                        CloseButtonText = "Ok"
                    };
                    await dialog.ShowAsync();
                    Frame.Navigate(typeof(FilesPage));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);

                ContentDialog errorDialog = new ContentDialog()
                {
                    Title = "Error",
                    Content = ex,
                    CloseButtonText = "Ok"
                };

                await errorDialog.ShowAsync();

                Frame.Navigate(typeof(FilesPage));
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (FileView.SelectedIndex > -1)
            {
                try
                {
                    string name = _files[FileView.SelectedIndex].Name;
                    await _files[FileView.SelectedIndex].DeleteAsync();
                    _files.RemoveAt(FileView.SelectedIndex);
                    //Common.CurrentLoadContext.RemoveAt(FileView.SelectedIndex);
                    FileView.Items.Remove(FileView.SelectedItem);
                    StatusBlock.Visibility = Visibility.Visible;
                    StatusBlock.Foreground = new SolidColorBrush(Color.FromArgb(255, 15, 166, 0));
                    StatusBlock.Text = "Successfully deleted " + name;
                }
                catch
                {
                    StatusBlock.Foreground = new SolidColorBrush(Color.FromArgb(255, 166, 0, 0));
                    StatusBlock.Text = "Error deleting selected file";
                }
            }
        }

        private void ImportDoc(StorageFile file)
		{
            Debug.WriteLine(file.Path);
            var doc = Common.CurrentLoadContext.LoadDoc(file.Path);

            FileView.Items.Add(new ListViewItem
            {
                Content = $"{doc.Name} [{doc.Uuid}]",
            });
            _files.Add(file);

            StatusBlock.Visibility = Visibility.Visible;
            StatusBlock.Foreground = new SolidColorBrush(Color.FromArgb(255, 15, 166, 0));
            StatusBlock.Text = "Successfully copied " + file.Name;
        }
    }
}
