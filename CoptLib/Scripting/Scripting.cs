using System;
using System.Collections.Generic;
using CoptLib.Models;
using CoptLib.Scripting.Commands;
using NLua;
using NodaTime;

namespace CoptLib.Scripting
{
    public class Scripting
    {
        private static Dictionary<string, Type> _availCmds;

        public static IDictionary<string, bool> GetArgs(LocalDate date)
        {
            if (date == null)
                date = CopticDateHelper.TodayCoptic;
            var args = new Dictionary<string, bool>();

            // Check if today is the Feast of the Nativity, always Jan. 7th [Gregorian]
            if (date == CopticDateHelper.GetNextNativity(date))
            {
                args.Add("Nativity", true);
            }
            else
            {
                args.Add("Nativity", false);
            }

            // Check if today is the Sunday before the Feast of the Nativity
            if (date == CopticDateHelper.GetNextNativitySunday(date))
            {
                args.Add("Nativity Sunday", true);
            }
            else
            {
                args.Add("Nativity Sunday", false);
            }

            // Check if today is during Great Lent
            /*if (date >= CopticDate.GreatLentStart && date < CopticDate.PalmSunday)
            {
                args.Add("Great Lent", true);
            }
            else
            {
                args.Add("Great Lent", false);
            }*/

            // Check if today is during Holy Week
            if (date >= CopticDateHelper.GetNextHosannaSunday(date) && date < CopticDateHelper.GetNextPascha(date))
            {
                args.Add("Holy Week", true);
            }
            else
            {
                args.Add("Holy Week", false);
            }

            // Check if today is Palm Sunday
            if (date == CopticDateHelper.GetNextHosannaSunday(date))
            {
                args.Add("Palm Sunday", true);
            }
            else
            {
                args.Add("Palm Sunday", false);
            }

            // Check if today is Pascha
            if (date == CopticDateHelper.GetNextPascha(date))
            {
                args.Add("Pascha", true);
                args.Add("Easter", true);
            }
            else
            {
                args.Add("Pascha", false);
                args.Add("Easter", false);
            }

            return args;
        }

        public static string RunScript(IfXml script, IfXml parentIf, Doc parentDoc)
        {
            var type = script.LeftHand.Split(':')[0];
            if (type == script.RightHand.Split(':')[0])
            {
                switch (type)
                {
                    case "int":
                        try
                        {
                            int int1 = Convert.ToInt32(script.LeftHand.Split(':')[1]);
                            int int2 = Convert.ToInt32(script.RightHand.Split(':')[1]);
                            return ReturnHandler(PerformOperation(int1, int2, script.Comparator), script, parentIf, parentDoc);
                        }
                        catch (Exception ex)
                        {
                            return "error:" + ex.Message;
                        }

                    case "bool":
                        try
                        {
                            Boolean.TryParse(script.LeftHand.Split(':')[1], out bool b1);
                            Boolean.TryParse(script.RightHand.Split(':')[1], out bool b2);
                            return ReturnHandler(PerformOperation(b1, b2, script.Comparator), script, parentIf, parentDoc);
                        }
                        catch (Exception ex)
                        {
                            return "error:" + ex.Message;
                        }

                    case "date":
                        try
                        {
                            string ds1 = script.LeftHand.Split(':')[1];
                            string ds2 = script.RightHand.Split(':')[1];

                            if (ds1 == "today" && !ds2.Contains("/"))
                            {
                                return ReturnHandler(PerformOperation(ds1, ds2, script.Comparator, true), script, parentIf, parentDoc);
                            }
                            else
                            {
                                return ReturnHandler(PerformOperation(ds1, ds2, script.Comparator, false),
                                                                    script, parentIf, parentDoc);
                            }
                        }
                        catch (Exception ex)
                        {
                            return "error:" + ex.Message;
                        }

                    default:
                        return "error:Type not recognized";
                }
            }
            else
            {
                return "error:Types are different";
            }
        }
        public static string RunScript(string xml, Doc parentDoc)
        {
            return RunScript(IfXml.FromString(xml), null, parentDoc);
        }

        private static string ReturnHandler(bool boolean, IfXml script, IfXml parentIf, Doc parentDoc)
        {
            if (boolean)
            {
                if (script.If != null)
                {
                    return RunScript(script.If, script, null);
                }
                else if (Guid.TryParse(script.Return, out Guid next))
                {
                    return next.ToString();
                }
                else
                {
                    return "error:\"Work\" is not a valid GUID";
                }
            }
            else
            {
                if (parentIf != null)
                {
                    if (parentIf.Return != "")
                    {
                        return parentIf.Return;
                    }
                }
                // TODO: Don't do this
                return "";
            }
        }

        #region Operation Helpers
        /// <summary>
        /// Performs a comparison of two intergers using the specified operator
        /// </summary>
        public static bool PerformOperation(int i1, int i2, string op)
        {
            switch (op)
            {
                // Less than
                case "lth":
                    return i1 < i2;

                // Less than or equal to
                case "lth=":
                    return i1 <= i2;

                // Greater than
                case "gth":
                    return i1 > i2;

                // Greater than or equal to
                case "gth=":
                    return i1 >= i2;

                // Equal to
                case "==":
                    return i1 == i2;

                // Not equal to
                case "!=":
                    return i1 != i2;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Performs a comparison of two booleans using the specified operator
        /// </summary>
        public static bool PerformOperation(bool b1, bool b2, string op)
        {
            switch (op)
            {
                // Equal to
                case "==":
                    return b1 == b2;

                // Not equal to
                case "!=":
                    return b1 != b2;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Performs a comparison of two dates using the specified operator
        /// </summary>
        public static bool PerformOperation(string ds1, string ds2, string op, bool isSpecial)
        {
            if (!isSpecial)
            {
                if (DateTime.TryParse(ds1, out DateTime d1) && DateTime.TryParse(ds2, out DateTime d2))
                {
                    switch (op)
                    {
                        // Less than
                        case "lth":
                            return d1 < d2;

                        // Less than or equal to
                        case "lth=":
                            return d1 <= d2;

                        // Greater than
                        case "gth":
                            return d1 > d2;

                        // Greater than or equal to
                        case "gth=":
                            return d1 >= d2;

                        // Equal to
                        case "==":
                            return d1 == d2;

                        // Not equal to
                        case "!=":
                            return d1 != d2;

                        default:
                            return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                // ds1 is Today
                // ds2 is Special Event

                switch (op)
                {
                    case "==":
                        return GetArgs(CopticDateHelper.TodayCoptic)[ds2];

                    case "!=":
                        return !GetArgs(CopticDateHelper.TodayCoptic)[ds2];

                    default:
                        return false;
                }
            }
        }
        #endregion

        public static string RunLuaScript(string scriptBody)
        {
            string scriptCode = "function GetNext()\n" + scriptBody + "\nend";
            Lua state = new Lua();

            // Add the CoptLib date functions
            state["Today"] = DateTime.Today.Ticks;
            //state["NextCovenantThursday"] = (Func<LocalDate>)CopticDateHelper.GetNextCovenantThursday;
            //state["NextFeastResurrection"] = (Func<LocalDate>)CopticDateHelper.GetNextFeastResurrection;
            //state["NextGoodFriday"] = (Func<LocalDate>)CopticDateHelper.GetNextGoodFriday;
            //state["NextHosannaSunday"] = (Func<LocalDate>)CopticDateHelper.GetNextHosannaSunday;
            //state["NextLazarusSaturday"] = (Func<LocalDate>)CopticDateHelper.GetNextLazarusSaturday;
            //state["NextNativity"] = (Func<LocalDate>)CopticDateHelper.GetNextNativity;
            //state["NextNativityFast"] = (Func<LocalDate>)CopticDateHelper.GetNextNativityFast;
            //state["NextNativitySunday"] = (Func<LocalDate>)CopticDateHelper.GetNextNativitySunday;
            //state["NextPascha"] = (Func<LocalDate>)CopticDateHelper.GetNextPascha;
            //state["NextSpringEquinox"] = (Func<LocalDate>)CopticDateHelper.GetNextSpringEquinox;

            try
            {
                state.DoString(scriptCode);
                var script = state["GetNext"] as LuaFunction;

                if (script?.Call()[0] is string res)
                {
                    return res;
                }
                else
                {
                    throw new InvalidCastException("Invalid return type");
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
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
                if (ch == '\\')
                {
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
                    string[] parameters = strippedText.Substring(start + 1, index - start - 1).Split('|');

                    var parsedCmd = GetCommand(name, content, start, parameters);
                    if (parsedCmd == null)
                        continue;

                    if (parsedCmd.Text != null)
                    {
                        int cmdLength = index - cmdStart + 1;
                        strippedText = strippedText.Remove(cmdStart, cmdLength);

                        if (parsedCmd.Text != string.Empty)
                            strippedText = strippedText.Insert(cmdStart, parsedCmd.Text);

                        // Make sure to update the current index
                        index += parsedCmd.Text.Length - cmdLength;
                    }

                    parsedCmds.Add(parsedCmd);
                }
            }

            if (paramStartPositions.Count > 0)
                throw new ArgumentException($"Mismatched start brackets, {paramStartPositions.Count} total");

            return parsedCmds;
        }

        private static TextCommandBase GetCommand(string cmd, IContent content, int startIndex, string[] parameters)
        {
            PopulateAvailableCommands();

            if (_availCmds.TryGetValue(cmd, out var type))
                return Activator.CreateInstance(type, cmd, content, startIndex, parameters) as TextCommandBase;
            return null;
        }

        private static void PopulateAvailableCommands()
        {
            if (_availCmds != null && _availCmds.Count > 0)
                return;

            _availCmds = new Dictionary<string, Type>
            {
                { "def", typeof(DefinitionCmd) },
                { "ipa", typeof(IpaTranscribeCmd) },
                { "language", typeof(LanguageCmd) },
                { "lang", typeof(LanguageCmd) },
                { "ms", typeof(TimestampCmd) },
            };
        }
    }
}
