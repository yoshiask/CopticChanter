using NodaTime;
using System;
using System.Collections.Generic;

namespace CoptLib
{
    public static class CopticDateHelper
    {
        public static LocalDate TodayCoptic => DateTime.Today.ToCopticDate();
        public static LocalDate TodayJulian => DateTime.Today.ToJulian();

        public static LocalDate ToCopticDate(this DateTime date)
        {
            return LocalDate.FromDateTime(date).WithCalendar(CalendarSystem.Coptic);
        }
        public static LocalDate ToJulian(this DateTime date)
        {
            return LocalDate.FromDateTime(date).WithCalendar(CalendarSystem.Julian);
        }
        public static LocalDate GregorianToCoptic(this LocalDate greg)
        {
            if (greg.Calendar == CalendarSystem.Coptic)
                return greg;

            var copt = greg.WithCalendar(CalendarSystem.Coptic);
            if (greg.DayOfYear >= 254)
                copt.PlusYears(-283);
            else
                copt.PlusYears(-284);
            return copt;
        }
        public static LocalDate CopticToGregorian(this LocalDate copt)
        {
            if (copt.Calendar == CalendarSystem.Gregorian)
                return copt;

            var greg = copt.WithCalendar(CalendarSystem.Gregorian);
            if (copt.DayOfYear >= 254)
                greg.PlusYears(283);
            else
                greg.PlusYears(284);
            return greg;
        }

        public static LocalDate GetNextFeastResurrection(LocalDate date)
        {
            date = date.WithCalendar(CalendarSystem.Coptic);
            var palmSunday = CalcFeastResurrection(date);

            if (palmSunday < date)
                return CalcFeastResurrection(date.PlusYears(1));
            else
                return palmSunday;
        }
        public static LocalDate GetNextFeastResurrection() => GetNextFeastResurrection(TodayCoptic);
        public static LocalDate CalcFeastResurrection(LocalDate dateWithYear)
        {
            int year = dateWithYear.CopticToGregorian().WithCalendar(CalendarSystem.Julian).Year;

            // From https://en.wikipedia.org/wiki/Date_of_Easter#Julian_calendar
            int goldenNum = (year) % 19;
            (int month, int day) = JulianPaschalFullMoons[goldenNum];
            var pashcalFullMoon = new LocalDate(year, month, day, CalendarSystem.Julian);
            return pashcalFullMoon.WithCalendar(CalendarSystem.Gregorian)
                .Next(IsoDayOfWeek.Sunday)
                .WithCalendar(CalendarSystem.Coptic);

        }
        public static (int month, int day)[] JulianPaschalFullMoons = new (int month, int day)[]
        {
            (4, 5),     // 1
            (3, 25),    // 2
            (4, 13),    // 3
            (4, 2),     // 4
            (3, 22),    // 5
            (4, 10),    // 6
            (3, 30),    // 7
            (4, 18),    // 8
            (4, 7),     // 9
            (3, 27),    // 10
            (4, 15),    // 11
            (4, 4),     // 12
            (3, 24),    // 13
            (4, 12),    // 14
            (4, 1),     // 15
            (3, 21),    // 16
            (4, 9),     // 17
            (3, 29),    // 18
            (4, 17),    // 19
        };

        public static LocalDate GetNextLazarusSaturday(LocalDate date)
        {
            date = date.WithCalendar(CalendarSystem.Coptic);
            var lazarusSaturday = CalcLazarusSaturday(date);

            if (lazarusSaturday < date)
                return CalcLazarusSaturday(date.PlusYears(1));
            else
                return lazarusSaturday;
        }
        public static LocalDate GetNextLazarusSaturday() => GetNextLazarusSaturday(TodayCoptic);
        public static LocalDate CalcLazarusSaturday(LocalDate dateWithYear)
        {
            return CalcFeastResurrection(dateWithYear).PlusDays(-8);
        }

        public static LocalDate GetNextHosannaSunday(LocalDate date)
        {
            date = date.WithCalendar(CalendarSystem.Coptic);
            var hosannaSunday = CalcHosannaSunday(date);

            if (hosannaSunday < date)
                return CalcHosannaSunday(date.PlusYears(1));
            else
                return hosannaSunday;
        }
        public static LocalDate CalcHosannaSunday(LocalDate dateWithYear)
        {
            return CalcFeastResurrection(dateWithYear).PlusWeeks(-1);
        }

        public static LocalDate GetNextPascha(LocalDate date)
        {
            date = date.WithCalendar(CalendarSystem.Coptic);
            var holyPascha = CalcPascha(date);

            if (holyPascha < date)
                return CalcPascha(date.PlusYears(1));
            else
                return holyPascha;
        }
        public static LocalDate CalcPascha(LocalDate dateWithYear)
        {
            return CalcFeastResurrection(dateWithYear).PlusDays(-6);
        }

        public static LocalDate GetNextCovenantThursday(LocalDate date)
        {
            date = date.WithCalendar(CalendarSystem.Coptic);
            var covenantThursday = CalcCovenantThursday(date);

            if (covenantThursday < date)
                return CalcCovenantThursday(date.PlusYears(1));
            else
                return covenantThursday;
        }
        public static LocalDate CalcCovenantThursday(LocalDate dateWithYear)
        {
            return CalcFeastResurrection(dateWithYear).PlusDays(-3);
        }

        public static LocalDate GetNextGoodFriday(LocalDate date)
        {
            date = date.WithCalendar(CalendarSystem.Coptic);
            var goodFriday = CalcGoodFriday(date);

            if (goodFriday < date)
                return CalcGoodFriday(date.PlusYears(1));
            else
                return goodFriday;
        }
        public static LocalDate CalcGoodFriday(LocalDate dateWithYear)
        {
            return CalcFeastResurrection(dateWithYear).PlusDays(-2);
        }

        public static LocalDate GetNextNativityFast(LocalDate date)
        {
            date = date.WithCalendar(CalendarSystem.Coptic);
            var nativityFast = CalcNativityFast(date);

            if (nativityFast < date)
                return CalcNativityFast(date.PlusYears(1));
            else
                return nativityFast;
        }
        public static LocalDate CalcNativityFast(LocalDate dateWithYear)
        {
            return CalcNativity(dateWithYear).PlusDays(6 * 7);
        }

        public static LocalDate GetNextNativity(LocalDate date)
        {
            date = date.WithCalendar(CalendarSystem.Coptic);
            var nativity = CalcNativity(date);

            if (nativity < date)
                return CalcNativity(date.PlusYears(1));
            else
                return nativity;
        }
        public static LocalDate CalcNativity(LocalDate dateWithYear)
        {
            return new LocalDate(dateWithYear.Year, 1, 7);
        }

        public static LocalDate GetNextNativitySunday(LocalDate date)
        {
            date = date.WithCalendar(CalendarSystem.Coptic);
            var nativitySunday = CalcNativitySunday(date);

            if (nativitySunday < date)
                return CalcNativitySunday(date.PlusYears(1));
            else
                return nativitySunday;
        }
        public static LocalDate CalcNativitySunday(LocalDate dateWithYear)
        {
            LocalDate nativitySunday = CalcNativity(dateWithYear);
            while (nativitySunday.DayOfWeek != IsoDayOfWeek.Sunday)
            {
                nativitySunday.PlusDays(-1);
            }
            return nativitySunday;
        }
    }
}
