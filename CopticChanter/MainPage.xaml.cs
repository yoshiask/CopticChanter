using CoptLib.ViewModels;
using OwlCore.Storage;
using OwlCore.Storage.Uwp;
using System;
using System.Diagnostics;
using System.Linq;
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

            media.SetMediaPlayer(new Windows.Media.Playback.MediaPlayer());
            media.TransportControls.IsCompact = true;
            media.TransportControls.IsFullWindowButtonVisible = false;
        }

        private async Task LoadDocs(bool present = false)
        {
            var meta = await ApplicationData.Current.RoamingFolder.TryGetItemAsync("meta.xml");
            var roamingFolder = new WindowsStorageFolder(ApplicationData.Current.RoamingFolder);

            try
            {
                DocSetViewModel setVm;

                if (meta != null)
                {
                    setVm = await DocSetViewModel.ReadFromFile(roamingFolder);
                }
                else
                {
                    var set = new CoptLib.Models.DocSet("adhoc", "Coptic Chanter");
                    await foreach (var file in roamingFolder.GetFilesAsync())
                    {
                        using var fileStream = await file.OpenStreamAsync();
                        set.IncludedDocs.Add(set.Context.LoadDoc(fileStream));
                    }

                    setVm = new DocSetViewModel(set);
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

        private async void ListenButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // The string to speak with SSML customizations.
            //string ipa = "niˌaŋˈgeˌlos";
            string ipa = inBox.Text.ToLower();// "Tɛn.u.ɔʃt ni.aŋ.ge.los";
            string ssml =
                $"<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"el-GR\">\r\n    <voice name=\"el-GR-NestorasNeural\">\r\n        <s>\r\n            {inBox.Text}</s>\r\n    </voice>\r\n</speak>";

            var analyzer = new CoptLib.Writing.Linguistics.Analyzers.CopticGrecoBohairicAnalyzer();
            (ssml, ipaOut.Text) = analyzer.GenerateSsml(inBox.Text);

            // The media object for controlling and playing audio.
            var mediaElement = this.media.MediaPlayer;

            // The object for controlling the speech synthesis engine (voice).
            var synth = new SpeechSynthesizer();
            synth.Options.SpeakingRate = 0.7;
            //synth.Voice = SpeechSynthesizer.AllVoices[new Random().Next() % SpeechSynthesizer.AllVoices.Count];

            try
            {
                // Generate the audio stream from plain text.
                //SpeechSynthesisStream stream = await synth.SynthesizeSsmlToStreamAsync(ssml);
                SpeechSynthesisStream stream = await synth.SynthesizeSsmlToStreamAsync(ssml);

                // Send the stream to the media object.
                media.Tag = stream;
                mediaElement.Source = MediaSource.CreateFromStream(stream, stream.ContentType);
                mediaElement.Play();
            }
            catch (Exception)
            {
                
            }
        }

        private async void SaveButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var speechStream = media.Tag as SpeechSynthesisStream;
            string fileNameNoExt = DateTimeOffset.Now.ToString("yyyy-dd-M_HH-mm-ss");

            var roamingFolder = ApplicationData.Current.RoamingFolder;
            var synthFolder = await roamingFolder.CreateFolderAsync("synth", CreationCollisionOption.OpenIfExists);
            var wavFile = await synthFolder.CreateFileAsync(fileNameNoExt + ".wav");
            var mp3File = await synthFolder.CreateFileAsync(fileNameNoExt + ".mp3");

            using (var reader = new DataReader(speechStream.GetInputStreamAt(0)))
            {
                await reader.LoadAsync((uint)speechStream.Size);
                var buffer = new byte[(int)speechStream.Size];
                reader.ReadBytes(buffer);
                await FileIO.WriteBytesAsync(wavFile, buffer);
            }

            // Convert to MP3
            var transcoder = new MediaTranscoder();
            var profile = MediaEncodingProfile.CreateMp3(AudioEncodingQuality.High);

            PrepareTranscodeResult prepareOp = await
                transcoder.PrepareFileTranscodeAsync(wavFile, mp3File, profile);
            await prepareOp.TranscodeAsync();
            await wavFile.DeleteAsync();
        }

        private async void LoadDocsButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await LoadDocs(true);
        }
    }
}
