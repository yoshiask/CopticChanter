using CoptLib.IO;
using CoptLib.Writing;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace CopticChanter
{
    public class Common
    {
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

        public static int GetFontSize(KnownLanguage lang)
        {
            string key = $"font-size-{lang}";
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
                ApplicationData.Current.LocalSettings.Values.Add(key, 40);

            return (int)ApplicationData.Current.LocalSettings.Values[key];
        }

        public static void SetFontSize(KnownLanguage lang, int size)
        {
            string key = $"font-size-{lang}";
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
                ApplicationData.Current.LocalSettings.Values[key] = size;
            else
                ApplicationData.Current.LocalSettings.Values.Add(key, size);
        }

        public static FontFamily GetFontFamily(KnownLanguage lang)
        {
            string key = $"font-family-{lang}";
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
                ApplicationData.Current.LocalSettings.Values.Add(key, Segoe.Source);

            string familyName = (string)ApplicationData.Current.LocalSettings.Values[key];
            return new FontFamily(familyName);
        }

        public static void SetFontFamily(KnownLanguage lang, string familyName)
        {
            string key = $"font-family-{lang}";
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
                ApplicationData.Current.LocalSettings.Values[key] = familyName;
            else
                ApplicationData.Current.LocalSettings.Values.Add(key, familyName);
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
}
