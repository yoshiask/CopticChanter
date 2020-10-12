using System;
using Windows.UI;
using static CoptLib.CopticInterpreter;

namespace CopticChanter.Layouts
{
    public class SinglePanelArgs
    {
        public Language Language;
        public Color BackColor;
        public Color ForeColor;

        public SinglePanelArgs(Language lang, Color bcolor, Color fcolor)
        {
            Language = lang;
            BackColor = bcolor;
            ForeColor = fcolor;
        }

        public SinglePanelArgs(Language[] langs, Color bcolor, Color fcolor)
        {
            if (langs.Length < 2)
                throw new ArgumentException("One languages must be specified for DoublePanel");

            Language = langs[0];
            BackColor = bcolor;
            ForeColor = fcolor;
        }

        public SinglePanelArgs(Color bcolor, Color fcolor, params Language[] langs)
        {
            if (langs.Length < 2)
                throw new ArgumentException("Two languages must be specified for DoublePanel");

            Language = langs[0];
            BackColor = bcolor;
            ForeColor = fcolor;
        }

        public SinglePanelArgs(params Language[] langs)
        {
            if (langs.Length < 1)
                throw new ArgumentException("One language must be specified for SinglePanel");

            Language = langs[0];
            BackColor = Colors.Black;
            ForeColor = Colors.White;
        }
    }
}
