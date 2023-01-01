using NodaTime;
using System.Collections.Generic;

namespace CoptLib
{
    public static class CopticCalendar
    {
        private static readonly (int month, int day)[] JulianPaschalFullMoons = new (int month, int day)[]
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

        private static readonly Dictionary<int, LocalDate> ResurrectionCache = new();

        /// <summary>
        /// The time of day after which the rites of the next day are used.
        /// </summary>
        /// <remarks>
        /// Defaults to 6:00 PM.
        /// </remarks>
        public static LocalTime TransitionTime { get; set; } = new(18, 00);

        public static LocalDate NewYear(int copticYear) => DateHelper.NewCopticDate(copticYear, 1, 1);

        public static LocalDate FirstFeastCross(int copticYear) => DateHelper.NewCopticDate(copticYear, 1, 17);

        public static DatePeriod KoiahkSeason(int copticYear)
            => new(DateHelper.NewCopticDate(copticYear, 4, 1), DateHelper.NewCopticDate(copticYear, 4, 28));

        public static LocalDate NativityFast(int copticYear)
        {
            // 40 days for Christ + Elijah + Moses, and 3 for the mountain
            return Nativity(copticYear).PlusDays(-43);
        }

        public static LocalDate Nativity(int copticYear) => DateHelper.NewCopticDate(copticYear, 4, 29);

        public static LocalDate Circumcision(int copticYear) => DateHelper.NewCopticDate(copticYear, 5, 6);

        public static LocalDate Theophany(int copticYear) => Circumcision(copticYear).PlusWeeks(1);

        public static LocalDate Epiphany(int copticYear) => Nativity(copticYear).PlusDays(12);

        public static LocalDate WeddingAtCanaOfGalilee(int copticYear) => Epiphany(copticYear).PlusDays(2);

        public static LocalDate PresentationLordChristTemple(int copticYear) => DateHelper.NewCopticDate(copticYear, 6, 8);

        public static DatePeriod JonahsFast(int copticYear) => DatePeriod.FromEndDate(JonahsFeast(copticYear).PlusDays(-1), Period.FromDays(3));

        public static LocalDate JonahsFeast(int copticYear) => GreatFast(copticYear).StartDate.PlusDays(-11);

        public static DatePeriod GreatFast(int copticYear)
            => new(Resurrection(copticYear).PlusDays(-55), GreatFastLastDay(copticYear));

        public static LocalDate GreatFastLastDay(int copticYear) => LazarusSaturday(copticYear).PlusDays(-1);

        public static LocalDate SecondFeastCross(int copticYear) => DateHelper.NewCopticDate(copticYear, 7, 10);

        public static LocalDate Annunciation(int copticYear)
        {
            LocalDate feastDate = DateHelper.NewCopticDate(copticYear, 7, 29);

            // The Annunciation Feast does not appear to be celebrated when it falls
            // between the last day of Great Lent and the Feast of the Resurrection.
            if (PaschalPeriod(copticYear).IsDuring(feastDate))
                return Annunciation(copticYear + 1);

            return feastDate;
        }

        public static DatePeriod PaschalPeriod(int copticYear) => new(GreatFastLastDay(copticYear), Resurrection(copticYear));

        public static LocalDate LazarusSaturday(int copticYear) => Resurrection(copticYear).PlusDays(-8);

        public static LocalDate HosannaSunday(int copticYear) => Resurrection(copticYear).PlusWeeks(-1);

        public static DatePeriod Pascha(int copticYear) => new(Resurrection(copticYear).PlusDays(-6), Period.FromDays(2));

        public static LocalDate CovenantThursday(int copticYear) => Resurrection(copticYear).PlusDays(-3);

        public static LocalDate GoodFriday(int copticYear) => Resurrection(copticYear).PlusDays(-2);

        public static LocalDate JoyousSaturday(int copticYear) => Resurrection(copticYear).PlusDays(-1);

        public static LocalDate Resurrection(int copticYear)
        {
            if (!ResurrectionCache.TryGetValue(copticYear, out var feastDate))
            {
                // Convert the Coptic year to the Julian calendar.
                // Since Easter will always fall after the Julian new year,
                // we should always subtract 284 (rather than 283).
                int julianYear = copticYear + 284;

                // From https://en.wikipedia.org/wiki/Date_of_Easter#Julian_calendar
                int goldenNum = julianYear % 19;
                (int month, int day) = JulianPaschalFullMoons[goldenNum];
                var pashcalFullMoon = new LocalDate(julianYear, month, day, CalendarSystem.Julian);

                feastDate = pashcalFullMoon
                    .Next(IsoDayOfWeek.Sunday)
                    .WithCoptic();
                ResurrectionCache.Add(copticYear, feastDate);
            }

            return feastDate;
        }

        public static LocalDate ThomasSunday(int copticYear) => Resurrection(copticYear).PlusWeeks(1);

        public static LocalDate MartyrdomStMark(int copticYear) => DateHelper.NewCopticDate(copticYear, 8, 30);

        public static LocalDate Ascension(int copticYear) => Resurrection(copticYear).PlusDays(39);

        public static LocalDate LordsEntryIntoEgypt(int copticYear) => DateHelper.NewCopticDate(copticYear, 9, 24);

        public static LocalDate Pentecost(int copticYear) => Resurrection(copticYear).PlusWeeks(7);

        public static DatePeriod ApostlesFast(int copticYear) => new(Pentecost(copticYear).PlusDays(1), ApostlesFeast(copticYear).PlusDays(-1));

        public static LocalDate ApostlesFeast(int copticYear) => DateHelper.NewCopticDate(copticYear, 11, 5);

        public static DatePeriod StMarysFast(int copticYear)
            => new(DateHelper.NewCopticDate(copticYear, 12, 1), Assumption(copticYear).PlusDays(-1));

        public static LocalDate Transfiguration(int copticYear) => DateHelper.NewCopticDate(copticYear, 11, 13);

        public static LocalDate Assumption(int copticYear) => DateHelper.NewCopticDate(copticYear, 12, 16);
    }
}
