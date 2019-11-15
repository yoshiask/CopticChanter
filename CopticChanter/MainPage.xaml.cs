using CoptLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Storage;
using Windows.UI.Xaml;
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
            WinVer.GetWinVer();
            LoadDocs(true);
        }

        private async void NewLoadDocs(bool present = false)
        {
            OutputBox.Text = "";
            InputBox.Text = "";
            try
            {
                if (Common.Docs.Count > 0)
                {
                    foreach (CoptLib.XML.DocXML doc in Common.Docs)
                    {
                        Debug.WriteLine(doc.Name);

                        if (doc.Coptic)
                        {
                            Common.CopticDocCount++;
                        }
                        else
                        {
                            Common.EnglishDocCount++;
                        }

                        if (present)
                        {
                            Present();
                        }
                    }
                }
                else
                {
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

        private async void LoadDocs(bool present = false)
        {
            OutputBox.Text = "";
            InputBox.Text = "";
            IReadOnlyList<StorageFile> files = await ApplicationData.Current.RoamingFolder.GetFilesAsync();
            try
            {
                if (files != null)
                {
                    if (files.Count > 0)
                    {
                        Common.Docs.Clear();
                        foreach (StorageFile file in files)
                        {
                            Debug.WriteLine(file.Path);
                            var doc = CopticInterpreter.ReadDocXML(file.Path);

                            Common.Docs.Add(doc);
                            if (doc.Coptic)
                            {
                                Common.CopticDocCount++;
                            }
                            else
                            {
                                Common.EnglishDocCount++;
                            }

                            if (present)
                            {
                                Present();
                            }
                        }
                    }
                    else
                    {
                        Frame.Navigate(typeof(FilesPage));
                    }
                }
                else
                {
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

        private void Present()
        {
            bool eng = Common.EnglishDocCount > 0;
            Debug.WriteLine("English: " + eng);
            bool copt = Common.CopticDocCount > 0;
            Debug.WriteLine("Coptic: " + copt);
            bool arabic = Common.ArabicDocCount > 0;
            Debug.WriteLine("Arabic: " + arabic);

            if (eng && !copt && !arabic)
            {
                Common.OnePanelArgs = new Layouts.SinglePanelArgs(
                    Common.Language.English,
                    Windows.UI.Colors.Black,
                    Windows.UI.Colors.White);
                Frame.Navigate(typeof(Layouts.SinglePanel));
            }
            else if (!eng && copt && !arabic)
            {
                Common.OnePanelArgs = new Layouts.SinglePanelArgs(
                    Common.Language.Coptic,
                    Windows.UI.Colors.Black,
                    Windows.UI.Colors.White);
                Frame.Navigate(typeof(Layouts.SinglePanel));
            }
            else if (!eng && !copt && arabic)
            {
                Common.OnePanelArgs = new Layouts.SinglePanelArgs(
                    Common.Language.Arabic,
                    Windows.UI.Colors.Black,
                    Windows.UI.Colors.White);
                Frame.Navigate(typeof(Layouts.SinglePanel));
            }

            else if (eng && copt && !arabic)
            {
                Common.TwoPanelArgs = new Layouts.DoublePanelArgs(
                    Common.Language.English,
                    Common.Language.Coptic,
                    Windows.UI.Colors.Black,
                    Windows.UI.Colors.White);
                Frame.Navigate(typeof(Layouts.DoublePanel));
            }
            else if (!eng && copt && arabic)
            {
                Common.TwoPanelArgs = new Layouts.DoublePanelArgs(
                    Common.Language.Coptic,
                    Common.Language.Arabic,
                    Windows.UI.Colors.Black,
                    Windows.UI.Colors.White);
                Frame.Navigate(typeof(Layouts.DoublePanel));
            }
            else if (eng && !copt && arabic)
            {
                Common.TwoPanelArgs = new Layouts.DoublePanelArgs(
                    Common.Language.English,
                    Common.Language.Arabic,
                    Windows.UI.Colors.Black,
                    Windows.UI.Colors.White);
                Frame.Navigate(typeof(Layouts.DoublePanel));
            }
        }

        private async void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".xml");
            picker.FileTypeFilter.Add(".txt");

            StorageFile file = await picker.PickSingleFileAsync();

            var doc = CopticInterpreter.ReadDocXML(file.Path);
            if (String.IsNullOrWhiteSpace(OutputBox.Text))
            {
                //OutputBox.Text += CopticInterpreter.ConvertFromString(doc.Content);
                InputBox.Text += doc.Stanzas[0].Content;
            }
            else
            {
                OutputBox.Text += CopticInterpreter.ConvertFromString("- -" + doc.Stanzas[0].Content);
                InputBox.Text += "- -" + doc.Stanzas[0].Content;
            }
            //OutputBox.Text = CopticInterpreter.ConvertFromString(InputBox.Text);
        }
    }
}
