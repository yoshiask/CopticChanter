using System.Linq;
using CommunityToolkit.Diagnostics;
using CoptLib.Extensions;
using CoptLib.Models;
using CoptLib.Models.Text;
using CoptLib.Writing.Linguistics;

namespace CoptLib.Scripting.Commands;

public class AbbreviationCmd : TextCommandBase
{
    public AbbreviationCmd(string name, InlineCommand inline, IDefinition[] parameters)
        : base(name, inline, parameters)
    {
        Parse(name == "abvsh");
    }

    private void Parse(bool keepAbbreviated)
    {
        var abvKey = Parameters.FirstOrDefault()?.ToString();
        Guard.IsNotNull(abvKey);

        var analyzer = LinguisticLanguageService.Default.GetAnalyzerForLanguage(Inline.GetLanguage());
        var abbreviation = analyzer.ResolveAbbreviation(abvKey, keepAbbreviated);
        
        Output = new Run(abbreviation, Inline);

        Evaluated = true;
    }
}