using CoptLib.Extensions;
using CoptLib.Models;
using CoptLib.Models.Text;
using CoptLib.Writing;

namespace CoptLib.Scripting.Commands
{
    public class IpaTranscribeCmd : TextCommandBase
    {
        private static readonly LanguageInfo _ipaLang = new(KnownLanguage.IPA);

        public IpaTranscribeCmd(string name, InlineCommand inline, IDefinition[] parameters)
            : base(name, inline, parameters)
        {
            Parse();
        }

        private void Parse()
        {
            Output = Parameters[0].Select(Transcribe);
            Evaluated = true;
        }

        private void Transcribe(IDefinition def)
        {
            if (def is Run run)
                run.Text = CopticInterpreter.IpaTranscribe(run.Text);

            if (def is IMultilingual multi)
            {
                // Ensure that the language and font are set.
                // Set secondary language to indicate transliteration.
                if (multi.Language != null)
                    multi.Language.Secondary = _ipaLang;
                else
                    multi.Language = _ipaLang;

                multi.Font = null;
            }

            // Make sure referenced elements are also transliterated
            if (def is InlineCommand inCmd && inCmd.Command.Output != null)
                inCmd.Command.Output = inCmd.Command.Output.Select(Transcribe);
        }
    }
}
