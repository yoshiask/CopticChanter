using CoptLib.Models.Text;
using CoptLib.Scripting;

namespace CoptLib.Models;

/// <summary>
/// A helper class to reduce duplicate code in classes implementing
/// <see cref="IContent"/>.
/// </summary>
public static class ContentHelper
{
    public static string UpdateSourceText(IContent content, string sourceText)
    {
        if (sourceText != content.SourceText)
        {
            content.Commands = new();

            if (sourceText == string.Empty)
            {
                content.CommandsHandled = true;
                content.Inlines = InlineCollection.Empty;
            }
            else
            {
                content.CommandsHandled = false;
                content.Inlines = new()
                {
                    new Run(sourceText, content)
                };
            }
        }

        return sourceText;
    }

    public static string GetText(IContent content)
        => content.CommandsHandled ? content.Inlines.ToString() : content.SourceText;

    public static void HandleCommands(IContent content)
    {
        if (content.CommandsHandled)
            return;

        var parsed = ScriptingEngine.ParseTextCommands(content.Inlines);
        var cmds = ScriptingEngine.RunTextCommands(parsed);

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