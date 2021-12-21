using Windows.UI;

namespace CopticChanter.Layouts
{
    public class DocumentLayoutArgs
    {
        public Color BackColor;
        public Color ForeColor;

        public DocumentLayoutArgs(Color bcolor, Color fcolor)
        {
            BackColor = bcolor;
            ForeColor = fcolor;
        }

        public DocumentLayoutArgs()
        {
            BackColor = Colors.Black;
            ForeColor = Colors.White;
        }
    }
}
