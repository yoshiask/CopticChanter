using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using System.Diagnostics;
using CoptLib;
using Windows.UI;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CopticChanter
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FilesPage : Page
    {
        Color[] langColors = {
            Windows.UI.Color.FromArgb(255, 139, 133, 191), // English
            Windows.UI.Color.FromArgb(255, 153, 148, 98), // Coptic
            Windows.UI.Color.FromArgb(255, 74, 128, 151) // Arabic
        };
        IList<StorageFile> Files;

        public FilesPage()
        {
            this.InitializeComponent();
        }

        private async void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".xml");

            StorageFile file = await picker.PickSingleFileAsync();

            try
            {
                if (file != null)
                {
                    var copy = await file.CopyAsync(ApplicationData.Current.RoamingFolder);
                    if (copy != null)
                    {
                        Debug.WriteLine(file.Path);
                        var doc = CopticInterpreter.ReadDocXML(file.Path);
                        string lang;
                        if (doc.Coptic)
                        {
                            lang = "C";
                        }
                        else
                        {
                            lang = "E";
                        }

                        FileView.Items.Add(new ListViewItem()
                        {
                            Content = $"[{lang}] {doc.Name}",
                        });
                        Files.Add(copy);
                        //Common.Docs.Add(doc);

                        StatusBlock.Visibility = Visibility.Visible;
                        StatusBlock.Foreground = new SolidColorBrush(Color.FromArgb(255, 15, 166, 0));
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

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DirectoryBox.Text = ApplicationData.Current.RoamingFolder.Path;

            // Begin loading files
            var readFiles = await ApplicationData.Current.RoamingFolder.GetFilesAsync();
            Files = readFiles.ToList();
            try
            {
                if (Files != null)
                {
                    if (Files.Count > 0)
                    {
                        Common.Docs.Clear();
                        foreach (StorageFile file in Files)
                        {
                            Debug.WriteLine(file.Path);
                            var doc = CopticInterpreter.ReadDocXML(file.Path);
                            string lang;
                            if (doc.Coptic)
                            {
                                lang = "C";
                            }
                            else
                            {
                                lang = "E";
                            }

                            FileView.Items.Add(new ListViewItem()
                            {
                                Content = "[" + lang + "] " + doc.Name,
                            });
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

            // Check version for Fluent Design
            try
            {
                if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.Xaml.Media.XamlCompositionBrushBase"))
                {
                    AcrylicBrush BackBrush = new AcrylicBrush();
                    BackBrush.BackgroundSource = AcrylicBackgroundSource.HostBackdrop;
                    BackBrush.TintColor = Color.FromArgb(255, 246, 246, 246);
                    BackBrush.FallbackColor = Color.FromArgb(255, 246, 246, 246);
                    BackBrush.TintOpacity = 0.6;
                    MainGrid.Background = BackBrush;

                    AcrylicBrush BarBrush = new AcrylicBrush();
                    BarBrush.BackgroundSource = AcrylicBackgroundSource.HostBackdrop;
                    BarBrush.TintColor = Common.GetAccentBrush().Color;
                    BarBrush.FallbackColor = Common.GetAccentBrush().Color;
                    BarBrush.TintOpacity = 0.6;
                    BottomBar.Background = BarBrush;
                }
                else
                {
                    SolidColorBrush BackBrush = new SolidColorBrush(Color.FromArgb(255, 246, 246, 246));
                    MainGrid.Background = BackBrush;

                    BottomBar.Background = Common.GetAccentBrush();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (FileView.SelectedIndex > -1)
            {
                try
                {
                    string name = Files[FileView.SelectedIndex].Name;
                    await Files[FileView.SelectedIndex].DeleteAsync();
                    Files.RemoveAt(FileView.SelectedIndex);
                    //Common.Docs.RemoveAt(FileView.SelectedIndex);
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
    }
}
