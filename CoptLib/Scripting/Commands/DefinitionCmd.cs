using CoptLib.Models;
using System;
using System.Linq;

namespace CoptLib.Scripting.Commands
{
    public class DefinitionCmd : TextCommandBase
    {
        public DefinitionCmd(string name, Doc context, int startIndex, string[] parameters)
            : base(name, context, startIndex, parameters)
        {
            Parse(name, context, parameters);
        }

        public Definition Definition { get; private set; }

        private void Parse(string cmd, Doc context, params string[] parameters)
        {
            string defId = parameters[0];
            Definition = context.Definitions.FirstOrDefault(d => d.Key.Equals(defId, StringComparison.OrdinalIgnoreCase));

            if (Definition is Models.String strDef)
                Text = strDef.Value;
        }
    }
}
