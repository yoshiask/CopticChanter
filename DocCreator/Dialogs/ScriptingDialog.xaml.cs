using CoptLib;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DocCreator.Dialogs
{
    /// <summary>
    /// Interaction logic for ScriptingDialog.xaml
    /// </summary>
    public partial class ScriptingDialog : Window
    {
        private Window _consoleDialog;
        private ListBox _intellisenseBox;
        private int _caret;
        public string Script;

        public ScriptingDialog(string script)
        {
            InitializeComponent();
            TopGrid.Background = MainWindow.AccentBrush;
            SaveButton.Background = MainWindow.AccentBrush;
            ClearButton.Background = MainWindow.AccentBrush;
            CancelButton.Background = MainWindow.AccentBrush;
            RunScriptButton.Background = MainWindow.AccentBrush;

            InputBox.TextArea.TextEntered += InputBox_TextEntered;

            InputBox.FontSize = 14;
            InputBox.ShowLineNumbers = true;
            InputBox.Text = script;
            InputBox.Focus();
        }

        #region Code Suggestions
        private void InputBox_TextEntered(object sender, TextCompositionEventArgs e)
        {
            _intellisenseBox = new ListBox();

            #region Get prestring "\"
            // first find the caret position
            int caretPosition = InputBox.SelectionStart;
            // now find all text that occurs before the caret
            string textUpToCaret = InputBox.Text.Substring(0, caretPosition);
            // now we can look for the last space character in the
            // textUpToCaret to find the word
            int lastSpaceIndex = textUpToCaret.LastIndexOf("\"");
            if (lastSpaceIndex < 0)
                lastSpaceIndex = 0;
            string lettersBeforeCaret = textUpToCaret.Substring(lastSpaceIndex);
            #endregion

            if (lettersBeforeCaret.StartsWith("\"date:"))
            {
                _intellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "M/D/YYYY",
                    Focusable = false
                });
                _intellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "today",
                    Focusable = false
                });
                _intellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "Spring Equinox",
                    Focusable = false
                });
                _intellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "Palm Sunday",
                    Focusable = false
                });
                _intellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "Pascha",
                    Focusable = false
                });
                _intellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "Holy Week",
                    Focusable = false
                });
                _intellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "Great Lent",
                    Focusable = false
                });
                _intellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "Nativity",
                    Focusable = false
                });
                _intellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "NativitySunday",
                    Focusable = false
                });

                ShowIntellisense();
            }
            if (lettersBeforeCaret.StartsWith("\"bool:"))
            {
                _intellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "true",
                    Focusable = false
                });
                _intellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "false",
                    Focusable = false
                });

                ShowIntellisense();
            }

            #region Get prestring " "
            // first find the caret position
            caretPosition = InputBox.SelectionStart;
            // now find all text that occurs before the caret
            textUpToCaret = InputBox.Text.Substring(0, caretPosition);
            // now we can look for the last space character in the
            // textUpToCaret to find the word
            lastSpaceIndex = textUpToCaret.LastIndexOf(" ");
            if (lastSpaceIndex < 0)
                lastSpaceIndex = 0;
            lettersBeforeCaret = textUpToCaret.Substring(lastSpaceIndex);
            #endregion

            if (lettersBeforeCaret == " LeftHand=\"" || lettersBeforeCaret == " RightHand=\"")
            {
                _intellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "bool",
                    Focusable = false
                });
                _intellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "date",
                    Focusable = false
                });
                _intellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "int",
                    Focusable = false
                });

                ShowIntellisense();
            }
            else if (lettersBeforeCaret == " Comparator=\"")
            {
                _intellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "lth",
                    Focusable = false
                });
                _intellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "lth=",
                    Focusable = false
                });
                _intellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "gth",
                    Focusable = false
                });
                _intellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "gth=",
                    Focusable = false
                });
                _intellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "==",
                    Focusable = false
                });
                _intellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "!=",
                    Focusable = false
                });

                ShowIntellisense();
            }

            if (e.Text == "<")
            {
                _intellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "If",
                    Focusable = false
                });
                _intellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "Return",
                    Focusable = false
                });

                ShowIntellisense();
            }
            else if (e.Text == "L")
            {
                InputBox.TextArea.PerformTextInput("eftHand=\"");
            }
            else if (e.Text == "C")
            {
                InputBox.TextArea.PerformTextInput("omparator=\"");
            }
            else if (e.Text == "R")
            {
                InputBox.TextArea.PerformTextInput("ightHand=\"");
            }
            else if (e.Text == ">")
            {
                #region Get prestring " "
                // first find the caret position
                caretPosition = InputBox.SelectionStart;
                // now find all text that occurs before the caret
                textUpToCaret = InputBox.Text.Substring(0, caretPosition);
                // now we can look for the last space character in the
                // textUpToCaret to find the word
                lastSpaceIndex = textUpToCaret.LastIndexOf("<");
                if (lastSpaceIndex < 0)
                    lastSpaceIndex = 0;
                lettersBeforeCaret = textUpToCaret.Substring(lastSpaceIndex);
                #endregion

                if (lettersBeforeCaret.StartsWith("<If LeftHand=\""))
                {
                    InputBox.TextArea.PerformTextInput("\r\n\t\r\n</If>");
                    InputBox.TextArea.Caret.Offset -= 6;
                }
            }
        }

        private void ShowIntellisense()
        {
            _intellisenseBox.Height = Double.NaN;
            _intellisenseBox.Width = Double.NaN;
            _intellisenseBox.Name = "IntellisenseBox";
            var location = InputBox.TextArea.PointToScreen(new Point(0, 0));
            _caret = InputBox.TextArea.Caret.Offset;
            Rect rect = InputBox.TextArea.Caret.CalculateCaretRectangle();
            MainGrid.Children.Add(_intellisenseBox);
            _intellisenseBox.SelectedIndex = 0;
            _intellisenseBox.HorizontalAlignment = HorizontalAlignment.Left;
            _intellisenseBox.VerticalAlignment = VerticalAlignment.Top;
            _intellisenseBox.Margin = new Thickness(rect.X, rect.Y + TopGrid.ActualHeight + 18, 0, 0);
            _intellisenseBox.Focusable = false;
            InputBox.TextArea.KeyDown += InputBox_KeyDown;
            InputBox.TextArea.KeyUp += TextArea_KeyUp;
            InputBox.TextArea.ActiveInputHandler.Detach();
        }

        bool _shift = false;
        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            string selected;
            switch (e.Key)
            {
                #region Suggestion Navigation
                case Key.Down:
                    if (_intellisenseBox.SelectedIndex < (_intellisenseBox.Items.Count - 1))
                        _intellisenseBox.SelectedIndex += 1;
                    else
                        _intellisenseBox.SelectedIndex = 0;
                    break;

                case Key.Up:
                    if (_intellisenseBox.SelectedIndex > 0)
                        _intellisenseBox.SelectedIndex -= 1;
                    else
                        _intellisenseBox.SelectedIndex = (_intellisenseBox.Items.Count - 1);
                    break;
                #endregion

                #region Insert Suggestion
                case Key.Tab:
                    selected = ((ListBoxItem)_intellisenseBox.SelectedItem).Content.ToString();
                    InputBox.Text = InputBox.Text.Insert(InputBox.SelectionStart, selected);
                    MainGrid.Children.RemoveAt(2);
                    e.Handled = true;
                    InputBox.TextArea.KeyDown -= InputBox_KeyDown;
                    InputBox.TextArea.ActiveInputHandler.Attach();
                    InputBox.TextArea.Caret.Offset = _caret + selected.Length;
                    _intellisenseBox = null;

                    // Extras for neater code
                    if (selected == "bool" || selected == "date" || selected == "int")
                    {
                        InputBox.TextArea.PerformTextInput(":");
                    }
                    else if (selected == "true" || selected == "false")
                    {
                        InputBox.TextArea.PerformTextInput("\" ");
                    }
                    else if (selected == "LeftHand" || selected == "Comparator" || selected == "RightHand")
                    {
                        InputBox.TextArea.PerformTextInput("=\"");
                    }
                    else if (selected == "If")
                    {
                        InputBox.TextArea.PerformTextInput(" LeftHand=\"");
                    }
                    else if (selected == "Return")
                    {
                        InputBox.TextArea.PerformTextInput("></Return>");
                        InputBox.TextArea.Caret.Offset -= 9;
                    }
                    break;
                #endregion

                #region Close Intellisense
                case Key.Escape:
                    MainGrid.Children.RemoveAt(2);
                    e.Handled = true;
                    InputBox.TextArea.KeyDown -= InputBox_KeyDown;
                    InputBox.TextArea.ActiveInputHandler.Attach();
                    InputBox.TextArea.Caret.Offset = _caret;
                    _intellisenseBox = null;
                    break;

                case Key.Enter:
                    MainGrid.Children.RemoveAt(2);
                    e.Handled = true;
                    InputBox.TextArea.KeyDown -= InputBox_KeyDown;
                    InputBox.TextArea.ActiveInputHandler.Attach();
                    InputBox.TextArea.Caret.Offset = _caret;
                    _intellisenseBox = null;
                    InputBox.TextArea.PerformTextInput("\r\n");
                    break;
                #endregion

                #region General Keys
                case Key.LeftShift:
                    _shift = true;
                    break;
                case Key.RightShift:
                    _shift = true;
                    break;

                case Key.Back:
                    InputBox.Text = InputBox.Text.Remove(InputBox.TextArea.Caret.Offset - 1, 1);
                    _caret -= 1;
                    InputBox.TextArea.Caret.Offset = _caret;
                    break;

                case Key.Space:
                    MainGrid.Children.RemoveAt(2);
                    e.Handled = true;
                    InputBox.TextArea.KeyDown -= InputBox_KeyDown;
                    InputBox.TextArea.ActiveInputHandler.Attach();
                    InputBox.TextArea.Caret.Offset = _caret;
                    _intellisenseBox = null;
                    InputBox.TextArea.PerformTextInput(" ");
                    break;

                default:
                    try
                    {
                        selected = e.Key.ToString();
                        if (selected.Length == 1)
                        {
                            if (!_shift)
                                selected = e.Key.ToString().ToLower();
                            InputBox.TextArea.Caret.Offset = _caret;
                            MainGrid.Children.RemoveAt(2);
                            e.Handled = true;
                            InputBox.TextArea.KeyDown -= InputBox_KeyDown;
                            InputBox.TextArea.ActiveInputHandler.Attach();
                            _intellisenseBox = null;
                            InputBox.TextArea.PerformTextInput(selected);
                        }
                        e.Handled = true;
                        break;
                    }
                    catch
                    {
                        break;
                    }
                #endregion
            }
            InputBox.Focus();
        }

        private void TextArea_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.LeftShift:
                    _shift = false;
                    break;
                case Key.RightShift:
                    _shift = false;
                    break;
            }
        }
        #endregion

        private void TopGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            InputBox.Clear();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Script = "";
            Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Script = InputBox.Text;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Script = InputBox.Text;
        }

        private void RunScriptButton_Click(object sender, RoutedEventArgs e)
        {
            var console = new TextBlock()
            {
                Name = "Console",
                Background = new SolidColorBrush(Colors.Transparent),
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily("Segoe UI"),
                FontSize = 14,
                Margin = new Thickness(5, 0, 5, 0)
            };
            console.MouseDown += Console_MouseDown;
            _consoleDialog = new Window()
            {
                Content = console,
                Title = "Script Output",
                Width = 300,
                Height = 75,
                Background = MainWindow.AccentBrush,
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            _consoleDialog.Show();
            //Console.Text = (string) await Scripting.ParseNextDocScriptFunctionAsync(InputBox.Text);
            console.Text = Scripting.RunScript(InputBox.Text, MainWindow.CurrentDoc);
        }

        private void Console_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ((Window)((TextBlock)sender).Parent).DragMove();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            TopGrid.Background = MainWindow.UnfocusedBrush;
            SaveButton.Background = MainWindow.UnfocusedBrush;
            ClearButton.Background = MainWindow.UnfocusedBrush;
            CancelButton.Background = MainWindow.UnfocusedBrush;
            RunScriptButton.Background = MainWindow.UnfocusedBrush;
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            TopGrid.Background = MainWindow.AccentBrush;
            SaveButton.Background = MainWindow.AccentBrush;
            ClearButton.Background = MainWindow.AccentBrush;
            CancelButton.Background = MainWindow.AccentBrush;
            RunScriptButton.Background = MainWindow.AccentBrush;
        }
    }
}
