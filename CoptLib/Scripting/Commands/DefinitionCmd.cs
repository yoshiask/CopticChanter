using CoptLib.Models;

namespace CoptLib.Scripting.Commands
{
    public class DefinitionCmd : TextCommandBase
    {
        public DefinitionCmd(string name, IContent content, int startIndex, IDefinition[] parameters)
            : base(name, content, startIndex, parameters)
        {
            Parse(name, content, parameters);
        }

        private void Parse(string cmd, IContent content, params IDefinition[] parameters)
        {
            string defId = ((IContent)parameters[0]).SourceText;
            Output = content.DocContext.Definitions[defId];
            HandleOutput();
        }
    }
}
