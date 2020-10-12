using System;
using System.Collections.Generic;
using Windows.UI;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CoptLib.CopticInterpreter;

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
    }
}
