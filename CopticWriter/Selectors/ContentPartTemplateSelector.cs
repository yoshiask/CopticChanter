using CoptLib.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CopticWriter.Selectors
{
    public class ContentPartTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Stanza { get; set; }
        public DataTemplate Section { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is Section)
			{
                return Section;
			}
            else 
			{
                return Stanza;
			}
        }

		protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
		{
			return SelectTemplateCore(item);
		}
	}
}
