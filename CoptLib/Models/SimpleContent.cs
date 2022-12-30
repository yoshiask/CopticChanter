using CoptLib.Models.Text;
using CoptLib.Scripting;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CoptLib.Models
{
    [XmlRoot("String")]
    public class SimpleContent : IContent
    {
        private string _sourceText;

        public SimpleContent(string sourceText, IDefinition parent)
        {
            SourceText = sourceText;
            Parent = parent;
            DocContext = parent?.DocContext;
        }

        public string SourceText
        {
            get => _sourceText;
            set
            {
                if (_sourceText != value)
                {
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

        public string Key { get; set; }

        public Doc DocContext { get; set; }

        public IDefinition Parent { get; set; }

        public bool IsExplicitlyDefined { get; set; }

        public InlineCollection Inlines { get; set; }

        public List<TextCommandBase> Commands { get; set; }

        public string GetText() => ContentHelper.GetText(this);

        public void HandleCommands() => ContentHelper.HandleCommands(this);

        public override string ToString() => GetText();
    }
}
