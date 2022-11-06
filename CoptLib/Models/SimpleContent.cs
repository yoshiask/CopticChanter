using CoptLib.Scripting;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CoptLib.Models
{
    [XmlRoot("String")]
    public class SimpleContent : IContent
    {
        public SimpleContent(string sourceText, IDefinition parent)
        {
            SourceText = sourceText;
            Parent = parent;
            DocContext = parent?.DocContext;
        }

        public string SourceText { get; set; }

        public bool HasBeenParsed { get; private set; }

        public string Text { get; private set; }

        public List<TextCommandBase> Commands { get; private set; }

        public string Key { get; set; }

        public Doc DocContext { get; set; }

        public IDefinition Parent { get; set; }

        public void ParseCommands()
        {
            if (HasBeenParsed)
                return;

            Commands = Scripting.Scripting.ParseTextCommands(this, out var text);
            Text = text;
            HasBeenParsed = true;
        }

        public override string ToString() => Text ?? SourceText;
    }
}
