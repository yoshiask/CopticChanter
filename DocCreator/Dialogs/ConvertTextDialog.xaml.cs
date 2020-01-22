using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace DocCreator.Dialogs
{
    /// <summary>
    /// Interaction logic for ConvertTasbehaDialog.xaml
    /// </summary>
    public partial class ConvertTextDialog : Window
    {
        SolidColorBrush _accentBrush = new SolidColorBrush(AccentColorSet.ActiveSet["SystemAccent"]);
        public string ConvertedText = "";

        public ConvertTextDialog()
        {
            InitializeComponent();
            MainGrid.Background = _accentBrush;
            ConvertButton.Background = _accentBrush;
            InputBox.Focus();

            ConvertFromOption.Items.Clear();
            CoptLib.CopticInterpreter.CopticFonts.Clear();

            foreach (string path in Directory.EnumerateFiles(@"C:\Users\jjask\Documents\Coptic Chanter"))
            {
                if (path.EndsWith(".fnt.xml"))
                {
                    CoptLib.CopticFont.ReadFontXml(path);
                }
                else if (path.EndsWith(".csv"))
                {
                    CoptLib.CopticInterpreter.CopticFonts.Add(CoptLib.CopticFont.GenerateFromCsv(path));
                }
            }
            foreach (CoptLib.CopticFont font in CoptLib.CopticInterpreter.CopticFonts)
            {
                ConvertFromOption.Items.Add(new ComboBoxItem() { Content = font.Name });

                font.SaveFontXml($@"C:\Users\jjask\Documents\Coptic Chanter\{font.Name}.fnt.xml", false);
            }
        }

        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            ConvertedText = CoptLib.CopticInterpreter.ConvertFont(
                InputBox.Text,
                CoptLib.CopticInterpreter.CopticFonts[ConvertFromOption.SelectedIndex],
                CoptLib.CopticFont.CopticUnicode
            );

            /*ConvertedText = CoptLib.CopticInterpreter.ConvertToUnicode(
                InputBox.Text,
                CoptLib.CopticInterpreter.CopticFonts[ConvertFromOption.SelectedIndex]);*/

            Close();
        }

        private void ConvertFromOption_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ConvertFromOption.SelectedIndex >= 0)
            {
                const string strConvertFrom = "Convert from ";
                var font = CoptLib.CopticInterpreter.CopticFonts[ConvertFromOption.SelectedIndex];
                InputBox.FontFamily = new FontFamily(font.FontName);
                Title = strConvertFrom + font.Name;
            }
        }
    }
}
