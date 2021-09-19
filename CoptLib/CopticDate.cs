using System;
using System.Collections.Generic;

namespace CoptLib
{
    public class CopticDate : IComparable, IComparable<CopticDate>, IEquatable<CopticDate>, IComparable<DateTime>, IEquatable<DateTime>
    {
        #region Properties
        private CopticMonth _month;
        private int _day;

        public int Year
        {
            get;
            internal set;
        }
        public int GregYear => Year + 284;

        public int Day
        {
            get => _day;
            internal set
            {
                _day = value;
            }
        }

        public CopticMonth Month
        {
            get => _month;
            internal set
            {
                _month = value;
            }
        }

        public DayOfWeek DayOfWeek
        {
            get;
        }

        public int WeekNumber
        {
            get;
        }

        public long Ticks
        {
            get;
            private set;
        }
        #endregion

        #region Static Dates
        public static CopticDate Today
        {
            get
            {
                return CopticDate.ToCopticDate(DateTime.Today);
            }
        }

        public static DateTime GetNextSpringEquinox(DateTime date)
        {
            var equinox = CalcSpringEquinox(date.Year);
            if (equinox < date)
                return equinox.AddYears(1);
            else
                return equinox;
        }
        public static long GetNextSpringEquinox()
        {
            return GetNextSpringEquinox(DateTime.Now).Ticks;
        }
        public static DateTime CalcSpringEquinox(int year)
        {
            return new DateTime(year, 3, 20);
        }

        public static DateTime GetNextFeastResurrection(DateTime date)
        {
            var palmSunday = CalcFeastResurrection(date.Year);

            if (palmSunday < date)
                return CalcFeastResurrection(date.Year + 1);
            else
                return palmSunday;
        }
        public static long GetNextFeastResurrection()
        {
            return GetNextFeastResurrection(DateTime.Now).Ticks;
        }
        public static DateTime CalcFeastResurrection(int year)
        {
            // From http://www.copticchurch.net/topics/coptic_calendar/ortheast.html,
            // returns Easter Day on the Julian Calendar
            int g = year % 19;
            int I = (19 * g + 15) % 30;
            int j = (year + year / 4 + I) % 7;
            int l = I - j;
            int easterMonth = 3 + (l + 40) / 44;
            int easterDay = l + 28 - 31 * (easterMonth / 4);

            // Convert the output above to the Gregorian calendar
            // Accurate to 2100
            var julDate = new DateTime(year, easterMonth, easterDay);
            return julDate.AddDays(13);
        }
        public static List<DateTime> PalmSundayDays
        {
            get
            {
                var list = new List<DateTime>();
                for (int i = 1; i <= 2100; i++)
                {
                    list.Add(new DateTime());
                }
                list[2014] = new DateTime(2014, 4, 13);
                list[2015] = new DateTime(2015, 3, 29);
                list[2016] = new DateTime(2014, 3, 20);
                list[2017] = new DateTime(2017, 4, 9);
                list[2018] = new DateTime(2018, 3, 25);
                list[2019] = new DateTime(2019, 4, 14);
                list[2020] = new DateTime(2020, 4, 5);
                list[2021] = new DateTime(2021, 3, 28);
                list[2022] = new DateTime(2022, 4, 10);
                list[2023] = new DateTime(2023, 4, 2);
                list[2024] = new DateTime(2024, 3, 24);
                list[2025] = new DateTime(2025, 4, 13);
                list[2026] = new DateTime(2026, 3, 29);
                list[2027] = new DateTime(2027, 3, 21);
                list[2028] = new DateTime(2028, 4, 9);
                list[2029] = new DateTime(2029, 3, 25);
                list[2030] = new DateTime(2030, 4, 14);

                return list;
            }
        }

        public static DateTime GetNextLazarusSaturday(DateTime date)
        {
            var lazarusSaturday = CalcLazarusSaturday(date.Year);

            if (lazarusSaturday < date)
                return CalcLazarusSaturday(date.Year + 1);
            else
                return lazarusSaturday;
        }
        public static long GetNextLazarusSaturday()
        {
            return GetNextLazarusSaturday(DateTime.Now).Ticks;
        }
        public static DateTime CalcLazarusSaturday(int year)
        {
            return CalcFeastResurrection(year).AddDays(-8);
        }

        public static DateTime GetNextHosannaSunday(DateTime date)
        {
            var hosannaSunday = CalcHosannaSunday(date.Year);

            if (hosannaSunday < date)
                return CalcHosannaSunday(date.Year + 1);
            else
                return hosannaSunday;
        }
        public static long GetNextHosannaSunday()
        {
            return GetNextHosannaSunday(DateTime.Now).Ticks;
        }
        public static DateTime CalcHosannaSunday(int year)
        {
            return CalcFeastResurrection(year).AddDays(-7);
        }

        public static DateTime GetNextPascha(DateTime date)
        {
            var holyPascha = CalcPascha(date.Year);

            if (holyPascha < date)
                return CalcPascha(date.Year + 1);
            else
                return holyPascha;
        }
        public static long GetNextPascha()
        {
            return GetNextPascha(DateTime.Now).Ticks;
        }
        public static DateTime CalcPascha(int year)
        {
            return CalcFeastResurrection(year).AddDays(-6);
        }

        public static DateTime GetNextCovenantThursday(DateTime date)
        {
            var covenantThursday = CalcCovenantThursday(date.Year);

            if (covenantThursday < date)
                return CalcCovenantThursday(date.Year + 1);
            else
                return covenantThursday;
        }
        public static long GetNextCovenantThursday()
        {
            return GetNextCovenantThursday(DateTime.Now).Ticks;
        }
        public static DateTime CalcCovenantThursday(int year)
        {
            return CalcFeastResurrection(year).AddDays(-3);
        }

        public static DateTime GetNextGoodFriday(DateTime date)
        {
            var goodFriday = CalcGoodFriday(date.Year);

            if (goodFriday < date)
                return CalcGoodFriday(date.Year + 1);
            else
                return goodFriday;
        }
        public static long GetNextGoodFriday()
        {
            return GetNextGoodFriday(DateTime.Now).Ticks;
        }
        public static DateTime CalcGoodFriday(int year)
        {
            return CalcFeastResurrection(year).AddDays(-2);
        }

        public static DateTime GetNextNativityFast(DateTime date)
        {
            var nativityFast = CalcNativityFast(date.Year);

            if (nativityFast < date)
                return CalcNativityFast(date.Year + 1);
            else
                return nativityFast;
        }
        public static long GetNextNativityFast()
        {
            return GetNextNativityFast(DateTime.Now).Ticks;
        }
        public static DateTime CalcNativityFast(int year)
        {
            return CalcNativity(year).AddDays(6 * 7);
        }

        public static DateTime GetNextNativity(DateTime date)
        {
            var nativity = CalcNativity(date.Year);

            if (nativity < date)
                return CalcNativity(date.Year + 1);
            else
                return nativity;
        }
        public static long GetNextNativity()
        {
            return GetNextNativity(DateTime.Now).Ticks;
        }
        public static DateTime CalcNativity(int year)
        {
            return new DateTime(year, 1, 7);
        }

        public static DateTime GetNextNativitySunday(DateTime date)
        {
            var nativitySunday = CalcNativitySunday(date.Year);

            if (nativitySunday < date)
                return CalcNativitySunday(date.Year + 1);
            else
                return nativitySunday;
        }
        public static long GetNextNativitySunday()
        {
            return GetNextNativitySunday(DateTime.Now).Ticks;
        }
        public static DateTime CalcNativitySunday(int year)
        {
            DateTime nativitySunday = CalcNativity(year);
            while (nativitySunday.DayOfWeek != DayOfWeek.Sunday)
            {
                nativitySunday.AddDays(-1);
            }
            return nativitySunday;
        }
        #endregion

        public CopticDate(int year, int month, int day)
        {
            Year = year;
            Month = new CopticMonth(month);
            Day = day;
        }

        public CopticDate(int year, CopticMonth month, int day)
        {
            Year = year;
            Month = month;
            Day = day;
        }

        public CopticDate(DateTime eDate)
        {
            var cDate = CopticDate.ToCopticDate(eDate);
            Year = cDate.Year;
            Month = cDate.Month;
            Day = cDate.Day;
        }

        public void SetMonth(CopticMonth month)
        {
            Month = month;
        }

        public void SetDay(int day)
        {
            Day = day;
        }

        public void SetYear(int year)
        {
            Year = year;
        }

        public CopticDate AddDays(int days)
        {
            Day += days;
            return this;
        }

        int IComparable.CompareTo(object compare)
        {
            CopticDate other;
            try
            {
                other = (CopticDate)compare;
            }
            catch
            {
                return 1;
            }

            // If other is not a valid object reference, this instance is greater.
            if (other == null) return 1;

            if (Year > other.Year)
            {
                return -1;
            }
            else if (Month > other.Month)
            {
                return -1;
            }
            else if (Day > other.Day)
            {
                return -1;
            }
            else if (Year == other.Year && Month == other.Month && Day == other.Day)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        int IComparable<CopticDate>.CompareTo(CopticDate other)
        {
            // If other is not a valid object reference, this instance is greater.
            if (other == null) return 1;

            if (Year > other.Year)
            {
                return -1;
            }
            else if (Month > other.Month)
            {
                return -1;
            }
            else if (Day > other.Day)
            {
                return -1;
            }
            else if (Year == other.Year && Month == other.Month && Day == other.Day)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        int IComparable<DateTime>.CompareTo(DateTime eDate)
        {
            var other = CopticDate.ToCopticDate(eDate);
            // If other is not a valid object reference, this instance is greater.
            if (other == null) return 1;

            if (Year > other.Year)
            {
                return -1;
            }
            else if (Month > other.Month)
            {
                return -1;
            }
            else if (Day > other.Day)
            {
                return -1;
            }
            else if (Year == other.Year && Month == other.Month && Day == other.Day)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        bool IEquatable<CopticDate>.Equals(CopticDate other)
        {
            if (other.Year == Year && other.Month == Month && other.Day == Day)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool IEquatable<DateTime>.Equals(DateTime eDate)
        {
            var other = CopticDate.ToCopticDate(eDate);
            return Equals(other);
        }

        public override bool Equals(Object obj)
        {
            CopticDate other = obj as CopticDate;
            if (other == null)
                return false;
            else
                return other.Equals(this);
        }

        public override int GetHashCode()
        {
            return this.Day.GetHashCode();
        }

        public override string ToString()
        {
            string output = "";
            output += Day.ToString();
            output += " ";
            output += Month.EnglishName.ToString();
            output += ", ";
            output += Year.ToString();

            return output;
        }

        public static CopticDate ToCopticDate(DateTime eDate)
        {
            var cDate = new CopticDate(eDate.Year, eDate.Month, eDate.DayOfYear);
            cDate.Ticks = eDate.Ticks;

            // Calculate Coptic month & day from eDate.DayOfYear
            int day = eDate.DayOfYear;
            if (!IsLeapYear(eDate))
            {
                // Month of Tout
                if (day >= 254 && day < 284)
                {
                    cDate.SetMonth(new CopticMonth(1));
                    cDate.SetDay(day - 253);
                }

                // Month of Baba
                else if (day >= 284 && day < 314)
                {
                    cDate.SetMonth(new CopticMonth(2));
                    cDate.SetDay(day - 283);
                }

                // Month of Hator
                else if (day >= 314 && day < 344)
                {
                    cDate.SetMonth(new CopticMonth(3));
                    cDate.SetDay(day - 313);
                }

                // Month of Kiahk
                else if (day >= 344 && day <= 365)
                {
                    cDate.SetMonth(new CopticMonth(4));
                    cDate.SetDay(day - 343);
                }
                else if (day >= 1 && day < 9)
                {
                    cDate.SetMonth(new CopticMonth(4));
                    // TODO: Calculate days to subtract to get day of month
                    cDate.SetDay(day + 22);
                }

                // Month of Toba
                else if (day >= 9 && day < 39)
                {
                    cDate.SetMonth(new CopticMonth(5));
                    cDate.SetDay(day - 8);
                }

                // Month of Amshir
                else if (day >= 39 && day < 69)
                {
                    cDate.SetMonth(new CopticMonth(6));
                    cDate.SetDay(day - 38);
                }

                // Month of Baramhat
                else if (day >= 69 && day < 99)
                {
                    cDate.SetMonth(new CopticMonth(7));
                    cDate.SetDay(day - 68);
                }

                // Month of Baramouda
                else if (day >= 99 && day < 129)
                {
                    cDate.SetMonth(new CopticMonth(8));
                    cDate.SetDay(day - 98);
                }

                // Month of Bashans
                else if (day >= 129 && day < 159)
                {
                    cDate.SetMonth(new CopticMonth(9));
                    cDate.SetDay(day - 128);
                }

                // Month of Paona
                else if (day >= 159 && day < 189)
                {
                    cDate.SetMonth(new CopticMonth(10));
                    cDate.SetDay(day - 158);
                }

                // Month of Epep
                else if (day >= 189 && day < 219)
                {
                    cDate.SetMonth(new CopticMonth(11));
                    cDate.SetDay(day - 188);
                }

                // Month of Mesra
                else if (day >= 219 && day < 249)
                {
                    cDate.SetMonth(new CopticMonth(12));
                    cDate.SetDay(day - 218);
                }

                // Month of Nasie
                else if (day >= 249 && day < 254)
                {
                    cDate.SetMonth(new CopticMonth(13));
                    cDate.SetDay(day - 248);
                }
            }
            else
            {
                // Month of Tout
                if (day >= 255 && day < 285)
                {
                    cDate.SetMonth(new CopticMonth(1));
                    cDate.SetDay(day - 254);
                }

                // Month of Baba
                else if (day >= 285 && day < 315)
                {
                    cDate.SetMonth(new CopticMonth(2));
                    cDate.SetDay(day - 284);
                }

                // Month of Hator
                else if (day >= 315 && day < 345)
                {
                    cDate.SetMonth(new CopticMonth(3));
                    cDate.SetDay(day - 314);
                }

                // Month of Kiahk
                else if (day >= 345 && day < 366)
                {
                    cDate.SetMonth(new CopticMonth(4));
                    cDate.SetDay(day - 344);
                }
                else if (day >= 1 && day < 10)
                {
                    cDate.SetMonth(new CopticMonth(4));
                    // TODO: Calculate days to subtract to get day of month
                    cDate.SetDay(day - 0);
                }

                // Month of Toba
                else if (day >= 10 && day < 40)
                {
                    cDate.SetMonth(new CopticMonth(5));
                    cDate.SetDay(day - 9);
                }

                // Month of Amshir
                else if (day >= 40 && day < 69)
                {
                    cDate.SetMonth(new CopticMonth(6));
                    cDate.SetDay(day - 39);
                }

                // Month of Baramhat
                else if (day >= 69 && day < 99)
                {
                    cDate.SetMonth(new CopticMonth(7));
                    cDate.SetDay(day - 68);
                }

                // Month of Baramouda
                else if (day >= 99 && day < 129)
                {
                    cDate.SetMonth(new CopticMonth(8));
                    cDate.SetDay(day - 98);
                }

                // Month of Bashans
                else if (day >= 129 && day < 159)
                {
                    cDate.SetMonth(new CopticMonth(9));
                    cDate.SetDay(day - 128);
                }

                // Month of Paona
                else if (day >= 159 && day < 189)
                {
                    cDate.SetMonth(new CopticMonth(10));
                    cDate.SetDay(day - 158);
                }

                // Month of Epep
                else if (day >= 189 && day < 219)
                {
                    cDate.SetMonth(new CopticMonth(11));
                    cDate.SetDay(day - 188);
                }

                // Month of Mesra
                else if (day >= 219 && day < 249)
                {
                    cDate.SetMonth(new CopticMonth(12));
                    cDate.SetDay(day - 218);
                }

                // Month of Nasie
                else if (day >= 249 && day < 254)
                {
                    cDate.SetMonth(new CopticMonth(13));
                    cDate.SetDay(day - 248);
                }
            }

            if (day >= 254)
            {
                cDate.SetYear(eDate.Year - 283);
            }
            else
            {
                cDate.SetYear(eDate.Year - 284);
            }

            return cDate;
        }

        public DateTime ToGregorianDate()
        {
            return new DateTime(Ticks);
        }

        public CopticDate Next(DayOfWeek dayOfWeek)
        {
            return this.AddDays((dayOfWeek < this.DayOfWeek ? 7 : 0) + dayOfWeek - this.DayOfWeek);
        }

        public CopticDate GetNthWeekofMonth(CopticDate date, int nthWeek, DayOfWeek dayOfWeek)
        {
            return date.Next(dayOfWeek).AddDays((nthWeek - 1) * 7);
        }

        public static double JulianDate(int d, int m, int y)
        {
            int mm, yy; int k1, k2, k3; int j;

            yy = y - (int)((12 - m) / 10);
            mm = m + 9;

            if (mm >= 12)
            {
                mm = mm - 12;
            }
            k1 = (int)(365.25 * (yy + 4712));
            k2 = (int)(30.6001 * mm + 0.5);
            k3 = (int)((int)((yy / 100) + 49) * 0.75) - 38;
            j = k1 + k2 + d + 59;
            if (j > 2299160)
            {
                j = j - k3;
            }
            return j;
        }
        public static double JulianDate(DateTime date)
        {
            int mm, yy; int k1, k2, k3; int j;

            yy = date.Year - (int)((12 - date.Month) / 10);
            mm = date.Month + 9;

            if (mm >= 12)
            {
                mm = mm - 12;
            }
            k1 = (int)(365.25 * (yy + 4712));
            k2 = (int)(30.6001 * mm + 0.5);
            k3 = (int)((int)((yy / 100) + 49) * 0.75) - 38;
            j = k1 + k2 + date.Day + 59;
            if (j > 2299160)
            {
                j = j - k3;
            }
            return j;
        }
        public static double JulianDate(CopticDate date)
        {
            return JulianDate(date.ToGregorianDate());
        }

        public static DateTime JulianToDateTime(double julianDate)
        {
            DateTime date;
            double z, w, x, a, b, c, d, e, f;
            int day, month, year;

            try
            {
                z = Math.Floor(julianDate + 0.5);
                w = Math.Floor((z - 1867216.25) / 36524.25);
                x = Math.Floor(w / 4);
                a = z + 1 + w - x;
                b = a + 1524;
                c = Math.Floor((b - 122.1) / 365.25);
                d = Math.Floor(365.25 * c);
                e = Math.Floor((b - d) / 30.6001);
                f = Math.Floor(30.6001 * e);

                day = Convert.ToInt32(b - d - f);
                if (e > 13)
                {
                    month = Convert.ToInt32(e - 13);
                }
                else
                {
                    month = Convert.ToInt32(e - 1);
                }

                if ((month == 1) || (month == 2))
                {
                    year = Convert.ToInt32(c - 4715);
                }
                else
                {
                    year = Convert.ToInt32(c - 4716);
                }

                date = new DateTime(year, month, day);

                return date;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine(ex.Message);

                date = new DateTime(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                date = new DateTime(0);
            }

            return date;
        }
        public static CopticDate JulianToCopticDate(double julianDate)
        {
            return ToCopticDate(JulianToDateTime(julianDate));
        }

        public const double LunarMonthLength = 29.530588;
        public static double MoonAge(int d, int m, int y)
        {
            int j = (int)JulianDate(d, m, y);
            double ip = (j + 4.867) / LunarMonthLength;
            ip = ip - Math.Floor(ip);

            double ag;
            if (ip < 0.5)
                ag = ip * LunarMonthLength + LunarMonthLength / 2;
            else
                ag = ip * LunarMonthLength - LunarMonthLength / 2;

            ag = Math.Floor(ag) + 1;
            return ag;
        }
        public static double MoonAge(DateTime date)
        {
            int j = (int)JulianDate(date.Day, date.Month, date.Year);
            double ip = (j + 4.867) / LunarMonthLength;
            ip = ip - Math.Floor(ip);

            double ag;
            if (ip < 0.5)
                ag = ip * LunarMonthLength + LunarMonthLength / 2;
            else
                ag = ip * LunarMonthLength - LunarMonthLength / 2;

            ag = Math.Floor(ag) + 1;
            return ag;
        }
        public static double MoonAge(CopticDate date)
        {
            return MoonAge(date.ToGregorianDate());
        }

        public static bool IsLeapYear(DateTime date)
        {
            if (date == new DateTime())
                date = DateTime.Today;

            bool leapYear = false;
            if (date.Year % 4 == 0)
            {
                if (date.Year % 100 == 0)
                {
                    if (date.Year % 400 == 0)
                    {
                        leapYear = true;
                    }
                    else
                    {
                        leapYear = false;
                    }
                }
                else
                {
                    leapYear = true;
                }
            }
            else
            {
                leapYear = false;
            }

            return leapYear;
        }
        public static bool IsLeapYear(CopticDate date)
        {
            return IsLeapYear(date.ToGregorianDate());
        }
    }
}
