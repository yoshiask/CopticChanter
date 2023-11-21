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

    protected override IDefinition ExecuteInternal(ILoadContext? context) =>
        LinguisticLanguageService.Default.Transliterate(Source, Language, syllableSeparator: _syllableSeparator);
}