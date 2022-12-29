using CommunityToolkit.Diagnostics;
using CoptLib.Models;
using CoptLib.Models.Text;
using System.Linq;

namespace CoptLib.Scripting.Commands
{
    public class DefinitionCmd : TextCommandBase
    {
        public DefinitionCmd(string name, Run run, IDefinition[] parameters)
            : base(name, run, parameters)
        {
            Parse(name, run, parameters);
        }

        private void Parse(string cmd, Run run, params IDefinition[] parameters)
        {
            string defId = parameters.FirstOrDefault()?.ToString();
            Guard.IsNotNull(defId);

            Output = run.DocContext.Definitions[defId];
            HandleOutput();
        }
    }
}
