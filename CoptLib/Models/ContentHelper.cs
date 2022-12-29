using CoptLib.Models.Text;

namespace CoptLib.Models
{
    /// <summary>
    /// A helper class to reduce duplicate code in classes implementing
    /// <see cref="IContent"/>.
    /// </summary>
    public static class ContentHelper
    {
        public static string GetText(IContent content)
            => content.Inlines == null ? content.SourceText : content.Inlines.ToString();

        public static void ParseCommands(IContent content)
        {
            if (content.HasBeenParsed)
                return;

            content.Inlines.Add(new Run(content.SourceText, content));
            var parsed = Scripting.Scripting.ParseTextCommands(content.Inlines);

            content.Inlines = parsed;
            content.HasBeenParsed = true;
        }

        public static void HandleFont(InlineCollection inlines)
        {
            foreach (Inline inline in inlines)
                inline.HandleFont();
        }
    }
}
