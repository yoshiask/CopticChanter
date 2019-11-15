using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoptLib
{
    public class CopticDate : IComparable, IComparable<CopticDate>, IEquatable<CopticDate>
    {
        #region Properties
        private CopticMonth _month;
        private int _day;

        public int Year {
            get;
            internal set;
        }
        public int Greg_Year {
            get {
                return Year + 284;
            }
        }

        public int Day {
            get => _day;
            internal set {
                if (value > 30)
                {
                    _day = value - 30;
                    Month++;
                }
                else
                {
                    _day = value;
                }
            }
        }

        public CopticMonth Month {
            get => _month;
            internal set {
                _month = value;
            }
        }

        public long Ticks {
            get;
            set;
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

            // Check if eDate.Year is a leap year
            bool leapYear = false;
            if (eDate.Year % 4 == 0)
            {
                if (eDate.Year % 100 == 0)
                {
                    if (eDate.Year % 400 == 0)
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

            // Calculate Coptic month & day from eDate.DayOfYear
            int day = eDate.DayOfYear;
            if (!leapYear)
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
                    cDate.SetDay(day+ 22);
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
            else if (leapYear)
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
    }
}
