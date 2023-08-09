using CoptLib.Extensions;
using CoptLib.IO;
using CoptLib.Models;
using CoptLib.Models.Text;
using CoptLib.Writing;

namespace CoptLib.Scripting.Commands;

public class LanguageCmd : TextCommandBase
{
    public LanguageCmd(string cmd, InlineCommand inline, IDefinition[] parameters)
        : base(cmd, inline, parameters)
    {
        var langParam = Parameters[0].ToString();
        Source = Parameters[^1];

        Language = LanguageInfo.Parse(langParam);

        if (Parameters.Length >= 3 && Language.Known is KnownLanguage.Coptic or KnownLanguage.Greek)
        {
            var fontParam = Parameters[1].ToString();
            Font = DisplayFont.FindFontByMapId(fontParam, () => DisplayFont.CopticStandard);
        }
    }
    
    public IDefinition Source { get; }

    public LanguageInfo Language { get; }

    public DisplayFont? Font { get; }

    protected override void ExecuteInternal(LoadContextBase? context)
    {
        Output = Source.Select(ConvertFont);
        Evaluated = true;
    }

    private void ConvertFont(IDefinition def)
    {
        if (Font != null && def is Run run)
            run.Text = Font.Convert(run.Text);

        if (def is IMultilingual multi)
        {
            // Clear the font since we've already handled it here
            multi.Font = null;

            // Set the new language
            multi.Language = Language;
        }
    }
}