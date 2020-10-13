using System;
using Windows.UI;
using static CoptLib.CopticInterpreter;

namespace CopticChanter.Layouts
{
    public class DocumentLayoutArgs
    {
        public Language[] Languages;
        public Color BackColor;
        public Color ForeColor;

        public DocumentLayoutArgs(Language[] langs, Color bcolor, Color fcolor)
        {
            if (langs.Length < 1)
                throw new ArgumentException("At least one language must be specified");

            Languages = langs;
            BackColor = bcolor;
            ForeColor = fcolor;
        }

        public DocumentLayoutArgs(Color bcolor, Color fcolor, params Language[] langs)
        {
            if (langs.Length < 1)
                throw new ArgumentException("At least one language must be specified");

            Languages = langs;
            BackColor = bcolor;
            ForeColor = fcolor;
        }

        public DocumentLayoutArgs(params Language[] langs)
        {
            if (langs.Length < 1)
                throw new ArgumentException("At least one language must be specified");

            Languages = langs;
            BackColor = Colors.Black;
            ForeColor = Colors.White;
        }
    }
}
