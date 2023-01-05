using CommunityToolkit.Diagnostics;
using CoptLib.Models;
using CoptLib.Models.Text;
using System.Linq;

namespace CoptLib.Scripting.Commands
{
    public class DefinitionCmd : TextCommandBase
    {
        public DefinitionCmd(string name, InlineCommand inline, IDefinition[] parameters)
            : base(name, inline, parameters)
        {
            Parse();
        }

        private void Parse()
        {
            string defId = Parameters.FirstOrDefault()?.ToString();
            Guard.IsNotNull(defId);

            Output = Inline.DocContext.LookupDefinition(defId);
            HandleOutput();
        }
    }
}
