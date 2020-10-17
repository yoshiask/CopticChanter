using CoptLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
                    foreach (CoptLib.Models.Doc doc in Common.Docs)
                    {
                        doc.Translations.Any(t => t.Language == CopticInterpreter.Language.English);
                        Debug.WriteLine(doc.Name);

                        //if (doc.Language == CopticInterpreter.Language.Coptic)
                        //{
                        //    Common.AnyCoptic++;
                        //}
                        //else
                        //{
                        //    Common.AnyEnglish++;
                        //}

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
                            if (doc.Translations.Any(t => t.Language == CopticInterpreter.Language.English))
                                Common.AnyEnglish = true;
                            if (doc.Translations.Any(t => t.Language == CopticInterpreter.Language.Coptic))
                                Common.AnyCoptic = true;
                            if (doc.Translations.Any(t => t.Language == CopticInterpreter.Language.Arabic))
                                Common.AnyArabic = true;
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

            if (Common.AnyEnglish)
                langs.Add(CopticInterpreter.Language.English);
            if (Common.AnyCoptic)
                langs.Add(CopticInterpreter.Language.Coptic);
            if (Common.AnyArabic)
                langs.Add(CopticInterpreter.Language.Arabic);

            Frame.Navigate(typeof(Layouts.DocumentLayout),
                new Layouts.DocumentLayoutArgs(langs.ToArray())
            );
        }

        private async void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
			var picker = new Windows.Storage.Pickers.FileOpenPicker
			{
				ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
				SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
			};
			picker.FileTypeFilter.Add(".xml");
            picker.FileTypeFilter.Add(".txt");

            StorageFile file = await picker.PickSingleFileAsync();

            var doc = CopticInterpreter.ReadDocXml(file.Path);
            if (String.IsNullOrWhiteSpace(OutputBox.Text))
            {
                //OutputBox.Text += CopticInterpreter.ConvertFromString(doc.Content);
                InputBox.Text += doc.Translations[0];
            }
            else
            {
                //string input = String.Join("\r\n", doc.Translations[0].Content.Select(s => s.Text));
                //OutputBox.Text += CopticInterpreter.ConvertFont(
                //    input, CopticFont.Coptic1, CopticFont.CopticUnicode
                //);
                //InputBox.Text += input;
            }
            //OutputBox.Text = CopticInterpreter.ConvertFromString(InputBox.Text);
        }
    }
}
