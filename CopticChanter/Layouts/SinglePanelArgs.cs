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
    }
}
