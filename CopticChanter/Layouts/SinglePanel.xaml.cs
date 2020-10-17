using CopticChanter.Helpers;
using CoptLib.Models;
using System;
using System.Diagnostics;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CopticChanter.Layouts
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SinglePanel : Page
    {
        public SinglePanel()
        {
            InitializeComponent();

            #region Menu
            InitializeHamburgerMenu();
            trnslttrnsfrmMenuTop.X = -_stckpnlMenuWidth;
            trnslttrnsfrmMenuBottom.X = -_stckpnlMenuWidth;
            this.ManipulationMode = ManipulationModes.TranslateX;
            #endregion
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var args = e.Parameter as SinglePanelArgs;
            ApplicationView.GetForCurrentView().FullScreenSystemOverlayMode = FullScreenSystemOverlayMode.Minimal;
            ApplicationView.GetForCurrentView().TryEnterFullScreenMode();

            MainGrid.Background = new SolidColorBrush(args.BackColor);

            // Populate the column
            foreach (Doc doc in Common.Docs)
            {
                foreach (Translation translation in doc.Translations)
                {
                    if (translation.Language == args.Language)
                    {
                        foreach (UIElement element in DocumentUIFactory.CreateBlocksFromTranslation(translation, args.ForeColor))
                            ContentPanel.Children.Add(element);
                    }
                }
            }

            base.OnNavigatedTo(e);
        }

        private int _index = 0;
        private BringIntoViewOptions _scrollOpt = new BringIntoViewOptions() { AnimationDesired = false };
        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            _index--;
            if (_index >= 0)
            {
                ContentView.ScrollToElement(ContentPanel.Children[_index]);
            }
            else
            {
                _index++;
            }
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _index++;
                ContentView.ScrollToElement(ContentPanel.Children[_index]);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                _index--;
            }
        }

        #region Menu
        double _stckpnlMenuWidth = 210;
        double _initialManipulationPointX;
        bool _hamburgerMenuOpen = false;
        bool _hamburgerMenuOpeningClosing = false;

        private void InitializeHamburgerMenu()
        {
            shMenu1.From = -_stckpnlMenuWidth;
            shMenu2.From = -_stckpnlMenuWidth;
            hdMenu1.To = -_stckpnlMenuWidth;
            hdMenu2.To = -_stckpnlMenuWidth;
            stckpnlMenuTop.Width = _stckpnlMenuWidth;
            stckpnlMenuBottom.Width = _stckpnlMenuWidth;
        }

        private void bttnHamburgerMenu_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (_hamburgerMenuOpen)
            {
                HideMenu();
            }
            else
            {
                ShowMenu();
            }
        }

        private void MenuHome_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //txtblckMenuTapped.Text = "Home";
            HideMenu();
        }

        private void MenuAdd_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(FilesPage));
            HideMenu();
        }

        private void MenuSettings_Tapped(object sender, TappedRoutedEventArgs e)
        {
            HideMenu();
            Frame.Navigate(typeof(SettingsPage));
        }

        private void MenuTerms_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //txtblckMenuTapped.Text = "Terms";
            HideMenu();
        }

        private void MenuAbout_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //txtblckMenuTapped.Text = "About";
            HideMenu();
        }

        private void grdPageOverlay_Tapped(object sender, TappedRoutedEventArgs e)
        {
            HideMenu();
        }

        private async void ShowMenu()
        {
            grdPageOverlay.Visibility = Visibility.Visible;
            await strbrdShowMenu.BeginAsync();
            _hamburgerMenuOpen = true;
        }

        private async void HideMenu()
        {
            _hamburgerMenuOpen = false;
            grdPageOverlay.Visibility = Visibility.Collapsed;
            await strbrdHideMenu.BeginAsync();
        }

        private void Page_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            _initialManipulationPointX = e.Position.X;
            _hamburgerMenuOpeningClosing = _hamburgerMenuOpen ? false : _initialManipulationPointX < 30 ? true : false;
        }

        private void Page_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {

            if (_hamburgerMenuOpeningClosing || (_hamburgerMenuOpen && _initialManipulationPointX > e.Position.X))
            {
                _hamburgerMenuOpeningClosing = true;
                if (e.Position.X < _stckpnlMenuWidth + 1)
                {
                    Point currentpoint = e.Position;
                    trnslttrnsfrmMenuTop.X = e.Position.X < _stckpnlMenuWidth ? -_stckpnlMenuWidth + e.Position.X : 0;
                    trnslttrnsfrmMenuBottom.X = e.Position.X < _stckpnlMenuWidth ? -_stckpnlMenuWidth + e.Position.X : 0;
                }
            }
        }

        private async void Page_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (_hamburgerMenuOpeningClosing || (_hamburgerMenuOpen && _initialManipulationPointX > e.Position.X))
            {
                double x = e.Position.X > _stckpnlMenuWidth ? _stckpnlMenuWidth : e.Position.X;
                if (x > _stckpnlMenuWidth / 2)
                {
                    shMenu1.From = -_stckpnlMenuWidth + x;
                    shMenu2.From = -_stckpnlMenuWidth + x;
                    await strbrdShowMenu.BeginAsync();
                    shMenu1.From = -_stckpnlMenuWidth;
                    shMenu2.From = -_stckpnlMenuWidth;
                    grdPageOverlay.Visibility = Visibility.Visible;
                    _hamburgerMenuOpen = true;
                }
                else
                {
                    grdPageOverlay.Visibility = Visibility.Collapsed;
                    hdMenu1.From = x - _stckpnlMenuWidth;
                    hdMenu2.From = x - _stckpnlMenuWidth;
                    await strbrdHideMenu.BeginAsync();
                    hdMenu1.From = 0;
                    hdMenu2.From = 0;
                    _hamburgerMenuOpen = false;
                }
            }
            _hamburgerMenuOpeningClosing = false;
        }

        private void ShowMenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (_hamburgerMenuOpen)
            {
                HideMenu();
            }
            else
            {
                ShowMenu();
            }
        }
        #endregion
    }
}
