using CoptLib.Extensions;
using CoptLib.Models;
using CoptLib.Writing;

namespace CoptLib.Scripting.Commands
{
    public class IpaTranscribeCmd : TextCommandBase
    {
        public IpaTranscribeCmd(string name, IContent content, int startIndex, IDefinition[] parameters)
            : base(name, content, startIndex, parameters)
        {
            Parse(name, content, parameters);
        }

        private void Parse(string cmd, IContent content, params IDefinition[] parameters)
        {
            Output = parameters[0].Select(def =>
            {
                if (def is not IContent content)
                    return;

                content.Text = CopticInterpreter.IpaTranscribe(content.Text ?? content.SourceText);
            });
        }
    }
}
