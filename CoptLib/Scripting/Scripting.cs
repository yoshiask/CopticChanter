using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoptLib.Extensions;
using CoptLib.Models;
using CoptLib.Models.Text;
using CoptLib.Scripting.Commands;
using NLua;
using NodaTime;
using OwlCore.Extensions;

namespace CoptLib.Scripting
{
    public class Scripting
    {
        private static readonly Dictionary<string, Type> _availCmds = new()
        {
            { "def", typeof(DefinitionCmd) },
            { "ipa", typeof(IpaTranscribeCmd) },
            { "language", typeof(LanguageCmd) },
            { "lang", typeof(LanguageCmd) },
            { "ms", typeof(TimestampCmd) },
            { "trslit", typeof(TransliterateCmd) },
        };

        public static IDictionary<string, bool> GetArgs(LocalDate date)
        {
            if (date == null)
                date = CopticDateHelper.TodayCoptic;

            bool isPascha = date == CopticDateHelper.GetNextPascha(date);
            var args = new Dictionary<string, bool>
            {
                // Check if today is the Feast of the Nativity, always Jan. 7th [Gregorian]
                { "Nativity", date == CopticDateHelper.GetNextNativity(date) },

                // Check if today is the Sunday before the Feast of the Nativity
                { "NativitySunday", date == CopticDateHelper.GetNextNativitySunday(date) },

                // Check if today is during Great Lent
                //{ "GreatLent", date >= CopticDateHelper.GreatLentStart && date < CopticDateHelper.PalmSunday },

                // Check if today is during Holy Week
                { "HolyWeek", date >= CopticDateHelper.GetNextHosannaSunday(date) && date < CopticDateHelper.GetNextPascha(date) },

                // Check if today is Palm Sunday
                { "PalmSunday", date == CopticDateHelper.GetNextHosannaSunday(date) },

                // Check if today is Pascha
                { "Pascha", isPascha },
                { "Easter", isPascha },

                // Check if today is the Feast of the Ressurection
                { "Ressurection", date == CopticDateHelper.GetNextFeastResurrection(date) }
            };

            return args;
        }

        public static IDefinition RunLuaScript(string scriptBody)
        {
            Lua state = new();
            state.LoadCLRPackage();
            state.DoString("import ('CoptLib', 'CoptLib.Models')");
            state.DoString("import ('NodaTime', 'NodaTime')");

            // Add the CoptLib date functions
            state["Today"] = CopticDateHelper.TodayCoptic;
            //state["NextCovenantThursday"] = (Func<LocalDate>)CopticDateHelper.GetNextCovenantThursday;
            state["NextFeastResurrection"] = (Func<LocalDate>)CopticDateHelper.GetNextFeastResurrection;
            //state["NextGoodFriday"] = (Func<LocalDate>)CopticDateHelper.GetNextGoodFriday;
            //state["NextHosannaSunday"] = (Func<LocalDate>)CopticDateHelper.GetNextHosannaSunday;
            state["NextLazarusSaturday"] = (Func<LocalDate>)CopticDateHelper.GetNextLazarusSaturday;
            //state["NextNativity"] = (Func<LocalDate>)CopticDateHelper.GetNextNativity;
            //state["NextNativityFast"] = (Func<LocalDate>)CopticDateHelper.GetNextNativityFast;
            //state["NextNativitySunday"] = (Func<LocalDate>)CopticDateHelper.GetNextNativitySunday;
            //state["NextPascha"] = (Func<LocalDate>)CopticDateHelper.GetNextPascha;

            var scriptResult = state.DoString(scriptBody)?.FirstOrDefault();
            state.Close();
            return scriptResult as IDefinition;

            if (scriptResult is IDefinition defResult)
                return defResult;
            else
                throw new InvalidCastException($"Expected an IDefinition, but script returned {scriptResult.GetType().Name}");
        }

        /// <summary>
        /// Parses inline text commands from an <see cref="InlineCollection"/>.
        /// </summary>
        /// <param name="contentInlines">The <see cref="Inline"/>s to parse.</param>
        /// <exception cref="ArgumentException">Mismatched brackets were detected.</exception>
        public static InlineCollection ParseTextCommands(InlineCollection contentInlines)
        {
            InlineCollection parsedInlines = new();

            for (int i = 0; i < contentInlines.Count; i++)
            {
                var inline = contentInlines[i];
                parsedInlines.Append(ParseTextCommands(inline));
            }

            return parsedInlines;
        }

        /// <summary>
        /// Parses inline text commands from a single <see cref="Inline"/>.
        /// </summary>
        /// <param name="inline">The content to parse.</param>
        /// <exception cref="ArgumentException">Mismatched brackets were detected.</exception>
        public static Inline ParseTextCommands(Inline inline)
        {
            switch (inline)
            {
                case Run run:
                    return ParseTextCommands(run.Text.AsSpan(), run.Parent);

                case Span span:
                    InlineCollection parsedSpanInlines = new();
                    foreach (Inline spanInline in span.Inlines)
                        parsedSpanInlines.Add(ParseTextCommands(spanInline));
                    return new Span(parsedSpanInlines, inline.Parent);

                default:
                    throw new ArgumentException($"Cannot parse commands from an Inline of type '{inline.GetType()}'.");
            }
        }

        /// <summary>
        /// Parses inline text commands from a plain string.
        /// </summary>
        /// <param name="text">
        /// The text to parse.
        /// </param>
        /// <param name="parent">
        /// The parent this text is associated with.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Mismatched brackets were detected.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// An invalid internal state was encountered.
        /// </exception>
        public static Inline ParseTextCommands(ReadOnlySpan<char> text, IDefinition parent)
        {
            InlineCollection inlines = new();

            int cmdStart = -1;
            bool isEscaped = false;
            StringBuilder plain = new(text.Length);
            for (int index = 0; index < text.Length; index++)
            {
                char ch = text[index];
                if (ch == '\\')
                {
                    if (index + 1 < text.Length)
                    {
                        // Check if this is an escape sequence
                        char nextCh = text[index + 1];
                        isEscaped = nextCh == '\\' || nextCh == '{' || nextCh == '}';
                        if (isEscaped)
                        {
                            // Received escape signal '\\'
                            continue;
                        }

                        // else, this is the start of a command
                        cmdStart = index;

                        // Save everything before the command
                        if (plain.Length > 0)
                        {
                            inlines.Add(new Run(plain.ToString(), parent));
                            plain.Clear();
                        }
                    }
                    else
                    {
                        plain.Append(ch);
                    }
                }
                else if (ch == '{' && !isEscaped)
                {
                    int openIndex = index;
                    int closeIndex = IndexOfClosingBrace(text, openIndex);

                    // Skip to end of command
                    index = closeIndex;

                    // If there was no command name specified, just skip
                    // everything within the braces.
                    if (cmdStart < 0)
                        continue;

                    if (closeIndex < 0)
                        throw new ArgumentException($"Mismatched opening bracket at index {openIndex}");
                    else if (closeIndex <= openIndex || closeIndex >= text.Length)
                        throw new InvalidOperationException($"Index of closing brace was not valid.");

                    // Get inner string. Note that (close - open) gives the length including the closing brace.
                    var innerText = text.Slice(openIndex + 1, closeIndex - openIndex - 1);

                    var name = plain.ToString();
                    plain.Clear();
                    InlineCommand inCmd = new(name, parent);

                    // Split and parse command parameters
                    ParseTextCommandParameters(innerText, inCmd);

                    inlines.Add(inCmd);
                }
                else if (ch == '}' && !isEscaped)
                {
                    throw new ArgumentException($"Mismatched end bracket at index {index}");
                }
                else
                {
                    if (ch == ' ' && cmdStart >= 0)
                    {
                        cmdStart = -1;
                    }

                    // Regular character
                    plain.Append(ch);
                }

                isEscaped = false;
            }

            // Ensure plain text that doesn't come after a command is included
            if (plain.Length > 0)
                inlines.Add(new Run(plain.ToString(), parent));

            return inlines.Count == 1
                ? inlines[0]
                : new Span(inlines, parent);
        }

        public static List<TextCommandBase> RunTextCommands(Inline inline)
        {
            List<TextCommandBase> cmds = new();
            RunTextCommands(inline, cmds);
            return cmds;
        }

        public static void RunTextCommands(Inline inline, in ICollection<TextCommandBase> cmds)
        {
            switch (inline)
            {
                case InlineCommand inCmd:
                    // Ensure each parameter is fully evaluated
                    foreach (Inline paramInline in inCmd.Parameters)
                        RunTextCommands(paramInline, cmds);

                    // Evaluate the current command
                    // If the command was already executed, just return it
                    if (inCmd.Command == null)
                    {
                        // Get each parameter, ensuring that the output of nested commands are used.
                        IDefinition[] parameters = new IDefinition[inCmd.Parameters.Count];
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            var param = inCmd.Parameters[i];
                            if (param is InlineCommand paramInCmd && paramInCmd.Command?.Output != null)
                                parameters[i] = paramInCmd.Command.Output;
                            else
                                parameters[i] = param;
                        }

                        inCmd.Command = GetCommand(inCmd, parameters);
                    }
                    
                    cmds.Add(inCmd.Command);
                    break;

                case Span span:
                    foreach (Inline spanInline in span.Inlines)
                        RunTextCommands(spanInline, cmds);
                    break;

                // Other inline types, such as Run, don't
                // need to be evaluated.
            }
        }

        private static TextCommandBase GetCommand(InlineCommand inline, IDefinition[] parameters)
        {
            if (_availCmds.TryGetValue(inline.CommandName, out var type))
                return Activator.CreateInstance(type, inline.CommandName, inline, parameters) as TextCommandBase;
            return null;
        }

        /// <summary>
        /// Finds the index of the closing curly bracket for the
        /// opening bracket at the <paramref name="openIndex"/>.
        /// </summary>
        /// <param name="strSpan">
        /// The string to search.
        /// </param>
        /// <param name="openIndex">
        /// The index of the opening brace.
        /// </param>
        /// <returns></returns>
        private static int IndexOfClosingBrace(ReadOnlySpan<char> strSpan, int openIndex = 0)
        {
            int openCount = 0;

            int i = openIndex;
            while (i < strSpan.Length)
            {
                char ch = strSpan[i];
                bool isEscaped = i >= 1 && strSpan[i - 1] == '\\';

                if (!isEscaped)
                {
                    if (ch == '{')
                        openCount++;
                    else if (ch == '}')
                        openCount--;

                    // Net equals 0 if there were the
                    // same number of opening and closing braces
                    if (openCount == 0)
                        return i;
                }

                i++;
            }

            return -1;
        }

        private static void ParseTextCommandParameters(ReadOnlySpan<char> innerText, InlineCommand cmd)
        {
            cmd.Parameters = new();

            if (innerText.Length <= 0)
                return;

            List<int> separatorIndexes = (-1).IntoList();
            
            // Find the index of each top-level parameter separator
            for (int i = 0; i < innerText.Length; i++)
            {
                ReadOnlySpan<char> remainingText = innerText.Slice(i);
                int nextSeparator = IndexOfFirstNonEscaped(remainingText, '|');
                int nextOpen = IndexOfFirstNonEscaped(remainingText, '{');

                if (nextSeparator < 0)
                    break;

                if (nextOpen < 0 || nextOpen > nextSeparator)
                {
                    // There aren't any more commands within the current parameter
                    separatorIndexes.Add(i += nextSeparator);
                }
                else// if (nextOpen > -1)
                {
                    // There is a nested command, which might also contain parameters.
                    // Make sure to skip over it.
                    i += IndexOfClosingBrace(remainingText, nextOpen);
                }
            }

            // Ensure the last parameter is captured
            if (separatorIndexes.Count == 0 || separatorIndexes[separatorIndexes.Count - 1] < innerText.Length - 1)
                separatorIndexes.Add(innerText.Length);

            for (int i = 0; i < separatorIndexes.Count - 1; i++)
            {
                int sep = separatorIndexes[i];
                int nextSep = separatorIndexes[i + 1];

                var paramText = innerText.Slice(sep + 1, nextSep - sep - 1);

                // Recursively parse inner text
                var parsedParam = ParseTextCommands(paramText, cmd);
                cmd.Parameters.Add(parsedParam);
            }
        }

        private static int IndexOfFirstNonEscaped(ReadOnlySpan<char> text, char value)
        {
            int i = 0;
            while ((i += text.Slice(i).IndexOf(value)) > 0)
            {
                if (i < 1 || text[i - 1] != '\\')
                    break;
                else
                    i++;
            }
            return i;
        }
    }
}
