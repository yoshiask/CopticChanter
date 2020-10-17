using CoptLib;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

using Language =  CoptLib.Language;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CopticWriter.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StanzaEditorView : Page
    {
        string _content;
        public string StanzaContent {
            get {
                return _content;
            }

            set {
                _content = value;
            }
        }

        #region Stanza Controls
        private void StanzaDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void StanzaCreate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void StanzaDecrement_Click(object sender, RoutedEventArgs e)
        {

        }

        private void StanzaIncrement_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        #region Key Clicks
        int _characterCaret = 0;

        private void Key_Click(object sender, RoutedEventArgs e)
        {
            _characterCaret = InputBox.SelectionStart;
            InputBox.Text = InputBox.Text.Insert(InputBox.SelectionStart, ((Button)sender).Content.ToString());
            InputBox.SelectionStart = _characterCaret += 1;

            // Reset the shift keys
            KeyLeftShift.IsChecked = false;
            KeyRightShift.IsChecked = false;
        }

        private void KeySpace_Click(object sender, RoutedEventArgs e)
        {
            _characterCaret = InputBox.SelectionStart;
            InputBox.Text = InputBox.Text.Insert(InputBox.SelectionStart, " ");
            InputBox.SelectionStart = _characterCaret += 1;
        }

        private void KeyEnter_Click(object sender, RoutedEventArgs e)
        {
            _characterCaret = InputBox.SelectionStart;
            InputBox.Text = InputBox.Text.Insert(InputBox.SelectionStart, "\n");
            InputBox.SelectionStart = _characterCaret += 1;
        }

        private void KeyBackspace_Click(object sender, RoutedEventArgs e)
        {
            _characterCaret = InputBox.SelectionStart;
            if (_characterCaret > 0)
            {
                InputBox.SelectionStart = _characterCaret - 1;
                InputBox.Text = InputBox.Text.Remove(InputBox.SelectionStart);
                InputBox.SelectionStart = _characterCaret;

                if (InputBox.Text == String.Empty)
                {
                    InputBox.SelectionStart = 0;
                }
            }
        }

        private void KeyShift_Checked(object sender, RoutedEventArgs e)
        {
            KeyLeftShift.IsChecked = true;
            KeyRightShift.IsChecked = true;

            if (LanguageOption.SelectedIndex > -1)
            {
                switch ((Language)LanguageOption.SelectedIndex)
                {
                    #region English
                    case CoptLib.Language.English:
                        InitEnglishSft();
                        break;
                    #endregion

                    #region Coptic
                    case CoptLib.Language.Coptic:
                        InitCopticSft();
                        break;
                    #endregion

                    #region Arabic
                    case CoptLib.Language.Arabic:
                        InitArabicSft();
                        break;
                        #endregion
                }
            }
        }

        private void KeyShift_Unchecked(object sender, RoutedEventArgs e)
        {
            KeyLeftShift.IsChecked = false;
            KeyRightShift.IsChecked = false;

            if (LanguageOption.SelectedIndex > -1)
            {
                switch ((Language)LanguageOption.SelectedIndex)
                {
                    #region English
                    case CoptLib.Language.English:
                        InitEnglish();
                        break;
                    #endregion

                    #region Coptic
                    case CoptLib.Language.Coptic:
                        InitCoptic();
                        break;
                    #endregion

                    #region Arabic
                    case CoptLib.Language.Arabic:
                        InitArabic();
                        break;
                        #endregion
                }
            }
        }
        #endregion

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
            "[", // 11
            "]", // 12
            "\\", // 13
            "a", // 14
            "s", // 15
            "d", // 16
            "f", // 17
            "g", // 18
            "h", // 19
            "j", // 20
            "k", // 21
            "l", // 22
            ";", // 23
            "'", // 24
            "z", // 25
            "x", // 26
            "c", // 27
            "v", // 28
            "b", // 29
            "n", // 30
            "m", // 31
            ",", // 32
            ".", // 33
            "/" // 34
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
            "{", // 11
            "}", // 12
            "|", // 13
            "A", // 14
            "S", // 15
            "D", // 16
            "F", // 17
            "G", // 18
            "H", // 19
            "J", // 20
            "K", // 21
            "L", // 22
            ":", // 23
            "\"", // 24
            "Z", // 25
            "X", // 26
            "C", // 27
            "V", // 28
            "B", // 29
            "N", // 30
            "M", // 31
            "<", // 32
            ">", // 33
            "?" // 34
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

        // TODO: Switch this to use the Coptic Unicode once you figure out how to 
        // replace keyboard input with the wanted characters
        public static List<string> Coptic = new List<string>()
        {
            null, // 0
            "\u03B1", // 1, a
            "\u03B2", // 2, b
            "\u03B3", // 3, g
            "\u03B4", // 4, d
            "\u03B5", // 5, eh
            "\u03EC", // 6, so
            "\u03B6", // 7, z
            "\u03B7", // 8, ee
            "\u03B8", // 9, th
            "\u03B9", // 10, i
            "\u03BA", // 11, k
            "\u03BB", // 12, l
            "\u03BC", // 13, m
            "\u03BD", // 14, n
            "\u03BE", // 15, ks
            "\u03BF", // 16, o
            "\u03C0", // 17, p
            "\u03C1", // 18, r
            "\u03C3", // 19, s
            "\u03C4", // 20, t
            "\u03C5", // 21, u
            "\u03C6", // 22, ph
            "\u03C7", // 23, kh
            "\u03C8", // 24, ps
            "\u03C9", // 25, oh
            "\u03E3", // 26, sh
            "\u03E5", // 27, f
            "\u03E7", // 28, x
            "\u03E9", // 29, h
            "\u03EB", // 30, j
            "\u03ED", // 31, q
            "\u03EF", // 32, tee
            "\u0384", // 33, jenkim
            ";" // 34
        };
        public static List<string> CopticSft = new List<string>()
        {
            null, // 0
            "\u0391", // 1, a
            "\u0392", // 2, b
            "\u0393", // 3, g
            "\u0394", // 4, d
            "\u0395", // 5, eh
            "\u03EC", // 6, so
            "\u0396", // 7, z
            "\u0397", // 8, ee
            "\u0398", // 9, th
            "\u0399", // 10, i
            "\u039A", // 11, k
            "\u039B", // 12, l
            "\u039C", // 13, m
            "\u039D", // 14, n
            "\u039E", // 15, ks
            "\u039F", // 16, o
            "\u03A0", // 17, p
            "\u03A1", // 18, r
            "\u03A3", // 19, s
            "\u03A4", // 20, t
            "\u03A5", // 21, u
            "\u03A6", // 22, ph
            "\u03A7", // 23, kh
            "\u03A8", // 24, ps
            "\u03A9", // 25, oh
            "\u03E2", // 26, sh
            "\u03E4", // 27, f
            "\u03E6", // 28, x
            "\u03E8", // 29, h
            "\u03EA", // 30, j
            "\u03EC", // 31, q
            "\u03EE", // 32, tee
            "\u0384", // 33, jenkim
            ":" // 34
        };

        bool _isUpper;
        private void InputBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            //_characterCaret = InputBox.SelectionStart;
            //if (LanguageOption.SelectedIndex == 2)
            //{

            //	// Language is set to Coptic, so intercept English keystrokes
            //	// and replace with Coptic unicode
            //	_isUpper = e.Key == Windows.System.VirtualKey.Shift;

            //	if (e.OriginalKey.ToString().Length <= 1 && char.IsLetterOrDigit(e.OriginalKey.ToString().ToCharArray()[0]))
            //	{
            //		string englishText = e.OriginalKey.ToString();
            //		if (_isUpper)
            //			englishText = englishText.ToUpper();
            //		else
            //			englishText = englishText.ToLower();
            //		string oldText = InputBox.Text;
            //		string newText = CopticInterpreter.ConvertFont(englishText, CopticFont.Coptic1, CopticFont.CopticUnicode);
            //		//InputBox.Text.Insert(CharacterCaret, newText);
            //		InputBox.Text += newText;
            //		InputBox.SelectionStart = ++_characterCaret;
            //		e.Handled = true;
            //	}
            //}
        }

        /// <summary>
        /// Initializes the specified key
        /// </summary>
        /// <param name="btn">The key to initialize</param>
        /// <param name="keytype">Language to load: eng / copt / arabic + ":sft"</param>
        /// <param name="index"></param>
        private void InitKey(Button btn, Language language, bool shift, int index)
        {
            switch (language)
			{
				#region English
				case CoptLib.Language.English:
                    if (shift)
					{
                        if (EnglishSft[index] != null)
                        {
                            btn.Visibility = Visibility.Visible;
                            btn.Content = EnglishSft[index];
                        }
                        else
                        {
                            btn.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
					{
                        if (English[index] != null)
                        {
                            btn.Visibility = Visibility.Visible;
                            btn.Content = English[index];
                        }
                        else
                        {
                            btn.Visibility = Visibility.Collapsed;
                        }
                    }
                    return;
				#endregion

				#region Coptic
				case CoptLib.Language.Coptic:
                    if (shift)
					{
                        if (CopticSft[index] != null)
                        {
                            btn.Visibility = Visibility.Visible;
                            btn.Content = CopticInterpreter.ConvertFont(EnglishSft[index], CopticFont.CsAvvaShenouda, CopticFont.CopticUnicode);
                        }
                        else
                        {
                            btn.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
					{
                        if (Coptic[index] != null)
                        {
                            btn.Visibility = Visibility.Visible;
                            var vals = CopticFont.CopticUnicode.Charmap.Values.ToList();
                            btn.Content = CopticInterpreter.ConvertFont(English[index], CopticFont.CsAvvaShenouda, CopticFont.CopticUnicode);
                        }
                        else
                        {
                            btn.Visibility = Visibility.Collapsed;
                        }
                    }
                    return;
                #endregion

                #region Arabic
                case CoptLib.Language.Arabic:
                    if (!shift)
                    {
                        if (ArabicSft[index] != null)
                        {
                            btn.Visibility = Visibility.Visible;
                            btn.Content = ArabicSft[index];
                        }
                        else
                        {
                            btn.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        if (Arabic[index] != null)
                        {
                            btn.Visibility = Visibility.Visible;
                            btn.Content = Arabic[index];
                        }
                        else
                        {
                            btn.Visibility = Visibility.Collapsed;
                        }
                    }
                    return;
				#endregion
			}
		}

        private void InitEnglish()
        {
            for (int i = 1; i <= 34; i++)
            {
                var key = KeyboardGrid.FindName("Key" + i.ToString());
                if (key != null)
                {
                    if (key.GetType() == typeof(Button))
                        InitKey((Button)key, CoptLib.Language.English, false, i);
                }
            }
            return;
        }
        private void InitEnglishSft()
        {
            for (int i = 1; i <= 34; i++)
            {
                var key = KeyboardGrid.FindName("Key" + i.ToString());
                if (key != null)
                {
                    if (key.GetType() == typeof(Button))
                        InitKey((Button)key, CoptLib.Language.English, true, i);
                }
            }
            return;
        }

        private void InitCoptic()
        {
            for (int i = 1; i <= 34; i++)
            {
                var key = KeyboardGrid.FindName("Key" + i.ToString());
                if (key != null)
                {
                    if (key.GetType() == typeof(Button))
                        InitKey((Button)key, CoptLib.Language.Coptic, false, i);
                }
            }
            return;
        }
        private void InitCopticSft()
        {
            for (int i = 1; i <= 34; i++)
            {
                var key = KeyboardGrid.FindName("Key" + i.ToString());
                if (key != null)
                {
                    if (key.GetType() == typeof(Button))
                        InitKey((Button)key, CoptLib.Language.Coptic, true, i);
                }
            }
            return;
        }

        private void InitArabic()
        {
            for (int i = 1; i <= 34; i++)
            {
                var key = KeyboardGrid.FindName("Key" + i.ToString());
                if (key != null)
                {
                    if (key.GetType() == typeof(Button))
                        InitKey((Button)key, CoptLib.Language.Arabic, false, i);
                }
            }
            return;
        }
        private void InitArabicSft()
        {
            for (int i = 1; i <= 34; i++)
            {
                var key = KeyboardGrid.FindName("Key" + i.ToString());
                if (key != null)
                {
                    if (key.GetType() == typeof(Button))
                        InitKey((Button)key, CoptLib.Language.Arabic, true, i);
                }
            }
            return;
        }
        #endregion

        private void LanguageOption_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
                return;
            try
            {
                if (LanguageOption.SelectedIndex > -1)
                {
                    var language = (Language)LanguageOption.SelectedIndex;
                    switch (language)
                    {
                        #region English
                        case CoptLib.Language.English:
                            InitEnglish();
                            break;
                        #endregion

                        #region Coptic
                        case CoptLib.Language.Coptic:
                            InitCoptic();
                            break;
                        #endregion

                        #region Arabic
                        case CoptLib.Language.Arabic:
                            InitArabic();
                            break;
                            #endregion
                    }
                }
            }
            catch
            {

            }
        }

        /*private void InputBox_LostFocus(object sender, RoutedEventArgs e)
        {

        }*/

        /*private void InputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // If the input language is Coptic, map the input to its Coptic unicode equivalent
            if (LanguageOption.SelectedIndex == 1)
            {
                
            }
        }*/

        public StanzaEditorView()
        {
            this.InitializeComponent();
            Loaded += StanzaEditorView_Loaded;
#if x
            // Handle touch event to manage focus.
            CoreWindow.GetForCurrentThread().PointerPressed += Page_PointerPressed;
#endif
        }

        private void StanzaEditorView_Loaded(object sender, RoutedEventArgs e)
        {
            InitEnglish();
        }

#if x
        //Used to check if the pointer is within the bounds of the edit control.
        //If it is, focus should go to the edit control.  If it is outside the bounds
        //Focus should not be in the edit control.
        private void Page_PointerPressed(CoreWindow sender, PointerEventArgs args)
        {
            Rect _boundingbox = EditControl.GetLayout();
            if (_boundingbox.Contains(args.CurrentPoint.Position))
            {
                _textEditContext.InternalSetFocus();
                EditControl.Focus(FocusState.Programmatic);
            }
            else
            {
                _textEditContext.InternalRemoveFocus();
            }
        }
#endif

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
#if x
            // Destroy the pointer pressed handlers when navigating to another page.
            CoreWindow.GetForCurrentThread().PointerPressed -= Page_PointerPressed;
            _textEditContext.ShutDown();
            base.OnNavigatingFrom(e);
#endif
        }

		private void InputBox_TextChanged(object sender, TextChangedEventArgs e)
		{
            int caret = InputBox.SelectionStart;
            switch ((Language)LanguageOption.SelectedIndex)
            {
                case CoptLib.Language.Coptic:
                    InputBox.Text = CopticInterpreter.ConvertFont(InputBox.Text, CopticFont.CsAvvaShenouda, CopticFont.CopticUnicode);
                    break;
            }
            InputBox.SelectionStart = caret;
        }
	}
}
