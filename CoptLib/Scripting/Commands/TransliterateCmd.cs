using System.Diagnostics.CodeAnalysis;
using CoptLib.Extensions;
using CoptLib.IO;
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
        Language = LanguageInfo.Parse(langParam);
            
        Source = Parameters[^1];

        _syllableSeparator = Parameters.Length > 2 ? Parameters[1].ToString() : "\u00B7\u200B";
    }
    
    public IDefinition Source { get; }

    public LanguageInfo Language { get; }

    public LinguisticAnalyzer? Analyzer { get; private set; }

    private void Transliterate(IDefinition def)
    {
        if (def is Run run)
            run.Text = Analyzer!.Transliterate(run.Text, Language.Known, _syllableSeparator);
            
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

    [MemberNotNull(nameof(Analyzer))]
    protected override void ExecuteInternal(LoadContextBase? context)
    {
        Analyzer = LinguisticLanguageService.Default.GetAnalyzerForLanguage(Source.GetLanguage());
        Output = Source.Select(Transliterate);
        Evaluated = true;
    }
}