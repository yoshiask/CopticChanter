using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class DoublePanel : Page
    {
        public DoublePanel()
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
            var args = e.Parameter as DoublePanelArgs;
            ApplicationView.GetForCurrentView().FullScreenSystemOverlayMode = FullScreenSystemOverlayMode.Minimal;
            ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            MainGrid.Background = new SolidColorBrush(args.BackColor.ToUiColor());

            #region Left
            switch (args.Language1)
            {
                #region English
                case Common.Language.English:
                    foreach (CoptLib.XML.DocXml doc in Common.Docs)
                    {
                        if (!doc.Coptic)
                        {
                            foreach (CoptLib.XML.DocXml.StanzaXML content in doc.Stanzas)
                            {
                                string[] split = content.Content.Split(new string[] { "&amp;#xD;", "&#xD;" }, StringSplitOptions.None);
                                foreach (string s in split)
                                {
                                    var contentBlockE = new TextBlock();
                                    contentBlockE.Text = s + "\r\n";
                                    contentBlockE.FontFamily = Common.Segoe;
                                    contentBlockE.FontSize = Common.GetEnglishFontSize();
                                    contentBlockE.TextWrapping = TextWrapping.WrapWholeWords;
                                    contentBlockE.Foreground = new SolidColorBrush(args.ForeColor.ToUiColor());
                                    ContentPanelLeft.Children.Add(contentBlockE);
                                }
                            }
                        }
                    }
                    break;
                #endregion

                #region Coptic
                case Common.Language.Coptic:
                    foreach (CoptLib.XML.DocXml doc in Common.Docs)
                    {
                        if (doc.Coptic)
                        {
                            foreach (CoptLib.XML.DocXml.StanzaXML content in doc.Stanzas)
                            {
                                string[] split = content.Content.Split(new string[] { "&amp;#xD;", "&#xD;" }, StringSplitOptions.None);
                                foreach (string s in split)
                                {
                                    string cont = CoptLib.CopticInterpreter.ConvertFromString(s);
                                    var contentBlockC = new TextBlock();
                                    contentBlockC.Text = cont + "\r\n";
                                    contentBlockC.FontFamily = Common.Coptic1;
                                    contentBlockC.FontSize = Common.GetCopticFontSize();
                                    contentBlockC.TextWrapping = TextWrapping.WrapWholeWords;
                                    contentBlockC.Foreground = new SolidColorBrush(args.ForeColor.ToUiColor());
                                    ContentPanelLeft.Children.Add(contentBlockC);
                                }
                            }
                        }
                    }
                    break;
                #endregion

                #region Arabic
                // TODO: Support Arabic text
                case Common.Language.Arabic:
                    var contentBlockA = new TextBlock();
                    contentBlockA.Text = "\n";
                    contentBlockA.FontFamily = Common.Segoe;
                    contentBlockA.FontSize = 40;
                    contentBlockA.TextWrapping = TextWrapping.WrapWholeWords;
                    ContentPanelLeft.Children.Add(contentBlockA);
                    break;
                    #endregion
            }
            #endregion

            #region Right
            switch (args.Language2)
            {
                #region English
                case Common.Language.English:
                    foreach (CoptLib.XML.DocXml doc in Common.Docs)
                    {
                        if (!doc.Coptic)
                        {
                            foreach (CoptLib.XML.DocXml.StanzaXML content in doc.Stanzas)
                            {
                                string[] split = content.Content.Split(new string[] { "&amp;#xD;", "&#xD;" }, StringSplitOptions.None);
                                foreach (string s in split)
                                {
                                    var contentBlockE = new TextBlock();
                                    contentBlockE.Text = s + "\r\n";
                                    contentBlockE.FontFamily = Common.Segoe;
                                    contentBlockE.FontSize = Common.GetEnglishFontSize();
                                    contentBlockE.TextWrapping = TextWrapping.WrapWholeWords;
                                    contentBlockE.Foreground = new SolidColorBrush(args.ForeColor.ToUiColor());
                                    ContentPanelRight.Children.Add(contentBlockE);
                                }
                            }
                        }
                    }
                    break;
                #endregion

                #region Coptic
                case Common.Language.Coptic:
                    foreach (CoptLib.XML.DocXml doc in Common.Docs)
                    {
                        if (doc.Coptic)
                        {
                            foreach (CoptLib.XML.DocXml.StanzaXML content in doc.Stanzas)
                            {
                                string[] split = content.Content.Split(new string[] { "&amp;#xD;", "&#xD;" }, StringSplitOptions.None);
                                foreach (string s in split)
                                {
                                    string cont = CoptLib.CopticInterpreter.ConvertFromString(s);
                                    var contentBlockC = new TextBlock();
                                    contentBlockC.Text = cont + "\r\n";
                                    contentBlockC.FontFamily = Common.Coptic1;
                                    contentBlockC.FontSize = Common.GetCopticFontSize();
                                    contentBlockC.TextWrapping = TextWrapping.WrapWholeWords;
                                    contentBlockC.Foreground = new SolidColorBrush(args.ForeColor.ToUiColor());
                                    ContentPanelRight.Children.Add(contentBlockC);
                                }
                            }
                        }
                    }
                    break;
                #endregion

                #region Arabic
                case Common.Language.Arabic:
                    var contentBlockA = new TextBlock();
                    contentBlockA.Text = "\n";
                    contentBlockA.FontFamily = Common.Segoe;
                    contentBlockA.FontSize = 40;
                    contentBlockA.TextWrapping = TextWrapping.WrapWholeWords;
                    ContentPanelRight.Children.Add(contentBlockA);
                    break;
                    #endregion
            }
            #endregion

            base.OnNavigatedTo(e);
        }

        private int _index = 0;
        private BringIntoViewOptions _scrollOpt = new BringIntoViewOptions() { AnimationDesired = false };
        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            _index--;
            if (_index >= 0)
            {
                ContentViewLeft.ScrollToElement(ContentPanelLeft.Children[_index]);
                ContentViewRight.ScrollToElement(ContentPanelRight.Children[_index]);
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
                ContentViewLeft.ScrollToElement(ContentPanelLeft.Children[_index]);
                ContentViewRight.ScrollToElement(ContentPanelRight.Children[_index]);
                if (Common.IsConnected)
                {
                    //Common.SendMessage(Common.RemoteCMDString.CMD_NEXT);
                }
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Subscribe to remote messages
            if (Common.IsConnected)
            {
                // Subscribe BytesRecieved_CollectionChanged to byte received
            }
        }

        private void BytesRecieved_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (string cmd in e.NewItems)
                {
                    /*switch (cmd)
                    {
                        case Common.RemoteCMDString.CMD_NEXT:
                            RightButton_Click(sender, new RoutedEventArgs());
                            break;

                        case Common.RemoteCMDString.CMD_PREV:
                            LeftButton_Click(sender, new RoutedEventArgs());
                            break;

                        case Common.RemoteCMDString.CMD_SETASREMOTE:
                            Debug.WriteLine("Device requested remote");
                            break;

                        case Common.RemoteCMDString.CMD_SETASDISPLAY:
                            Debug.WriteLine("Device requested display");
                            break;

                        case Common.RemoteCMDString.CMD_ENDMSG:
                            // Nothing more to read, continue to listen
                            break;

                        case Common.RemoteCMDString.CMD_DISCONNECT:
                            Common.Disconnect("Remote requested disconnect");
                            break;

                        case Common.RemoteCMDString.CMD_RECIEVEDOK:
                            Debug.WriteLine("Device recieved and executed command without errors");
                            break;

                        case Common.RemoteCMDString.CMD_RECIEVEDERROR:
                            Debug.WriteLine("Device failed to parse command");
                            break;

                        case Common.RemoteCMDString.CMD_ERROR:
                            Debug.WriteLine("Device failed to execute command");
                            break;
                    }*/
                }
            }
            throw new NotImplementedException();
        }
    }
}