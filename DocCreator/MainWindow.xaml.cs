using CoptLib.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;

namespace DocCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool _shift = false;
        List<string> _stanzas = new List<string>();
        int _curStanza = 1;
        int _curDoc = 1;
        int _characterCaret = 0;
        public static SolidColorBrush AccentBrush = new SolidColorBrush(AccentColorSet.ActiveSet["SystemAccent"]);
        public static SolidColorBrush UnfocusedBrush = new SolidColorBrush(Color.FromRgb(245, 245, 245));
        public static Doc CurrentDoc = new Doc();
        public static MainWindow Current;

        public MainWindow()
        {
            InitializeComponent();
            TopGrid.Background = AccentBrush;
            SaveButton.Background = AccentBrush;
            OpenButton.Background = AccentBrush;
            ConvertTasbehaButton.Background = AccentBrush;
            ScriptButton.Background = AccentBrush;
            KeyBackGrid.Background = AccentBrush;
            InitEnglish();
            _stanzas.Add("");
            DocSelection.Items.Add("Name");
            DocSelection.SelectedIndex = 0;
            Current = this;
        }

        private void TopGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        #region Keyboard
        public static List<string> English = new List<string>()
        {
            null, // 0
            "q", // 1
            "w", // 2
            "e", // 3
            "r", // 4
            "t", // 5
            "y", // 6
            "u", // 7
            "i", // 8
            "o", // 9
            "p", // 10
            "a", // 11
            "s", // 12
            "d", // 13
            "f", // 14
            "g", // 15
            "h", // 16
            "j", // 17
            "k", // 18
            "l", // 19
            ";", // 20
            "z", // 21
            "x", // 22
            "c", // 23
            "v", // 24
            "b", // 25
            "n", // 26
            "m", // 27
            ".", // 28
            null, // 29
            "(", // 30
            ")", // 31
            ",", // 32
            "?", // 33
            null // 34
        };
        public static List<string> EnglishSft = new List<string>()
        {
            null, // 0
            "Q", // 1
            "W", // 2
            "E", // 3
            "R", // 4
            "T", // 5
            "Y", // 6
            "U", // 7
            "I", // 8
            "O", // 9
            "P", // 10
            "A", // 11
            "S", // 12
            "D", // 13
            "F", // 14
            "G", // 15
            "H", // 16
            "J", // 17
            "K", // 18
            "L", // 19
            "\"", // 20
            "Z", // 21
            "X", // 22
            "C", // 23
            "V", // 24
            "B", // 25
            "N", // 26
            "M", // 27
            ".", // 28
            null, // 29
            "(", // 30
            ")", // 31
            ",", // 32
            "!", // 33
            null // 34
        };

        public static List<string> Arabic = new List<string>()
        {
            null, // 0
            "ض", // 1
            "ص", // 2
            "ث", // 3
            "ق", // 4
            "ف", // 5
            "غ", // 6
            "ع", // 7
            "ه", // 8
            "خ", // 9
            "ح", // 10
            "ش", // 11
            "س", // 12
            "ي", // 13
            "ب", // 14
            "ل", // 15
            "ا", // 16
            "ت", // 17
            "ن", // 18
            "م", // 19
            "ك", // 20
            "ئ", // 21
            "ء", // 22
            "ؤ", // 23
            "ر", // 24
            "لا", // 25
            "ى", // 26
            "ة", // 27
            "ز", // 28
            null, // 29
            ")", // 30
            "(", // 31
            "و", // 32
            "ظ", // 33
            null // 34
        };
        public static List<string> ArabicSft = new List<string>()
        {
            null, // 0
            "َ", // 1
            "ً", // 2
            "ُ", // 3
            "ٌ", // 4
            "لإ", // 5
            "إ", // 6
            "‘", // 7
            "÷", // 8
            "×", // 9
            "؛", // 10
            "ِ", // 11
            "ٍ", // 12
            "]", // 13
            "[", // 14
            "لأ", // 15
            "أ", // 16
            "ـ", // 17
            "،", // 18
            "/", // 19
            "\"", // 20
            "~", // 21
            "ْ", // 22
            "}", // 23
            "{", // 24
            "لآ", // 25
            "آ", // 26
            "’", // 27
            ".", // 28
            null, // 29
            "(", // 30
            ")", // 31
            ",", // 32
            "!", // 33
            null // 34
        };

        public static List<string> Coptic = new List<string>()
        {
            null, // 0
            "a", // 1, a
            "b", // 2, b
            "g", // 3, g
            "d", // 4, d
            "e", // 5, eh
            "6", // 6, so
            "z", // 7, z
            "/", // 8, ee
            "y", // 9, th
            "i", // 10, i
            "k", // 11, k
            "l", // 12, l
            "m", // 13, m
            "n", // 14, n
            "x", // 15, ks
            "o", // 16, o
            "p", // 17, p
            "r", // 18, r
            "c", // 19, s
            "t", // 20, t
            "u", // 21, u
            "v", // 22, ph
            ",", // 23, kh
            "'", // 24, ps
            "w", // 25, oh
            "s", // 26, sh
            "f", // 27, f
            "q", // 28, x
            "h", // 29, h
            "j", // 30, j
            "[", // 31, q
            "]", // 32, tee
            "~", // 33, jenkim
            ";" // 34
        };
        public static List<string> CopticSft = new List<string>()
        {
            null, // 0
            "A", // 1, a
            "B", // 2, b
            "G", // 3, g
            "D", // 4, d
            "E", // 5, eh
            "^", // 6, so
            "Z", // 7, z
            "?", // 8, ee
            "Y", // 9, th
            "I", // 10, i
            "K", // 11, k
            "L", // 12, l
            "M", // 13, m
            "N", // 14, n
            "X", // 15, ks
            "O", // 16, o
            "P", // 17, p
            "R", // 18, r
            "C", // 19, s
            "T", // 20, t
            "U", // 21, u
            "V", // 22, ph
            "<", // 23, kh
            "\"", // 24, ps
            "W", // 25, oh
            "S", // 26, sh
            "F", // 27, f
            "Q", // 28, x
            "H", // 29, h
            "J", // 30, j
            "{", // 31, q
            "}", // 32, tee
            "~", // 33, jenkim
            ":" // 34
        };
        #endregion

        #region Language
        public readonly FontFamily Segoe = new FontFamily("Segoe UI");
        public readonly FontFamily Copt = new FontFamily("Coptic1");

        /// <summary>
        /// Initializes the specified key
        /// </summary>
        /// <param name="btn">The key to initialize</param>
        /// <param name="keytype">Language to load: eng / copt / arabic + ":sft"</param>
        /// <param name="index"></param>
        private void InitKey(Button btn, string keytype, int index)
        {
            switch (keytype)
            {
                case "eng":
                    btn.FontFamily = Segoe;
                    if (English[index] != null)
                    {
                        btn.Visibility = Visibility.Visible;
                        btn.Content = English[index];
                    }
                    else
                    {
                        btn.Visibility = Visibility.Collapsed;
                    }
                    return;

                case "eng:sft":
                    btn.FontFamily = Segoe;
                    if (EnglishSft[index] != null)
                    {
                        btn.Visibility = Visibility.Visible;
                        btn.Content = EnglishSft[index];
                    }
                    else
                    {
                        btn.Visibility = Visibility.Collapsed;
                    }
                    return;

                case "copt":
                    btn.FontFamily = Copt;
                    if (Coptic[index] != null)
                    {
                        btn.Visibility = Visibility.Visible;
                        btn.Content = Coptic[index];
                    }
                    else
                    {
                        btn.Visibility = Visibility.Collapsed;
                    }
                    return;

                case "copt:sft":
                    btn.FontFamily = Copt;
                    if (CopticSft[index] != null)
                    {
                        btn.Visibility = Visibility.Visible;
                        btn.Content = CopticSft[index];
                    }
                    else
                    {
                        btn.Visibility = Visibility.Collapsed;
                    }
                    return;

                case "arabic":
                    btn.FontFamily = Segoe;
                    if (Arabic[index] != null)
                    {
                        btn.Visibility = Visibility.Visible;
                        btn.Content = Arabic[index];
                    }
                    else
                    {
                        btn.Visibility = Visibility.Collapsed;
                    }
                    return;

                case "arabic:sft":
                    btn.FontFamily = Segoe;
                    if (ArabicSft[index] != null)
                    {
                        btn.Visibility = Visibility.Visible;
                        btn.Content = ArabicSft[index];
                    }
                    else
                    {
                        btn.Visibility = Visibility.Collapsed;
                    }
                    return;
            }
        }

        private void InitEnglish()
        {
            InputBox.FontFamily = Segoe;

            for (int i = 1; i <= 34; i++)
            {
                var key = KeyboardGrid.FindName("Key" + i.ToString());
                if (key != null)
                {
                    if (key.GetType() == typeof(Button))
                        InitKey((Button)key, "eng", i);
                }
            }
            return;
        }
        private void InitEnglishSft()
        {
            InputBox.FontFamily = Segoe;
            for (int i = 1; i <= 34; i++)
            {
                var key = KeyboardGrid.FindName("Key" + i.ToString());
                if (key != null)
                {
                    if (key.GetType() == typeof(Button))
                        InitKey((Button)key, "eng:sft", i);
                }
            }
            return;
        }

        private void InitCoptic()
        {
            InputBox.FontFamily = Copt;
            for (int i = 1; i <= 34; i++)
            {
                var key = KeyboardGrid.FindName("Key" + i.ToString());
                if (key != null)
                {
                    if (key.GetType() == typeof(Button))
                        InitKey((Button)key, "copt", i);
                }
            }
            return;
        }
        private void InitCopticSft()
        {
            InputBox.FontFamily = Copt;
            for (int i = 1; i <= 34; i++)
            {
                var key = KeyboardGrid.FindName("Key" + i.ToString());
                if (key != null)
                {
                    if (key.GetType() == typeof(Button))
                        InitKey((Button)key, "copt:sft", i);
                }
            }
            return;
        }

        private void InitArabic()
        {
            InputBox.FontFamily = Segoe;

            for (int i = 1; i <= 34; i++)
            {
                var key = KeyboardGrid.FindName("Key" + i.ToString());
                if (key != null)
                {
                    if (key.GetType() == typeof(Button))
                        InitKey((Button)key, "arabic", i);
                }
            }
            return;
        }
        private void InitArabicSft()
        {
            InputBox.FontFamily = Segoe;

            for (int i = 1; i <= 34; i++)
            {
                var key = KeyboardGrid.FindName("Key" + i.ToString());
                if (key != null)
                {
                    if (key.GetType() == typeof(Button))
                        InitKey((Button)key, "arabic:sft", i);
                }
            }
            return;
        }
        #endregion

        private void LanguageOption_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsInitialized)
            {
                if (LanguageOption.SelectedIndex > -1)
                {
                    KeyShift.Background = new SolidColorBrush(Color.FromArgb(32, 0, 0, 0));
                    _shift = false;

                    switch (LanguageOption.SelectedIndex)
                    {
                        #region English
                        case 0:
                            InitEnglish();
                            break;
                        #endregion

                        #region Coptic
                        case 1:
                            InitCoptic();
                            break;
                        #endregion

                        #region Arabic
                        case 2:
                            InitArabic();
                            break;
                        #endregion
                    }
                }
            }
        }

        #region Key Clicks
        private void Key_Click(object sender, RoutedEventArgs e)
        {
            _characterCaret = InputBox.CaretIndex;
            InputBox.Text = InputBox.Text.Insert(InputBox.CaretIndex, ((Button)sender).Content.ToString());
            InputBox.CaretIndex = _characterCaret += 1;
        }

        private void KeySpace_Click(object sender, RoutedEventArgs e)
        {
            InputBox.Text += " ";
        }

        private void KeyEnter_Click(object sender, RoutedEventArgs e)
        {
            InputBox.Text += "\n";
        }

        private void KeyShift_Click(object sender, RoutedEventArgs e)
        {
            if (_shift == true)
            {
                KeyShift.Background = new SolidColorBrush(Color.FromArgb(33, 0, 0, 0));
                _shift = false;

                if (LanguageOption.SelectedIndex > -1)
                {
                    switch (LanguageOption.SelectedIndex)
                    {
                        #region English
                        case 0:
                            InitEnglish();
                            break;
                        #endregion

                        #region Coptic
                        case 1:
                            InitCoptic();
                            break;
                        #endregion

                        #region Arabic
                        case 2:
                            InitArabic();
                            break;
                            #endregion
                    }
                }
            }
            else
            {
                KeyShift.Background = new SolidColorBrush(Color.FromArgb(14, 0, 0, 0));
                _shift = true;

                if (LanguageOption.SelectedIndex > -1)
                {
                    switch (LanguageOption.SelectedIndex)
                    {
                        #region English
                        case 0:
                            InitEnglishSft();
                            break;
                        #endregion

                        #region Coptic
                        case 1:
                            InitCopticSft();
                            break;
                        #endregion

                        #region Arabic
                        case 2:
                            InitArabicSft();
                            break;
                            #endregion
                    }
                }
            }
        }
        #endregion

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialogX = new SaveFileDialog
            {
                Filter = "Doc Set (*.zip)|*.zip|Single Doc (*.xml)|*.xml|Coptic Font Text (*.txt)|*.txt",
                DefaultExt = "zip"
            };

            if (_stanzas.Count < 2)
                _stanzas[0] = InputBox.Text;

            if ((bool)dialogX.ShowDialog(this))
            {
                switch (System.IO.Path.GetExtension(dialogX.FileName))
                {
                    case ".xml":
                        var lang = (CoptLib.Language)LanguageOption.SelectedIndex;
                        CoptLib.CopticInterpreter.SaveDocXml(dialogX.FileName, _stanzas, lang, System.IO.Path.GetFileNameWithoutExtension(dialogX.FileName));
                        return;

                    case ".zip":
                        string setUuid = Guid.NewGuid().ToString();

                        foreach (Doc doc in CoptLib.CopticInterpreter.AllDocs.Values)
                        {
                            // TODO: Save multiple docs
                            
                        }

                        CoptLib.CopticInterpreter.SaveSet(dialogX.FileName, System.IO.Path.GetFileNameWithoutExtension(dialogX.FileName), setUuid, CoptLib.CopticInterpreter.AllDocs.Keys.AsEnumerable());
                        return;

                    case ".txt":
                        string txtContents = "";
                        var txtDoc = CoptLib.CopticInterpreter.AllDocs.First().Value;
                        foreach (Stanza stanza in txtDoc.Translations)
                        {
                            txtContents += stanza.Text;
                            txtContents += "\r\n";
                        }
                        System.IO.File.WriteAllText(dialogX.FileName, txtContents);
                        return;
                }
                

                return;

                /*List<string> content = new List<string>();
                if (LanguageOption.SelectedIndex == 0)
                {
                    content = Stanzas;
                    content.RemoveAt(0);
                    CoptLib.CopticInterpreter.SaveDocXML(dialogX.FileName, content, false, NameBox.Text);
                }
                else if (LanguageOption.SelectedIndex == 1)
                {
                    foreach (string s in Stanzas)
                    {
                        content.Add(CoptLib.CopticInterpreter.ConvertToString(s));
                    }
                    CoptLib.CopticInterpreter.SaveDocXML(dialogX.FileName, content, true, NameBox.Text);
                }*/
            }
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialogX = new OpenFileDialog
            {
                Filter = "Compressed Zip Folder (*zip)|*.zip|Extensible Markup Language (*.xml)|*.xml",
                DefaultExt = "zip"
            };

            if (_stanzas.Count < 2)
                _stanzas[0] = InputBox.Text;

            if ((bool)dialogX.ShowDialog(this))
            {
                switch (System.IO.Path.GetExtension(dialogX.FileName)) {

                    #region ZIP
                    case ".zip":
                        var set = CoptLib.CopticInterpreter.ReadSet(dialogX.FileName, System.IO.Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            "Coptic Chanter", "Doc Creator", "temp")
                        );
                        ResetControls();
                        _stanzas.Add("");
                        DocSelection.Items.Clear();

                        foreach (Doc docX in set.IncludedDocs)
                        {
                            CoptLib.CopticInterpreter.AllDocs.Add(docX.Uuid, set.IncludedDocs.Find((Doc xml) => { if (xml.Uuid == docX.Uuid) return true; else { return false; } }));
                            DocSelection.Items.Add(docX.Name);
                        }

                        DocSelection.SelectedIndex = 0;
                        Doc doc = set.IncludedDocs[0];
                        if (doc.Language != CoptLib.Language.Coptic)
                        {
                            foreach (Stanza stanza in doc.Translations)
                            {
                                _stanzas.Add(stanza.Text);
                            }
                        }
                        else
                        {
                            foreach (Stanza stanza in doc.Translations)
                            {
                                _stanzas.Add(CoptLib.CopticInterpreter.ConvertFromString(stanza.Text));
                            }
                        }

                        LanguageOption.SelectedIndex = (int)doc.Language;
                        NameBox.Text = doc.Name;
                        _curStanza = 1;
                        InputBox.Text = _stanzas[_curStanza];
                        CurrentDoc = doc;
                        return;
                    #endregion

                    #region XML
                    case ".xml":
                        var docXml = CoptLib.CopticInterpreter.ReadDocXml(dialogX.FileName);
                        ResetControls();
                        _stanzas.Add("");

                        if (docXml.Language != CoptLib.Language.Coptic)
                        {
                            foreach (Stanza stanza in docXml.Translations)
                            {
                                _stanzas.Add(stanza.Text);
                            }
                        }
                        else
                        {
                            foreach (Stanza stanza in docXml.Translations)
                            {
                                _stanzas.Add(CoptLib.CopticInterpreter.ConvertFromString(stanza.Text));
                            }
                        }

                        LanguageOption.SelectedIndex = (int)docXml.Language;
                        NameBox.Text = docXml.Name;
                        _curStanza = 1;
                        InputBox.Text = _stanzas[_curStanza];
                        return;
                        #endregion

                }
            }

            #region old
            /*VistaOpenFileDialog dialogX = new VistaOpenFileDialog
            {
                Filter = "Extensible Markup Language (*.xml)|*.xml",
                DefaultExt = "xml"
            };

            if (Stanzas.Count < 2)
                Stanzas[0] = InputBox.Text;

            // As of .Net 3.5 SP1, WPF's Microsoft.Win32.SaveFileDialog class still uses the old style
            if ((bool)dialogX.ShowDialog(this))
            {
                var doc = CoptLib.CopticInterpreter.ReadDocXML(dialogX.FileName);
                ResetControls();
                Stanzas.Add("");

                if (!doc.Coptic)
                {
                    foreach (string stanza in doc.Content)
                    {
                        Stanzas.Add(stanza);
                    }
                    LanguageOption.SelectedIndex = 0;
                }
                else
                {
                    foreach (string stanza in doc.Content)
                    {
                        Stanzas.Add(CoptLib.CopticInterpreter.ConvertFromString(stanza));
                    }
                    LanguageOption.SelectedIndex = 1;
                }

                NameBox.Text = doc.Name;
                CurStanza = 1;
                InputBox.Text = Stanzas[CurStanza];
            }*/
            #endregion
        }

        #region Stanza Controls
        private void StanzaDecrement_Click(object sender, RoutedEventArgs e)
        {
            if (_curStanza > 1)
            {
                _stanzas[_curStanza] = InputBox.Text;

                _curStanza--;
                StanzaLabel.Content = _curStanza;
                InputBox.Text = _stanzas[_curStanza];
            }
        }

        private void StanzaIncrement_Click(object sender, RoutedEventArgs e)
        {
            if (_stanzas.Count - 1 > _curStanza)
            {
                try
                {
                    _stanzas[_curStanza] = InputBox.Text;
                }
                catch
                {
                    _stanzas.Insert(_curStanza, InputBox.Text);
                }

                _curStanza++;
                StanzaLabel.Content = _curStanza;
                InputBox.Text = _stanzas[_curStanza];
            }
        }

        private void StanzaCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _stanzas[_curStanza] = InputBox.Text;
            }
            catch
            {
                _stanzas.Insert(_curStanza, InputBox.Text);
            }

            _stanzas.Insert(_stanzas.Count, "");
            _curStanza = _stanzas.Count - 1;
            StanzaLabel.Content = _curStanza;
            InputBox.Text = "";
        }

        private void StanzaDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _stanzas[_curStanza] = InputBox.Text;
            }
            catch
            {
                _stanzas.Insert(_curStanza, InputBox.Text);
            }
            _stanzas.RemoveAt(_curStanza);

            try
            {
                InputBox.Text = _stanzas[_curStanza];
            }
            catch
            {
                _curStanza--;
                StanzaLabel.Content = _curStanza;
                InputBox.Text = _stanzas[_curStanza];
            }
        }
        #endregion

        #region Doc Controls
        private void DocDecrement_Click(object sender, RoutedEventArgs e)
        {
            if (_curDoc > 1)
            {
                _stanzas[_curStanza] = InputBox.Text;
                var sxml = new List<Stanza>();
                foreach (string content in _stanzas)
                {
                    sxml.Add(new Stanza()
                    {
                        Text = content
                    });
                }
                var doc = new Doc()
                {
                    Name = NameBox.Text,
                    Translations = sxml,
                };
                CurrentDoc = doc;
                CoptLib.CopticInterpreter.AllDocs.Values.ToList()[_curDoc] = doc;

                _curDoc--;
                _stanzas.Clear();
                foreach (Stanza xml in CurrentDoc.Translations)
                {
                    _stanzas.Add(xml.Text);
                }
                StanzaLabel.Content = "1";
                InputBox.Text = _stanzas[1];
            }
        }

        private void DocIncrement_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DocDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DocCreate_Click(object sender, RoutedEventArgs e)
        {
            var newDoc = new Doc()
            {
                Name = "Name",
                Uuid = Guid.NewGuid().ToString(),
            };
            CoptLib.CopticInterpreter.AllDocs.Add(newDoc.Uuid, newDoc);
            NameBox.Text = newDoc.Name;
            InputBox.Text = "";
            _curDoc++;
            _curStanza = 1;
            DocSelection.SelectedIndex = _curDoc - 1;
            DocSelection.Items.Add(newDoc.Name);
            _stanzas.Clear();
        }

        private void SaveDocToVariable(Doc doc)
        {
            string docUuid = Guid.NewGuid().ToString();
            if (LanguageOption.SelectedIndex == 0)
            {
                // Convert the list of content to a serializable list of StanzaXML
                List<Stanza> stanzaXmLs = new List<Stanza>();
                foreach (string stanza in _stanzas)
                {
                    stanzaXmLs.Add(new Stanza(stanza));
                }

                Doc saveX = new Doc()
                {
                    Language = CoptLib.Language.English,
                    Uuid = docUuid,
                    Translations = stanzaXmLs,
                    Name = NameBox.Text,
                    NextScript = CurrentDoc.NextScript
                };
                // Checks if first stanza is empty
                if (saveX.Translations[0].Text == "")
                {
                    saveX.Translations.RemoveAt(0);
                }
                CoptLib.CopticInterpreter.AllDocs.Add(docUuid, saveX);
            }
            else if (LanguageOption.SelectedIndex == 1)
            {
                IList<string> contentCopt = new List<string>();
                List<Stanza> stanzaXmLs = new List<Stanza>();
                // Parse the Coptic-Font text to Coptic-Latin
                foreach (string s in _stanzas)
                {
                    contentCopt.Add(CoptLib.CopticInterpreter.ConvertToString(s));
                }
                Doc saveX = new Doc
                {
                    Language = CoptLib.Language.English,
                    Uuid = docUuid,
                    Translations = stanzaXmLs,
                    Name = NameBox.Text,
                    NextScript = CurrentDoc.NextScript
                };
                // Convert the list of content to a serializable list of StanzaXML
                foreach (string s in contentCopt)
                {
                    // Replaces c# escaped new lines with XML new lines
                    saveX.Translations.Add(new Stanza(s.Replace("\r\n", "&#xD;")));
                }

                // Checks if first stanza is empty
                if (saveX.Translations[0].Text == "")
                {
                    saveX.Translations.RemoveAt(0);
                }
                CoptLib.CopticInterpreter.AllDocs.Add(docUuid, saveX);
            }
        }
        #endregion

        private void ResetControls()
        {
            _curStanza = 1;
            StanzaLabel.Content = "1";
            _stanzas.Clear();
            InputBox.Text = "";
        }

        private void ConvertTasbehaButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Dialogs.ConvertTextDialog();
            dialog.ShowDialog();
            InputBox.Text += dialog.ConvertedText;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            TopGrid.Background = UnfocusedBrush;
            SaveButton.Background = UnfocusedBrush;
            OpenButton.Background = UnfocusedBrush;
            ConvertTasbehaButton.Background = UnfocusedBrush;
            ScriptButton.Background = UnfocusedBrush;
            KeyBackGrid.Background = UnfocusedBrush;
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            TopGrid.Background = AccentBrush;
            SaveButton.Background = AccentBrush;
            OpenButton.Background = AccentBrush;
            ConvertTasbehaButton.Background = AccentBrush;
            ScriptButton.Background = AccentBrush;
            KeyBackGrid.Background = AccentBrush;
        }

        private void ScriptButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Dialogs.ScriptingDialog(CurrentDoc.NextScript.ToString());
            dialog.ShowDialog();

            try
            {
                CurrentDoc.NextScript = dialog.Script;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void InputBox_LostFocus(object sender, RoutedEventArgs e)
        {
            _characterCaret = InputBox.CaretIndex;
        }
    }

    class AccentColorSet
    {
        public static AccentColorSet[] AllSets {
            get {
                if (_allSets == null)
                {
                    UInt32 colorSetCount = UxTheme.GetImmersiveColorSetCount();

                    List<AccentColorSet> colorSets = new List<AccentColorSet>();
                    for (UInt32 i = 0; i < colorSetCount; i++)
                    {
                        colorSets.Add(new AccentColorSet(i, false));
                    }

                    AllSets = colorSets.ToArray();
                }

                return _allSets;
            }
            private set {
                _allSets = value;
            }
        }

        public static AccentColorSet ActiveSet {
            get {
                UInt32 activeSet = UxTheme.GetImmersiveUserColorSetPreference(false, false);
                ActiveSet = AllSets[Math.Min(activeSet, AllSets.Length - 1)];
                return _activeSet;
            }
            private set {
                if (_activeSet != null) _activeSet.Active = false;

                value.Active = true;
                _activeSet = value;
            }
        }

        public Boolean Active { get; private set; }

        public Color this[string colorName] {
            get {
                IntPtr name = IntPtr.Zero;
                UInt32 colorType;

                try
                {
                    name = Marshal.StringToHGlobalUni("Immersive" + colorName);
                    colorType = UxTheme.GetImmersiveColorTypeFromName(name);
                    if (colorType == 0xFFFFFFFF) throw new InvalidOperationException();
                }
                finally
                {
                    if (name != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(name);
                        name = IntPtr.Zero;
                    }
                }

                return this[colorType];
            }
        }

        public Color this[UInt32 colorType] {
            get {
                UInt32 nativeColor = UxTheme.GetImmersiveColorFromColorSetEx(this._colorSet, colorType, false, 0);
                //if (nativeColor == 0)
                //    throw new InvalidOperationException();
                return Color.FromArgb(
                    (Byte)((0xFF000000 & nativeColor) >> 24),
                    (Byte)((0x000000FF & nativeColor) >> 0),
                    (Byte)((0x0000FF00 & nativeColor) >> 8),
                    (Byte)((0x00FF0000 & nativeColor) >> 16)
                    );
            }
        }

        AccentColorSet(UInt32 colorSet, Boolean active)
        {
            this._colorSet = colorSet;
            this.Active = active;
        }

        static AccentColorSet[] _allSets;
        static AccentColorSet _activeSet;

        UInt32 _colorSet;

        // HACK: GetAllColorNames collects the available color names by brute forcing the OS function.
        //   Since there is currently no known way to retrieve all possible color names,
        //   the method below just tries all indices from 0 to 0xFFF ignoring errors.
        public List<string> GetAllColorNames()
        {
            List<string> allColorNames = new List<string>();
            for (UInt32 i = 0; i < 0xFFF; i++)
            {
                IntPtr typeNamePtr = UxTheme.GetImmersiveColorNamedTypeByIndex(i);
                if (typeNamePtr != IntPtr.Zero)
                {
                    IntPtr typeName = (IntPtr)Marshal.PtrToStructure(typeNamePtr, typeof(IntPtr));
                    allColorNames.Add(Marshal.PtrToStringUni(typeName));
                }
            }

            return allColorNames;
        }

        static class UxTheme
        {
            [DllImport("uxtheme.dll", EntryPoint = "#98", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
            public static extern UInt32 GetImmersiveUserColorSetPreference(Boolean forceCheckRegistry, Boolean skipCheckOnFail);

            [DllImport("uxtheme.dll", EntryPoint = "#94", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
            public static extern UInt32 GetImmersiveColorSetCount();

            [DllImport("uxtheme.dll", EntryPoint = "#95", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
            public static extern UInt32 GetImmersiveColorFromColorSetEx(UInt32 immersiveColorSet, UInt32 immersiveColorType,
                Boolean ignoreHighContrast, UInt32 highContrastCacheMode);

            [DllImport("uxtheme.dll", EntryPoint = "#96", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
            public static extern UInt32 GetImmersiveColorTypeFromName(IntPtr name);

            [DllImport("uxtheme.dll", EntryPoint = "#100", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
            public static extern IntPtr GetImmersiveColorNamedTypeByIndex(UInt32 index);
        }
    }
}
