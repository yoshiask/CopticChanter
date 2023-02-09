using CoptLib.ViewModels;
using Windows.UI;

namespace CopticChanter.Layouts
{
    public class DocumentLayoutArgs
    {
        public DocSetViewModel ViewModel;
        public Color BackColor;
        public Color ForeColor;

        public DocumentLayoutArgs(DocSetViewModel vm, Color bcolor, Color fcolor)
        {
            ViewModel = vm;
            BackColor = bcolor;
            ForeColor = fcolor;
        }

        public DocumentLayoutArgs(DocSetViewModel vm) : this(vm, Colors.Black, Colors.White)
        {
        }
    }
}
