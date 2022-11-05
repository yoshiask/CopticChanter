using CoptLib.Models;

namespace CoptLib.Scripting.Commands
{
    public class DefinitionCmd : TextCommandBase
    {
        public DefinitionCmd(string name, IContent content, Doc context, int startIndex, string[] parameters)
            : base(name, content, context, startIndex, parameters)
        {
            Parse(name, content, context, parameters);
        }

        public IDefinition Definition { get; private set; }

        private void Parse(string cmd, IContent content, Doc context, params string[] parameters)
        {
            string defId = parameters[0];
            Definition = context.Definitions[defId];

            if (Definition is TranslationCollection defCol && content is IMultilingual multi)
                Definition = defCol[multi.Language];
            if (Definition is IContent defContent)
                Text = defContent.Text;
        }
    }
}
