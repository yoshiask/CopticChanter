using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI;
using System.Diagnostics;
using CoptLib;

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

            EFontSizeBox.Text = Common.GetEnglishFontSize().ToString();
            CFontSizeBox.Text = Common.GetCopticFontSize().ToString();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Common.SetEnglishFontSize(Convert.ToInt32(EFontSizeBox.Text));
            Common.SetCopticFontSize(Convert.ToInt32(CFontSizeBox.Text));

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

            // Set color for RemoteStatusDisplay
            RemoteStatusDisplay.Foreground = new SolidColorBrush(Colors.Gray);

            // Check if remote is connected
            /*if (Common.RemoteSocket == null)
            {
                Common.IsConnected = false;
                RemoteStatusDisplay.Foreground = new SolidColorBrush(Colors.Gray);
                RemoteStatusDisplay.Text = "Not Connected";
                ConnectDeviceButton.Content = "Connect to Device";
            }
            else
            {
                Common.IsConnected = true;
                ConnectDeviceButton.Content = "Disconnect";
                RemoteStatusDisplay.Foreground = new SolidColorBrush(Colors.Green);
                RemoteStatusDisplay.Text = "Connected to " + Common.RemoteInfo.Name;
            }*/
        }

        private void ConnectAsHostButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Pages.BluetoothHostConnectPage));
        }

        private void ConnectAsRemoteButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Pages.BluetoothRemoteConnectPage));
        }
    }
}
