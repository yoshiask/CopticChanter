using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CoptLib.XML;
using NLua;

namespace CoptLib
{
    public class Scripting
    {
        public static IDictionary<string, bool> GetArgs(DateTime date)
        {
            if (date == new DateTime())
                date = DateTime.Today;
            var args = new Dictionary<string, bool>();

            // Check if today is the Feast of the Nativity, always Jan. 7th [Gregorian]
            if (date == CopticDate.GetNextNativity(date))
            {
                args.Add("Nativity", true);
            }
            else
            {
                args.Add("Nativity", false);
            }

            // Check if today is the Sunday before the Feast of the Nativity
            if (date == CopticDate.GetNextNativitySunday(date))
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
            if (date >= CopticDate.GetNextHosannaSunday(date) && date < CopticDate.GetNextPascha(date))
            {
                args.Add("Holy Week", true);
            }
            else
            {
                args.Add("Holy Week", false);
            }

            // Check if today is Palm Sunday
            if (date == CopticDate.GetNextHosannaSunday(date))
            {
                args.Add("Palm Sunday", true);
            }
            else
            {
                args.Add("Palm Sunday", false);
            }

            // Check if today is Pascha
            if (date == CopticDate.GetNextPascha(date))
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

        public static string RunScript(IfXml script, IfXml parentIf, DocXml parentDoc)
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
        public static string RunScript(string xml, DocXml parentDoc)
        {
            return RunScript(IfXml.FromString(xml), null, parentDoc);
        }

        private static string ReturnHandler(bool boolean, IfXml script, IfXml parentIf, DocXml parentDoc)
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
                        return GetArgs(DateTime.Today)[ds2];

                    case "!=":
                        return !GetArgs(DateTime.Today)[ds2];

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
            state["NextCovenantThursday"] = (Func<long>)CopticDate.GetNextCovenantThursday;
            state["NextFeastResurrection"] = (Func<long>)CopticDate.GetNextFeastResurrection;
            state["NextGoodFriday"] = (Func<long>)CopticDate.GetNextGoodFriday;
            state["NextHosannaSunday"] = (Func<long>)CopticDate.GetNextHosannaSunday;
            state["NextLazarusSaturday"] = (Func<long>)CopticDate.GetNextLazarusSaturday;
            state["NextNativity"] = (Func<long>)CopticDate.GetNextNativity;
            state["NextNativityFast"] = (Func<long>)CopticDate.GetNextNativityFast;
            state["NextNativitySunday"] = (Func<long>)CopticDate.GetNextNativitySunday;
            state["NextPascha"] = (Func<long>)CopticDate.GetNextPascha;
            state["NextSpringEquinox"] = (Func<long>)CopticDate.GetNextSpringEquinox;

            try
            {
                state.DoString(scriptCode);
                var script = state["GetNext"] as LuaFunction;

                if (script.Call()[0] is string res)
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

        public static void ParseTextCommands(string input)
        {
            // Define a regular expression for repeated words.
            Regex rx = new Regex(@"(\\)(?<command>\w+)(\{)(?<param>\w+)*(\})",
              RegexOptions.Compiled | RegexOptions.IgnoreCase);

            // Find matches.
            MatchCollection matches = rx.Matches(input);

            // Report the number of matches found.
            Console.WriteLine("{0} matches found in:\n   {1}",
                              matches.Count,
                              input);
            foreach (Match m in matches)
            {
                Console.WriteLine($"\tCommand: {m.Groups["command"]}\r\n\tParameters: {m.Groups["params"]}\r\n");
            }
        }
    }
}
