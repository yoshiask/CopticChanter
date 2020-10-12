using System;
using Windows.UI;
using static CoptLib.CopticInterpreter;

namespace CopticChanter.Layouts
{
    public class TriplePanelArgs
    {
        public Language Language1;
        public Language Language2;
        public Language Language3;
        public Color BackColor;
        public Color ForeColor;

        public TriplePanelArgs(Language lang1, Language lang2, Language lang3, Color bcolor, Color fcolor)
        {
            Language1 = lang1;
            Language2 = lang2;
            Language3 = lang3;
            BackColor = bcolor;
            ForeColor = fcolor;
        }

        public TriplePanelArgs(Language[] langs, Color bcolor, Color fcolor)
        {
            if (langs.Length < 3)
                throw new ArgumentException("Three languages must be specified for TriplePanel");

            Language1 = langs[0];
            Language2 = langs[1];
            Language3 = langs[2];
            BackColor = bcolor;
            ForeColor = fcolor;
        }

        public TriplePanelArgs(Color bcolor, Color fcolor, params Language[] langs)
        {
            if (langs.Length < 3)
                throw new ArgumentException("Three languages must be specified for TriplePanel");

            Language1 = langs[0];
            Language2 = langs[1];
            Language3 = langs[2];
            BackColor = bcolor;
            ForeColor = fcolor;
        }

        public TriplePanelArgs(params Language[] langs)
        {
            if (langs.Length < 3)
                throw new ArgumentException("Three languages must be specified for TriplePanel");

            Language1 = langs[0];
            Language2 = langs[1];
            Language3 = langs[2];
            BackColor = Colors.Black;
            ForeColor = Colors.White;
        }
    }
}
