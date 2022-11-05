using CoptLib.Models;

namespace CoptLib.Scripting.Commands
{
    public class DefinitionCmd : TextCommandBase, ITextCommandDefOut
    {
        public DefinitionCmd(string name, IContent content, int startIndex, string[] parameters)
            : base(name, content, startIndex, parameters)
        {
            Parse(name, content, parameters);
        }

        public IDefinition Output { get; internal set; }

        private void Parse(string cmd, IContent content, params string[] parameters)
        {
            string defId = parameters[0];
            Output = content.DocContext.Definitions[defId];

            (Output, Text) = this.HandleOutput();
        }
    }
}
