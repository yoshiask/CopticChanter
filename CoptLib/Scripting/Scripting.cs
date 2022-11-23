using System;
using System.Collections.Generic;
using System.Linq;
using CoptLib.Models;
using CoptLib.Scripting.Commands;
using NLua;
using NodaTime;

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

        public static List<TextCommandBase> ParseTextCommands(IContent content, out string strippedText)
        {
            strippedText = content.SourceText;

            // Create a list to store parsed commands
            List<TextCommandBase> parsedCmds = new();
            Stack<int> paramStartPositions = new();
            Stack<int> cmdStartPositions = new();
            for (int index = 0; index < strippedText.Length; index++)
            {
                char ch = strippedText[index];
                if (ch == '\\' && index + 1 < strippedText.Length)
                {
                    // Check if this is an escape sequence
                    char nextCh = strippedText[index + 1];
                    if (nextCh == '\\' || nextCh == '{' || nextCh == '}')
                    {
                        // Remove escape signal '\\'
                        strippedText = strippedText.Remove(index, 1);
                        continue;
                    }

                    // else, this is the start of a command
                    cmdStartPositions.Push(index);
                }
                else if (ch == ' ' && cmdStartPositions.Count > paramStartPositions.Count)
                {
                    cmdStartPositions.Pop();
                }
                else if (ch == '{')
                {
                    paramStartPositions.Push(index);
                }
                else if (ch == '}')
                {
                    if (paramStartPositions.Count == 0)
                        throw new ArgumentException($"Mismatched end bracket at index {index}");

                    var depth = paramStartPositions.Count - 1;
                    var start = paramStartPositions.Pop();

                    // Ignore only opening and closing brackets
                    if (cmdStartPositions.Count == 0)
                        continue;
                    var cmdStart = cmdStartPositions.Pop();

                    string name = strippedText.Substring(cmdStart + 1, start - cmdStart - 1);

                    // Make sure all inputs are IDefinitions
                    List<IDefinition> parameters = new();
                    string paramText = strippedText.Substring(start + 1, index - start - 1);
                    int pStartIdx = -1;
                    int pNextStartIdx = paramText.IndexOf('|');
                    do
                    {
                        IDefinition par;

                        int pActStartIdx = cmdStart + start + ++pStartIdx + 1;
                        var cmd = parsedCmds.FirstOrDefault(c => c.StartIndex == pActStartIdx);
                        if (cmd != null)
                        {
                            par = cmd.Output;
                        }
                        else
                        {
                            int pEndIdx = pNextStartIdx >= 0 ? pNextStartIdx : paramText.Length;
                            string text = paramText.Substring(pStartIdx, pEndIdx - pStartIdx);
                            par = new SimpleContent(text, content);
                        }

                        parameters.Add(par);
                        pStartIdx = pNextStartIdx;
                        if (pStartIdx < 0)
                            break;
                        pNextStartIdx = paramText.IndexOf('|', pStartIdx + 1);
                    } while (pStartIdx >= 0);

                    var parsedCmd = GetCommand(name, content, cmdStart, parameters.ToArray());
                    if (parsedCmd == null)
                        continue;

                    int cmdLength = index - cmdStart + 1;
                    strippedText = strippedText.Remove(cmdStart, cmdLength);
                    if (parsedCmd.Output is IContent outputContent)
                    {
                        if (outputContent.Text != string.Empty)
                            strippedText = strippedText.Insert(cmdStart, outputContent.Text);

                        // Make sure to update the current index
                        index += outputContent.Text.Length - cmdLength;
                    }
                    else
                    {
                        // Strip out the command text
                        index = cmdStart - 1;
                    }

                    parsedCmds.Add(parsedCmd);
                }
            }

            if (paramStartPositions.Count > 0)
                throw new ArgumentException($"Mismatched start brackets, {paramStartPositions.Count} total");

            return parsedCmds;
        }

        private static TextCommandBase GetCommand(string cmd, IContent content, int startIndex, IDefinition[] parameters)
        {
            if (_availCmds.TryGetValue(cmd, out var type))
                return Activator.CreateInstance(type, cmd, content, startIndex, parameters) as TextCommandBase;
            return null;
        }
    }
}
