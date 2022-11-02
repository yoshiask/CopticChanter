using System;
using Windows.UI;
using CoptLib.Writing;

namespace CopticChanter.Layouts
{
    public class DoublePanelArgs
    {
        public Language Language1;
        public Language Language2;
        public Color BackColor;
        public Color ForeColor;

        public DoublePanelArgs(Language lang1, Language lang2, Color bcolor, Color fcolor)
        {
            Language1 = lang1;
            Language2 = lang2;
            BackColor = bcolor;
            ForeColor = fcolor;
        }

        public DoublePanelArgs(Language[] langs, Color bcolor, Color fcolor)
        {
            if (langs.Length < 2)
                throw new ArgumentException("Two languages must be specified for DoublePanel");

            Language1 = langs[0];
            Language2 = langs[1];
            BackColor = bcolor;
            ForeColor = fcolor;
        }

        public DoublePanelArgs(Color bcolor, Color fcolor, params Language[] langs)
        {
            if (langs.Length < 2)
                throw new ArgumentException("Two languages must be specified for DoublePanel");

            Language1 = langs[0];
            Language2 = langs[1];
            BackColor = bcolor;
            ForeColor = fcolor;
        }

        public DoublePanelArgs(params Language[] langs)
        {
            if (langs.Length < 2)
                throw new ArgumentException("Two languages must be specified for DoublePanel");

            Language1 = langs[0];
            Language2 = langs[1];
            BackColor = Colors.Black;
            ForeColor = Colors.White;
        }
    }
}
