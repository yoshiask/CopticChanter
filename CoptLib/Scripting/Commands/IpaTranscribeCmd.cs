using CoptLib.Models;
using CoptLib.Writing;

namespace CoptLib.Scripting.Commands
{
    public class IpaTranscribeCmd : TextCommandBase
    {
        public IpaTranscribeCmd(string name, IContent content, Doc context, int startIndex, string[] parameters)
            : base(name, content, context, startIndex, parameters)
        {
            Parse(name, content, context, parameters);
        }

        private void Parse(string cmd, IContent content, Doc context, params string[] parameters)
        {
            string sourceText = parameters[0];
            Text = CopticInterpreter.IpaTranscribe(sourceText);
        }
    }
}
