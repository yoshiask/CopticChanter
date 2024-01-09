using System.Linq;
using CoptLib.Extensions;
using CoptLib.IO;
using CoptLib.Models;
using CoptLib.Models.Text;
using CoptLib.Writing.Linguistics;

namespace CoptLib.Scripting.Commands;

public class AbbreviationCmd : TextCommandBase
{
    public AbbreviationCmd(string name, InlineCommand inline, IDefinition[] parameters)
        : base(name, inline, parameters)
    {
        AbbreviationKey = Parameters.FirstOrDefault()!.ToString();
        KeepAbbreviated = name == "abvsh";
    }
    
    public string AbbreviationKey { get; }
    
    public bool KeepAbbreviated { get; }
    
    protected override IDefinition ExecuteInternal(ILoadContext? context)
    {
        var analyzer = LinguisticLanguageService.Default.GetAnalyzerForLanguage(Inline.GetLanguage());
        var abbreviation = analyzer.ResolveAbbreviation(AbbreviationKey, KeepAbbreviated);
        
        return new Run(abbreviation, Inline);
    }
}