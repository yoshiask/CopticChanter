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

        public static void HandleCommands(IContent content)
        {
            if (content.CommandsHandled)
                return;

            var parsed = Scripting.Scripting.ParseTextCommands(content.Inlines);
            var cmds = Scripting.Scripting.RunTextCommands(parsed);

            content.Inlines = parsed;
            content.Commands = cmds;
            content.CommandsHandled = true;
        }

        public static void HandleFont(InlineCollection inlines)
        {
            foreach (Inline inline in inlines)
                inline.HandleFont();
        }
    }
}
