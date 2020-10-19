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
            switch (item)
			{
                default:
                case Stanza _:
                    return Stanza;

                case Section _:
                    return Section;
			}
        }

		protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
		{
			return SelectTemplateCore(item);
		}
	}
}
