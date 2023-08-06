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
    public class Settings
    {
        private const string KEY_FONT_FAMILY = "font-family";
        private const string KEY_FONT_SIZE = "font-size";
        private const string KEY_CHARMAP_ID = "char-map-id";

        public static FontFamily Segoe = new FontFamily("Segoe UI");
        public static FontFamily LeagueSpartan = new FontFamily("League Spartan YGA");
        public static FontFamily Coptic1 = new FontFamily("/Assets/Coptic1.ttf#Coptic1");

        public static int GetFontSize()
        {
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey(KEY_FONT_SIZE))
                ApplicationData.Current.LocalSettings.Values.Add(KEY_FONT_SIZE, 40);

            return (int)ApplicationData.Current.LocalSettings.Values[KEY_FONT_SIZE];
        }

        public static void SetFontSize(int size)
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(KEY_FONT_SIZE))
                ApplicationData.Current.LocalSettings.Values[KEY_FONT_SIZE] = size;
            else
                ApplicationData.Current.LocalSettings.Values.Add(KEY_FONT_SIZE, size);
        }

        public static FontFamily GetFontFamily() => new FontFamily(GetFontFamilyName());

        public static string GetFontFamilyName()
        {
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey(KEY_FONT_FAMILY))
                ApplicationData.Current.LocalSettings.Values.Add(KEY_FONT_FAMILY, LeagueSpartan.Source);

            return (string)ApplicationData.Current.LocalSettings.Values[KEY_FONT_FAMILY];
        }

        public static void SetFontFamily(string familyName)
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(KEY_FONT_FAMILY))
                ApplicationData.Current.LocalSettings.Values[KEY_FONT_FAMILY] = familyName;
            else
                ApplicationData.Current.LocalSettings.Values.Add(KEY_FONT_FAMILY, familyName);
        }

        public static string GetCharacterMapId()
        {
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey(KEY_CHARMAP_ID))
                SetCharacterMapId(DisplayFont.UnicodeMapId);

            return (string)ApplicationData.Current.LocalSettings.Values[KEY_CHARMAP_ID];
        }

        public static void SetCharacterMapId(string characterMapId)
            => ApplicationData.Current.LocalSettings.Values[KEY_CHARMAP_ID] = characterMapId;

        public static DisplayFont GetDisplayFont()
        {
            var font = DisplayFont.FindFontByMapId(GetCharacterMapId());
            font.FontFamily = GetFontFamilyName();
            return font;
        }
    }
}
