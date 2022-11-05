using CoptLib.Models;
using CoptLib.Writing;

namespace CoptLib.Scripting.Commands
{
    public class IpaTranscribeCmd : TextCommandBase, ITextCommandDefOut
    {
        public IpaTranscribeCmd(string name, IContent content, int startIndex, string[] parameters)
            : base(name, content, startIndex, parameters)
        {
            Parse(name, content, parameters);
        }

        public IDefinition Output { get; private set; }

        private void Parse(string cmd, IContent content, params string[] parameters)
        {
            string sourceText = parameters[0];
            Text = CopticInterpreter.IpaTranscribe(sourceText);
        }
    }
}
