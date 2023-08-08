using CoptLib.Extensions;
using CoptLib.Models;
using CoptLib.Models.Text;
using CoptLib.Writing;
using CoptLib.Writing.Linguistics;

namespace CoptLib.Scripting.Commands;

public class TransliterateCmd : TextCommandBase
{
    private readonly string _syllableSeparator;
        
    public TransliterateCmd(string cmd, InlineCommand inline, IDefinition[] parameters)
        : base(cmd, inline, parameters)
    {
        var langParam = Parameters[0].ToString();
        var sourceParam = Parameters[^1];
            
        _syllableSeparator = Parameters.Length > 2 ? Parameters[1].ToString() : "\u00B7\u200B";

        Language = LanguageInfo.Parse(langParam);
        Analyzer = LinguisticLanguageService.Default.GetAnalyzerForLanguage(sourceParam.GetLanguage());
        
        Output = sourceParam.Select(Transliterate);
        Evaluated = true;
    }

    public LanguageInfo Language { get; private set; }

    public LinguisticAnalyzer Analyzer { get; private set; }

    private void Transliterate(IDefinition def)
    {
        if (def is Run run)
            run.Text = Analyzer.Transliterate(run.Text, Language.Known, _syllableSeparator);
            
        if (def is IMultilingual multi)
        {
            // Ensure that the language and font are set.
            // Set secondary language to indicate transliteration.
            if (!multi.Language.IsDefault())
                multi.Language.Secondary = Language;
            else
                multi.Language = Language;

            multi.Font = null;
        }

        // Make sure referenced elements are also transliterated
        if (def is InlineCommand {Command.Output: { }} inCmd)
            inCmd.Command.Output = inCmd.Command.Output.Select(Transliterate);
    }
}