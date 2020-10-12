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
    }
}
