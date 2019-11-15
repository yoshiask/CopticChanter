using System;
using System.Collections.Generic;
using Windows.UI;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopticChanter.Layouts
{
    public class SinglePanelArgs
    {
        public Common.Language Language;
        public Color BackColor;
        public Color ForeColor;

        public SinglePanelArgs(Common.Language lang, Color bcolor, Color fcolor)
        {
            Language = lang;
            BackColor = bcolor;
            ForeColor = fcolor;
        }
    }
}
