using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Profile;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
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
            trnslttrnsfrmMenuTop.X = -stckpnlMenuWidth;
            trnslttrnsfrmMenuBottom.X = -stckpnlMenuWidth;
            this.ManipulationMode = ManipulationModes.TranslateX;
            #endregion

            var args = Common.OnePanelArgs;
            ApplicationView.GetForCurrentView().FullScreenSystemOverlayMode = FullScreenSystemOverlayMode.Minimal;
            ApplicationView.GetForCurrentView().TryEnterFullScreenMode();

            MainGrid.Background = new SolidColorBrush(args.BackColor.ToUIColor());

            switch (args.Language)
            {
                #region English
                case Common.Language.English:
                    foreach (CoptLib.XML.DocXML doc in Common.Docs)
                    {
                        if (!doc.Coptic)
                        {
                            foreach (CoptLib.XML.DocXML.StanzaXML content in doc.Stanzas)
                            {
                                string[] split = content.Content.Split(new string[] { "&amp;#xD;", "&#xD;" }, StringSplitOptions.None);
                                foreach (string s in split)
                                {
                                    var ContentBlockE = new TextBlock();
                                    ContentBlockE.Text = s + "\r\n";
                                    ContentBlockE.FontFamily = Common.Segoe;
                                    ContentBlockE.FontSize = Common.GetEnglishFontSize();
                                    ContentBlockE.TextWrapping = TextWrapping.WrapWholeWords;
                                    ContentBlockE.Foreground = new SolidColorBrush(args.ForeColor.ToUIColor());
                                    ContentPanel.Children.Add(ContentBlockE);
                                }
                            }
                        }
                    }
                    break;
                #endregion

                #region Coptic
                case Common.Language.Coptic:
                    foreach (CoptLib.XML.DocXML doc in Common.Docs)
                    {
                        if (doc.Coptic)
                        {
                            foreach (CoptLib.XML.DocXML.StanzaXML content in doc.Stanzas)
                            {
                                string[] split = content.Content.Split(new string[] { "&amp;#xD;", "&#xD;" }, StringSplitOptions.None);
                                foreach (string s in split)
                                {
                                    string cont = CoptLib.CopticInterpreter.ConvertFromString(s);
                                    var ContentBlockC = new TextBlock();
                                    ContentBlockC.Text = cont + "\r\n";
                                    ContentBlockC.FontFamily = Common.Coptic1;
                                    ContentBlockC.FontSize = Common.GetCopticFontSize();
                                    ContentBlockC.TextWrapping = TextWrapping.WrapWholeWords;
                                    ContentBlockC.Foreground = new SolidColorBrush(args.ForeColor.ToUIColor());
                                    ContentPanel.Children.Add(ContentBlockC);
                                }
                            }
                        }
                    }
                    break;
                #endregion

                #region Arabic
                case Common.Language.Arabic:
                    var ContentBlockA = new TextBlock();
                    ContentBlockA.Text = "\n";
                    ContentBlockA.FontFamily = Common.Segoe;
                    ContentBlockA.FontSize = 40;
                    ContentBlockA.TextWrapping = TextWrapping.WrapWholeWords;
                    ContentPanel.Children.Add(ContentBlockA);
                    break;
                #endregion
            }
        }

        private int index = 0;
        private BringIntoViewOptions ScrollOpt = new BringIntoViewOptions() { AnimationDesired = false };
        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            index--;
            if (index >= 0)
            {
                ContentView.ScrollToElement(ContentPanel.Children[index]);
            }
            else
            {
                index++;
            }
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                index++;
                ContentView.ScrollToElement(ContentPanel.Children[index]);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                index--;
            }
        }

        #region Menu
        double stckpnlMenuWidth = 210;
        double InitialManipulationPointX;
        bool HamburgerMenuOpen = false;
        bool HamburgerMenuOpeningClosing = false;

        private void InitializeHamburgerMenu()
        {
            shMenu1.From = -stckpnlMenuWidth;
            shMenu2.From = -stckpnlMenuWidth;
            hdMenu1.To = -stckpnlMenuWidth;
            hdMenu2.To = -stckpnlMenuWidth;
            stckpnlMenuTop.Width = stckpnlMenuWidth;
            stckpnlMenuBottom.Width = stckpnlMenuWidth;
        }

        private void bttnHamburgerMenu_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (HamburgerMenuOpen)
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
            HamburgerMenuOpen = true;
        }

        private async void HideMenu()
        {
            HamburgerMenuOpen = false;
            grdPageOverlay.Visibility = Visibility.Collapsed;
            await strbrdHideMenu.BeginAsync();
        }

        private void Page_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            InitialManipulationPointX = e.Position.X;
            HamburgerMenuOpeningClosing = HamburgerMenuOpen ? false : InitialManipulationPointX < 30 ? true : false;
        }

        private void Page_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {

            if (HamburgerMenuOpeningClosing || (HamburgerMenuOpen && InitialManipulationPointX > e.Position.X))
            {
                HamburgerMenuOpeningClosing = true;
                if (e.Position.X < stckpnlMenuWidth + 1)
                {
                    Point currentpoint = e.Position;
                    trnslttrnsfrmMenuTop.X = e.Position.X < stckpnlMenuWidth ? -stckpnlMenuWidth + e.Position.X : 0;
                    trnslttrnsfrmMenuBottom.X = e.Position.X < stckpnlMenuWidth ? -stckpnlMenuWidth + e.Position.X : 0;
                }
            }
        }

        private async void Page_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (HamburgerMenuOpeningClosing || (HamburgerMenuOpen && InitialManipulationPointX > e.Position.X))
            {
                double X = e.Position.X > stckpnlMenuWidth ? stckpnlMenuWidth : e.Position.X;
                if (X > stckpnlMenuWidth / 2)
                {
                    shMenu1.From = -stckpnlMenuWidth + X;
                    shMenu2.From = -stckpnlMenuWidth + X;
                    await strbrdShowMenu.BeginAsync();
                    shMenu1.From = -stckpnlMenuWidth;
                    shMenu2.From = -stckpnlMenuWidth;
                    grdPageOverlay.Visibility = Visibility.Visible;
                    HamburgerMenuOpen = true;
                }
                else
                {
                    grdPageOverlay.Visibility = Visibility.Collapsed;
                    hdMenu1.From = X - stckpnlMenuWidth;
                    hdMenu2.From = X - stckpnlMenuWidth;
                    await strbrdHideMenu.BeginAsync();
                    hdMenu1.From = 0;
                    hdMenu2.From = 0;
                    HamburgerMenuOpen = false;
                }
            }
            HamburgerMenuOpeningClosing = false;
        }

        private void ShowMenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (HamburgerMenuOpen)
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
