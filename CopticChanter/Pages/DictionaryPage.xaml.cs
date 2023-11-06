using CommunityToolkit.Mvvm.ComponentModel;
using CoptLib.Writing.Lexicon;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CopticChanter.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [ObservableObject]
    public sealed partial class DictionaryPage : Page
    {
        public DictionaryPage()
        {
            this.InitializeComponent();
        }

        [ObservableProperty]
        private LexiconEntry _selectedEntry;

        private async void SearchButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var query = SearchBox.Text;
            var lexicon = new CopticScriptoriumLexicon();

            await lexicon.InitAsync();

            SearchResultsView.Items.Clear();
            await foreach (var result in lexicon.SearchAsync(query, new CoptLib.Writing.LanguageInfo(CoptLib.Writing.KnownLanguage.CopticBohairic)))
            {
                SearchResultsView.Items.Add(result);
            }
        }

        public static string JoinOrthographies(IEnumerable<Form> forms)
            => string.Join(", ", forms.Select(f => f.Orthography));

        public static IEnumerable<Sense> FilterSenses(IEnumerable<Sense> senses)
        {
            foreach (var sense in senses)
            {
                for (int i = 0; i < sense.Translations.Count;)
                {
                    var translation = sense.Translations[i];
                    if (translation.ToString().Length == 0)
                        sense.Translations.RemoveAt(i);
                    else
                        ++i;
                }
            }

            return senses;
        }
    }
}
