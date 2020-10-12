using CoptLib;
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

            MainGrid.Background = new SolidColorBrush(args.BackColor.ToUiColor());

            switch (args.Language)
            {
                #region English
                case CopticInterpreter.Language.English:
                    foreach (CoptLib.XML.DocXml doc in Common.Docs)
                    {
                        if (doc.Language == CopticInterpreter.Language.English)
                        {
                            foreach (string content in doc.Content)
                            {
                                string[] split = content.Split(new string[] { "&amp;#xD;", "&#xD;" }, StringSplitOptions.None);
                                foreach (string s in split)
                                {
                                    var contentBlockE = new TextBlock();
                                    contentBlockE.Text = s + "\r\n";
                                    contentBlockE.FontFamily = Common.Segoe;
                                    contentBlockE.FontSize = Common.GetEnglishFontSize();
                                    contentBlockE.TextWrapping = TextWrapping.WrapWholeWords;
                                    contentBlockE.Foreground = new SolidColorBrush(args.ForeColor.ToUiColor());
                                    ContentPanel.Children.Add(contentBlockE);
                                }
                            }
                        }
                    }
                    break;
                #endregion

                #region Coptic
                case CopticInterpreter.Language.Coptic:
                    foreach (CoptLib.XML.DocXml doc in Common.Docs)
                    {
                        if (doc.Language == CopticInterpreter.Language.Coptic)
                        {
                            foreach (string content in doc.Content)
                            {
                                string[] split = content.Split(new string[] { "&amp;#xD;", "&#xD;" }, StringSplitOptions.None);
                                foreach (string s in split)
                                {
                                    string cont = CopticInterpreter.ConvertFont(s, CopticFont.Coptic1, CopticFont.CopticUnicode);
                                    var contentBlockC = new TextBlock();
                                    contentBlockC.Text = cont + "\r\n";
                                    contentBlockC.FontFamily = Common.Coptic1;
                                    contentBlockC.FontSize = Common.GetCopticFontSize();
                                    contentBlockC.TextWrapping = TextWrapping.WrapWholeWords;
                                    contentBlockC.Foreground = new SolidColorBrush(args.ForeColor.ToUiColor());
                                    ContentPanel.Children.Add(contentBlockC);
                                }
                            }
                        }
                    }
                    break;
                #endregion

                #region Arabic
                // TODO: Support Arabic text
                case CopticInterpreter.Language.Arabic:
                    var contentBlockA = new TextBlock();
                    contentBlockA.Text = "\n";
                    contentBlockA.FontFamily = Common.Segoe;
                    contentBlockA.FontSize = 40;
                    contentBlockA.TextWrapping = TextWrapping.WrapWholeWords;
                    ContentPanel.Children.Add(contentBlockA);
                    break;
                    #endregion
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
