using CoptLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
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

        private async Task NewLoadDocs(bool present = false)
        {
            OutputBox.Text = "";
            InputBox.Text = "";
            try
            {
                if (Common.Docs.Count > 0)
                {
                    foreach (CoptLib.XML.DocXml doc in Common.Docs)
                    {
                        Debug.WriteLine(doc.Name);

                        if (doc.Language == CopticInterpreter.Language.Coptic)
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
                            var doc = CopticInterpreter.ReadDocXml(file.Path);

                            Common.Docs.Add(doc);
                            if (doc.Language == CopticInterpreter.Language.Coptic)
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
            var langs = new List<CopticInterpreter.Language>(3);

            if (Common.EnglishDocCount > 0)
                langs.Add(CopticInterpreter.Language.English);
            if (Common.CopticDocCount > 0)
                langs.Add(CopticInterpreter.Language.Coptic);
            if (Common.ArabicDocCount > 0)
                langs.Add(CopticInterpreter.Language.Arabic);

            if (langs.Count == 1)
			{
                Frame.Navigate(typeof(Layouts.SinglePanel),
                    new Layouts.SinglePanelArgs(langs.ToArray())
                );
            }
            else if (langs.Count == 2)
			{
                Frame.Navigate(typeof(Layouts.DoublePanel),
                    new Layouts.DoublePanelArgs(langs.ToArray())
                );
            }
            else if (langs.Count == 3)
			{
                //Frame.Navigate(typeof(Layouts.TriplePanel),
                //    new Layouts.TriplePanelArgs(langs.ToArray())
                //);
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

            var doc = CopticInterpreter.ReadDocXml(file.Path);
            if (String.IsNullOrWhiteSpace(OutputBox.Text))
            {
                //OutputBox.Text += CopticInterpreter.ConvertFromString(doc.Content);
                InputBox.Text += doc.Content[0];
            }
            else
            {
                OutputBox.Text += CopticInterpreter.ConvertFont(doc.Content[0], CopticFont.Coptic1, CopticFont.CopticUnicode);
                InputBox.Text += "- -" + doc.Content[0];
            }
            //OutputBox.Text = CopticInterpreter.ConvertFromString(InputBox.Text);
        }
    }
}
