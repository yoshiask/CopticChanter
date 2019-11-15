using System;
using System.Collections.Generic;
using Windows.UI;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopticChanter.Layouts
{
    public class DoublePanelArgs
    {
        public Common.Language Language1;
        public Common.Language Language2;
        public Color BackColor;
        public Color ForeColor;

        public DoublePanelArgs(Common.Language lang1, Common.Language lang2, Color bcolor, Color fcolor)
        {
            Language1 = lang1;
            Language2 = lang2;
            BackColor = bcolor;
            ForeColor = fcolor;
        }
    }
}
