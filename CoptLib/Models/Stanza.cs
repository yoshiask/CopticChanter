using CoptLib.Models.Text;
using CoptLib.Scripting;
using System.Collections.Generic;

namespace CoptLib.Models
{
    public class Stanza : ContentPart, IContent
    {
        private string _sourceText;

        public Stanza(IDefinition parent) : base(parent)
        {

        }

        public string SourceText
        {
            get => _sourceText;
            set
            {
                if (_sourceText != value)
                {
                    FontHandled = false;
                    CommandsHandled = false;
                    Inlines = new()
                    {
                        new Run(value, this)
                    };
                }

                _sourceText = value;
            }
        }

        public bool CommandsHandled { get; set; }

        public InlineCollection Inlines { get; set; }

        public List<TextCommandBase> Commands { get; set; }

        public void HandleCommands() => ContentHelper.HandleCommands(this);

        public override void HandleFont()
        {
            ContentHelper.HandleFont(Inlines);
            FontHandled = true;
        }

        public string GetText() => ContentHelper.GetText(this);

        public override string ToString() => GetText();
    }
}
