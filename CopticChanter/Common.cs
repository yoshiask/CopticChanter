using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using static CoptLib.CopticInterpreter;

namespace CopticChanter
{
    public class Common
    {
        #region Doc Info
        public static int EnglishDocCount = 0;
        public static int CopticDocCount = 0;
        public static int ArabicDocCount = 0;
        public static List<CoptLib.XML.DocXml> Docs = new List<CoptLib.XML.DocXml>();
        public static CoptLib.CopticDate CopticDate = CoptLib.CopticDate.Today;
        #endregion

        #region Bluetooth Remote
        public static bool IsConnected = false;
        public static DeviceInformation RemoteInfo;

        /// <summary>
        /// Class containing Attributes and UUIDs that will populate the SDP record.
        /// </summary>
        public class RemoteConstants
        {
            // The Chat Server's custom service Uuid: 5948428E-686D-4F4E-BB78-52922293FFCE
            public static readonly Guid RfcommChatServiceUuid = Guid.Parse("5948428E-686D-4F4E-BB78-52922293FFCE");

            // The Id of the Service Name SDP attribute
            public const UInt16 SdpServiceNameAttributeId = 0x100;

            // The SDP Type of the Service Name SDP attribute.
            // The first byte in the SDP Attribute encodes the SDP Attribute Type as follows :
            //    -  the Attribute Type size in the least significant 3 bits,
            //    -  the SDP Attribute Type value in the most significant 5 bits.
            public const byte SdpServiceNameAttributeType = (4 << 3) | 5;

            // The value of the Service Name SDP attribute
            public const string SdpServiceName = "Coptic Chanter Remote Service";

            public static class RemoteCmdByte
            {
                /// <summary>
                /// Remote Protocol: Proceeds to next page
                /// </summary>
                public const byte CmdNext = 0x20;
                /// <summary>
                /// Remote Protocol: Proceeds to previous page
                /// </summary>
                public const byte CmdPrev = 0x40;
                /// <summary>
                /// Remote Protocol: The sending device is delcared as the remote.
                /// </summary>
                public const byte CmdSetasremote = 0x80;
                /// <summary>
                /// Remote Protocol: The sending device is delcared as the display device.
                /// </summary>
                public const byte CmdSetasdisplay = 0x81;
                /// <summary>
                /// Remote Protocol: End message
                /// </summary>
                public const byte CmdEndmsg = 0x00;
                /// <summary>
                /// Remote Protocol: Closes connection.
                /// </summary>
                public const byte CmdDisconnect = 0xE0;
                /// <summary>
                /// Remote Protocol: Messages recieved and interpreted
                /// </summary>
                public const byte CmdRecievedok = 0xE1;
                /// <summary>
                /// Remote Protocol: Messages recieved but not interpreted
                /// </summary>
                public const byte CmdRecievederror = 0xE2;
                /// <summary>
                /// Remote Protocol: Messages recieved but error executing
                /// </summary>
                public const byte CmdError = 0xE3;
            }
            public static class RemoteCmdString
            {
                /// <summary>
                /// Remote Protocol: Proceeds to next page
                /// </summary>
                public const string CmdNext = "cmd:next";
                /// <summary>
                /// Remote Protocol: Proceeds to previous page
                /// </summary>
                public const string CmdPrev = "cmd:prev";
                /// <summary>
                /// Remote Protocol: The sending device is delcared as the remote.
                /// </summary>
                public const string CmdSetasremote = "cmd:setasremote";
                /// <summary>
                /// Remote Protocol: The sending device is delcared as the display device.
                /// </summary>
                public const string CmdSetasdisplay = "cmd:setasdisplay";
                /// <summary>
                /// Remote Protocol: End message (Exclamation mark and seven spaces)
                /// </summary>
                public const string CmdEndmsg = "!       ";
                /// <summary>
                /// Remote Protocol: Closes connection.
                /// </summary>
                public const string CmdDisconnect = "co:disconnect";
                /// <summary>
                /// Remote Protocol: Messages recieved and interpreted
                /// </summary>
                public const string CmdRecievedok = "status:ok";
                /// <summary>
                /// Remote Protocol: Messages recieved but not interpreted
                /// </summary>
                public const string CmdRecievederror = "status:parseerror";
                /// <summary>
                /// Remote Protocol: Messages recieved but error executing
                /// </summary>
                public const string CmdError = "status:execerror";
            }
        }
        #endregion

        #region Styles
        public static FontFamily Segoe = new FontFamily("Segoe UI");
        public static FontFamily Coptic1 = new FontFamily("/Assets/Coptic1.ttf#Coptic1");
        private static SolidColorBrush _accentBrush;
        private static Color _accentColor;
        #endregion

        public static SolidColorBrush GetAccentBrush()
        {
            if (_accentBrush == null)
            {
                if (_accentColor == null)
                    _accentBrush = Application.Current.Resources["SystemControlHighlightAccentBrush"] as SolidColorBrush;
                else
                {
                    _accentBrush = new SolidColorBrush(_accentColor);
                }
            }
            return _accentBrush;
        }
        public static void SetAccentColor(Color accent)
        {
            _accentColor = accent;
        }

        public static int GetFontSize(Language lang)
        {
            switch (lang) {
                case Language.English:
                    if (!ApplicationData.Current.LocalSettings.Values.ContainsKey("font-size-e"))
                    {
                        ApplicationData.Current.LocalSettings.Values.Add("font-size-e", 40);
                    }
                    return (int)ApplicationData.Current.LocalSettings.Values["font-size-e"];

                case Language.Coptic:
                    if (!ApplicationData.Current.LocalSettings.Values.ContainsKey("font-size-c"))
                    {
                        ApplicationData.Current.LocalSettings.Values.Add("font-size-c", 40);
                    }
                    return (int)ApplicationData.Current.LocalSettings.Values["font-size-c"];

                case Language.Arabic:
                    if (!ApplicationData.Current.LocalSettings.Values.ContainsKey("font-size-a"))
                    {
                        ApplicationData.Current.LocalSettings.Values.Add("font-size-a", 40);
                    }
                    return (int)ApplicationData.Current.LocalSettings.Values["font-size-a"];

                default:
                    return 40;
            }
        }

        public static void SetFontSize(Language lang, int size)
        {
            switch (lang)
            {
                case Language.English:
                    if (ApplicationData.Current.LocalSettings.Values.ContainsKey("font-size-e"))
                    {
                        ApplicationData.Current.LocalSettings.Values["font-size-e"] = size;
                    }
                    else
                    {
                        ApplicationData.Current.LocalSettings.Values.Add("font-size-e", size);
                    }
                    break;

                case Language.Coptic:
                    if (ApplicationData.Current.LocalSettings.Values.ContainsKey("font-size-c"))
                    {
                        ApplicationData.Current.LocalSettings.Values["font-size-c"] = size;
                    }
                    else
                    {
                        ApplicationData.Current.LocalSettings.Values.Add("font-size-c", size);
                    }
                    break;

                case Language.Arabic:
                    if (ApplicationData.Current.LocalSettings.Values.ContainsKey("font-size-a"))
                    {
                        ApplicationData.Current.LocalSettings.Values["font-size-a"] = size;
                    }
                    else
                    {
                        ApplicationData.Current.LocalSettings.Values.Add("font-size-a", size);
                    }
                    break;
            }
        }

        public static void SetEnglishFontSize(int size)
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("font-size-e"))
            {
                ApplicationData.Current.LocalSettings.Values["font-size-e"] = size;
            }
            else
            {
                ApplicationData.Current.LocalSettings.Values.Add("font-size-e", size);
            }
        }

        public static int GetEnglishFontSize()
        {
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey("font-size-e"))
            {
                ApplicationData.Current.LocalSettings.Values.Add("font-size-e", 40);
            }
            return (int)ApplicationData.Current.LocalSettings.Values["font-size-e"];
        }

        public static void SetCopticFontSize(int size)
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("font-size-c"))
            {
                ApplicationData.Current.LocalSettings.Values["font-size-c"] = size;
            }
            else
            {
                ApplicationData.Current.LocalSettings.Values.Add("font-size-c", size);
            }
        }

        public static int GetCopticFontSize()
        {
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey("font-size-c"))
            {
                ApplicationData.Current.LocalSettings.Values.Add("font-size-c", 45);
            }
            return (int)ApplicationData.Current.LocalSettings.Values["font-size-c"];
        }
    }

    public class WinVer
    {
        static ulong _winVersion;
        static ulong _winBuild;

        /// <summary>
        /// Returns short Version number (ex. 1709)
        /// </summary>
        /// <returns></returns>
        public static ulong GetWinVer()
        {
            if (_winVersion == 0)
            {
                string sv = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
                ulong v = ulong.Parse(sv);
                ulong v1 = (v & 0xFFFF000000000000L) >> 48;
                ulong v2 = (v & 0x0000FFFF00000000L) >> 32;
                ulong v3 = (v & 0x00000000FFFF0000L) >> 16;
                ulong v4 = v & 0x000000000000FFFFL;
                string version = $"{v1}.{v2}.{v3}.{v4}"; // == 10.0.10240.16413
                _winBuild = v3;

                if (_winBuild >= 17134)
                {
                    _winVersion = 1803;
                }
                else if (_winBuild >= 16299)
                {
                    _winVersion = 1709;
                }
                else if (_winBuild >= 15063)
                {
                    _winVersion = 1703;
                }
                else if (_winBuild >= 14393)
                {
                    _winVersion = 1607;
                }
                else if (_winBuild >= 10586)
                {
                    _winVersion = 1511;
                }
                else if (_winBuild >= 10240)
                {
                    _winVersion = 1507;
                }
                else if (_winBuild < 10240)
                {
                    _winVersion = 1;
                }
            }

            return _winVersion;
        }

        /// <summary>
        /// Returns long Build number (ex. 16299)
        /// </summary>
        /// <returns></returns>
        public static ulong GetWinBuild()
        {
            if (_winVersion == 0)
            {
                string sv = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
                ulong v = ulong.Parse(sv);
                ulong v1 = (v & 0xFFFF000000000000L) >> 48;
                ulong v2 = (v & 0x0000FFFF00000000L) >> 32;
                ulong v3 = (v & 0x00000000FFFF0000L) >> 16;
                ulong v4 = v & 0x000000000000FFFFL;
                string version = $"{v1}.{v2}.{v3}.{v4}"; // == 10.0.10240.16413
                _winBuild = v3;

                if (_winBuild >= 17134)
                {
                    _winVersion = 1803;
                }
                else if (_winBuild >= 16299)
                {
                    _winVersion = 1709;
                }
                else if (_winBuild >= 15063)
                {
                    _winVersion = 1703;
                }
                else if (_winBuild >= 14393)
                {
                    _winVersion = 1607;
                }
                else if (_winBuild >= 10586)
                {
                    _winVersion = 1511;
                }
                else if (_winBuild >= 10240)
                {
                    _winVersion = 1507;
                }
                else if (_winBuild < 10240)
                {
                    _winVersion = 1;
                }
            }

            return _winBuild;
        }
    }

    public static class ColorExtensions
    {
        public static Windows.UI.Color ToUiColor(this Windows.UI.Color color)
        {
            return Windows.UI.Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }

    public static class MathExtensions
    {
        public static double Round(this double num, int increment)
        {
            double output = num;

            var fnum = Math.Floor(num);
            double target = 0;

            for (target = 0; target < fnum; target += increment)
            {
                Debug.WriteLine(target + ": " + fnum);
            }

            return output;
        }
    }

    public static class StoryboardExtensions
    {
        public static Task BeginAsync(this Storyboard storyboard)
        {
            try
            {
                TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
                if (storyboard == null)
                    tcs.SetException(new ArgumentNullException());
                else
                {
                    EventHandler<object> onComplete = null;
                    onComplete = (s, e) => {
                        storyboard.Completed -= onComplete;
                        tcs.SetResult(true);
                    };
                    storyboard.Completed += onComplete;
                    storyboard.Begin();
                }
                return tcs.Task;
            }
            catch
            {
                return null;
            }
        }

    }

    public static class ViewExtensions
    {
        public static void ScrollToElement(this ScrollViewer scrollViewer, UIElement element,
            bool isVerticalScrolling = true, bool smoothScrolling = true, float? zoomFactor = null)
        {
            if (element == null)
                return;

            var transform = element.TransformToVisual((UIElement)scrollViewer.Content);
            var position = transform.TransformPoint(new Windows.Foundation.Point(0, 0));

            if (isVerticalScrolling)
            {
                scrollViewer.ChangeView(null, position.Y, zoomFactor, !smoothScrolling);
            }
            else
            {
                scrollViewer.ChangeView(position.X, null, zoomFactor, !smoothScrolling);
            }
        }
    }

    public static class DateTimeExtensions
    {
        public static CoptLib.CopticDate ToCoptic(this DateTime date)
        {
            return CoptLib.CopticDate.ToCopticDate(date);
        }
    }
}
