using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI;
using System.Diagnostics;
using CoptLib;

using CLLanguage = CoptLib.Writing.KnownLanguage;
using NodaTime;

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
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.SetFontFamily(FontFamilyBox.Text);
            Settings.SetCharacterMapId(CharacterMapIdBox.Text);
            Settings.SetFontSize(Convert.ToInt32(FontSizeBox.Text));
            DateHelper.NowOverride = LocalDateTime.FromDateTime(GregorianDatePicker.Date.LocalDateTime);

            Frame.Navigate(typeof(MainPage));
        }

        private void GregorianDatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            CopticDateDisplay.Text = GregorianDatePicker.Date.Date.ToCopticDate().ToString();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Check version for Fluent Design
            try
            {
                if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.Xaml.Media.XamlCompositionBrushBase"))
                {
					AcrylicBrush backBrush = new AcrylicBrush
					{
						BackgroundSource = AcrylicBackgroundSource.HostBackdrop,
						TintColor = Color.FromArgb(255, 246, 246, 246),
						FallbackColor = Color.FromArgb(255, 246, 246, 246),
						TintOpacity = 0.6
					};
					MainGrid.Background = backBrush;
                }
                else
                {
                    SolidColorBrush backBrush = new SolidColorBrush(Color.FromArgb(255, 246, 246, 246));
                    MainGrid.Background = backBrush;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            // Calculate today's date on Coptic calendar
            CopticDateDisplay.Text = GregorianDatePicker.Date.Date.ToCopticDate().ToString();
        }
    }
}
