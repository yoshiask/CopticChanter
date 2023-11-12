using System.Diagnostics.CodeAnalysis;
using CoptLib.Extensions;
using CoptLib.IO;
using CoptLib.Models;
using CoptLib.Models.Text;
using CoptLib.Writing;
using CoptLib.Writing.Linguistics;

namespace CoptLib.Scripting.Commands;

public class IpaTranscribeCmd : TextCommandBase
{
    private static readonly LanguageInfo IpaLang = new(KnownLanguage.IPA);

    public IpaTranscribeCmd(string name, InlineCommand inline, IDefinition[] parameters)
        : base(name, inline, parameters)
    {
        Source = Parameters[0];
    }

    public IDefinition Source { get; }

    public LinguisticAnalyzer? Analyzer { get; protected set; }

    private void Transcribe(IDefinition def)
    {
        if (def is Run run)
            run.Text = Analyzer!.IpaTranscribe(run.Text);

        if (def is IMultilingual multi)
        {
            // Ensure that the language and font are set.
            // Set secondary language to indicate transliteration.
            if (!multi.Language.IsDefault())
                multi.Language.Secondary = IpaLang;
            else
                multi.Language = IpaLang;

            multi.Font = null;
        }
    }

    [MemberNotNull(nameof(Analyzer))]
    protected override void ExecuteInternal(ILoadContext? context)
    {
        Analyzer = LinguisticLanguageService.Default.GetAnalyzerForLanguage(Source.GetLanguage());
        Output = Source.Select(Transcribe);
    }
}