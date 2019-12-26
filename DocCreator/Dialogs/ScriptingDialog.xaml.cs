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
        private Window ConsoleDialog;
        private ListBox IntellisenseBox;
        private int Caret;
        public string Script;

        public ScriptingDialog(string script)
        {
            InitializeComponent();
            TopGrid.Background = MainWindow.accentBrush;
            SaveButton.Background = MainWindow.accentBrush;
            ClearButton.Background = MainWindow.accentBrush;
            CancelButton.Background = MainWindow.accentBrush;
            RunScriptButton.Background = MainWindow.accentBrush;

            InputBox.TextArea.TextEntered += InputBox_TextEntered;

            InputBox.FontSize = 14;
            InputBox.ShowLineNumbers = true;
            InputBox.Text = script;
            InputBox.Focus();
        }

        #region Code Suggestions
        private void InputBox_TextEntered(object sender, TextCompositionEventArgs e)
        {
            IntellisenseBox = new ListBox();

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
                IntellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "M/D/YYYY",
                    Focusable = false
                });
                IntellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "today",
                    Focusable = false
                });
                IntellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "Spring Equinox",
                    Focusable = false
                });
                IntellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "Palm Sunday",
                    Focusable = false
                });
                IntellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "Pascha",
                    Focusable = false
                });
                IntellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "Holy Week",
                    Focusable = false
                });
                IntellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "Great Lent",
                    Focusable = false
                });
                IntellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "Nativity",
                    Focusable = false
                });
                IntellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "NativitySunday",
                    Focusable = false
                });

                ShowIntellisense();
            }
            if (lettersBeforeCaret.StartsWith("\"bool:"))
            {
                IntellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "true",
                    Focusable = false
                });
                IntellisenseBox.Items.Add(new ListBoxItem()
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
                IntellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "bool",
                    Focusable = false
                });
                IntellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "date",
                    Focusable = false
                });
                IntellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "int",
                    Focusable = false
                });

                ShowIntellisense();
            }
            else if (lettersBeforeCaret == " Comparator=\"")
            {
                IntellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "lth",
                    Focusable = false
                });
                IntellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "lth=",
                    Focusable = false
                });
                IntellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "gth",
                    Focusable = false
                });
                IntellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "gth=",
                    Focusable = false
                });
                IntellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "==",
                    Focusable = false
                });
                IntellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "!=",
                    Focusable = false
                });

                ShowIntellisense();
            }

            if (e.Text == "<")
            {
                IntellisenseBox.Items.Add(new ListBoxItem()
                {
                    Content = "If",
                    Focusable = false
                });
                IntellisenseBox.Items.Add(new ListBoxItem()
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
            IntellisenseBox.Height = Double.NaN;
            IntellisenseBox.Width = Double.NaN;
            IntellisenseBox.Name = "IntellisenseBox";
            var location = InputBox.TextArea.PointToScreen(new Point(0, 0));
            Caret = InputBox.TextArea.Caret.Offset;
            Rect rect = InputBox.TextArea.Caret.CalculateCaretRectangle();
            MainGrid.Children.Add(IntellisenseBox);
            IntellisenseBox.SelectedIndex = 0;
            IntellisenseBox.HorizontalAlignment = HorizontalAlignment.Left;
            IntellisenseBox.VerticalAlignment = VerticalAlignment.Top;
            IntellisenseBox.Margin = new Thickness(rect.X, rect.Y + TopGrid.ActualHeight + 18, 0, 0);
            IntellisenseBox.Focusable = false;
            InputBox.TextArea.KeyDown += InputBox_KeyDown;
            InputBox.TextArea.KeyUp += TextArea_KeyUp;
            InputBox.TextArea.ActiveInputHandler.Detach();
        }

        bool Shift = false;
        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            string selected;
            switch (e.Key)
            {
                #region Suggestion Navigation
                case Key.Down:
                    if (IntellisenseBox.SelectedIndex < (IntellisenseBox.Items.Count - 1))
                        IntellisenseBox.SelectedIndex += 1;
                    else
                        IntellisenseBox.SelectedIndex = 0;
                    break;

                case Key.Up:
                    if (IntellisenseBox.SelectedIndex > 0)
                        IntellisenseBox.SelectedIndex -= 1;
                    else
                        IntellisenseBox.SelectedIndex = (IntellisenseBox.Items.Count - 1);
                    break;
                #endregion

                #region Insert Suggestion
                case Key.Tab:
                    selected = ((ListBoxItem)IntellisenseBox.SelectedItem).Content.ToString();
                    InputBox.Text = InputBox.Text.Insert(InputBox.SelectionStart, selected);
                    MainGrid.Children.RemoveAt(2);
                    e.Handled = true;
                    InputBox.TextArea.KeyDown -= InputBox_KeyDown;
                    InputBox.TextArea.ActiveInputHandler.Attach();
                    InputBox.TextArea.Caret.Offset = Caret + selected.Length;
                    IntellisenseBox = null;

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
                    InputBox.TextArea.Caret.Offset = Caret;
                    IntellisenseBox = null;
                    break;

                case Key.Enter:
                    MainGrid.Children.RemoveAt(2);
                    e.Handled = true;
                    InputBox.TextArea.KeyDown -= InputBox_KeyDown;
                    InputBox.TextArea.ActiveInputHandler.Attach();
                    InputBox.TextArea.Caret.Offset = Caret;
                    IntellisenseBox = null;
                    InputBox.TextArea.PerformTextInput("\r\n");
                    break;
                #endregion

                #region General Keys
                case Key.LeftShift:
                    Shift = true;
                    break;
                case Key.RightShift:
                    Shift = true;
                    break;

                case Key.Back:
                    InputBox.Text = InputBox.Text.Remove(InputBox.TextArea.Caret.Offset - 1, 1);
                    Caret -= 1;
                    InputBox.TextArea.Caret.Offset = Caret;
                    break;

                case Key.Space:
                    MainGrid.Children.RemoveAt(2);
                    e.Handled = true;
                    InputBox.TextArea.KeyDown -= InputBox_KeyDown;
                    InputBox.TextArea.ActiveInputHandler.Attach();
                    InputBox.TextArea.Caret.Offset = Caret;
                    IntellisenseBox = null;
                    InputBox.TextArea.PerformTextInput(" ");
                    break;

                default:
                    try
                    {
                        selected = e.Key.ToString();
                        if (selected.Length == 1)
                        {
                            if (!Shift)
                                selected = e.Key.ToString().ToLower();
                            InputBox.TextArea.Caret.Offset = Caret;
                            MainGrid.Children.RemoveAt(2);
                            e.Handled = true;
                            InputBox.TextArea.KeyDown -= InputBox_KeyDown;
                            InputBox.TextArea.ActiveInputHandler.Attach();
                            IntellisenseBox = null;
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
                    Shift = false;
                    break;
                case Key.RightShift:
                    Shift = false;
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
            var Console = new TextBlock()
            {
                Name = "Console",
                Background = new SolidColorBrush(Colors.Transparent),
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily("Segoe UI"),
                FontSize = 14,
                Margin = new Thickness(5, 0, 5, 0)
            };
            Console.MouseDown += Console_MouseDown;
            ConsoleDialog = new Window()
            {
                Content = Console,
                Title = "Script Output",
                Width = 300,
                Height = 75,
                Background = MainWindow.accentBrush,
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            ConsoleDialog.Show();
            //Console.Text = (string) await Scripting.ParseNextDocScriptFunctionAsync(InputBox.Text);
            Console.Text = Scripting.RunScript(InputBox.Text, MainWindow.CurrentDoc);
        }

        private void Console_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ((Window)((TextBlock)sender).Parent).DragMove();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            TopGrid.Background = MainWindow.unfocusedBrush;
            SaveButton.Background = MainWindow.unfocusedBrush;
            ClearButton.Background = MainWindow.unfocusedBrush;
            CancelButton.Background = MainWindow.unfocusedBrush;
            RunScriptButton.Background = MainWindow.unfocusedBrush;
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            TopGrid.Background = MainWindow.accentBrush;
            SaveButton.Background = MainWindow.accentBrush;
            ClearButton.Background = MainWindow.accentBrush;
            CancelButton.Background = MainWindow.accentBrush;
            RunScriptButton.Background = MainWindow.accentBrush;
        }
    }
}
