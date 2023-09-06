using CoptLib;
using NodaTime;
using System;
using System.Diagnostics;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CopticChanter
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();

            FontFamilyBox.Text = Settings.GetFontFamilyName();
            CharacterMapIdBox.Text = Settings.GetCharacterMapId();
            FontSizeBox.Text = Settings.GetFontSize().ToString();

            GregorianDatePicker.Date = DateTime.Now;
            GregorianDatePicker_DateChanged(null, null);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.SetFontFamily(FontFamilyBox.Text);
            Settings.SetCharacterMapId(CharacterMapIdBox.Text);
            Settings.SetFontSize(Convert.ToInt32(FontSizeBox.Text));
            MainPage.Context.SetDate(LocalDateTime.FromDateTime(GregorianDatePicker.Date.LocalDateTime));

            Frame.Navigate(typeof(MainPage));
        }

        private void GregorianDatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
            => UpdateCopticDateDisplay(GregorianDatePicker.Date.Date);

        private void UpdateCopticDateDisplay(DateTime date)
        {
            CopticDateDisplay.Text = date
                .ToCopticDate()
                .Format(new CoptLib.Writing.LanguageInfo(CoptLib.Writing.KnownLanguage.Coptic));
        }
    }
}
