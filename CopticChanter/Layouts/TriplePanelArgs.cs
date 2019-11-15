using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace CopticChanter.Layouts
{
    public class TriplePanelArgs
    {
        public Common.Language Language1;
        public Common.Language Language2;
        public Common.Language Language3;
        public Color BackColor;
        public Color ForeColor;

        public TriplePanelArgs(Common.Language lang1, Common.Language lang2, Common.Language lang3, Color bcolor, Color fcolor)
        {
            Language1 = lang1;
            Language2 = lang2;
            Language3 = lang3;
            BackColor = bcolor;
            ForeColor = fcolor;
        }
    }
}
