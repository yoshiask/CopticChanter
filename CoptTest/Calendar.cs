using CoptLib;
using CoptLib.Writing;
using NodaTime;
using System.Collections.Generic;
using Xunit;

namespace CoptTest
{
    public class Calendar
    {
        [Theory]
        [InlineData(1739, 13, 4, KnownLanguage.Coptic, "Ⲡⲓⲥⲁⲃⲃⲁⲧⲟⲛ, Ⲡⲓⲕⲟⲩϫⲓ ⲛ̀ⲁ̀ⲃⲟⲧ 4, 1739")]
        [InlineData(1739, 13, 1, KnownLanguage.Coptic, "Ⲡⲓϥ\u0300ⲧⲟⲩ, Ⲡⲓⲕⲟⲩϫⲓ ⲛ̀ⲁ̀ⲃⲟⲧ 1, 1739")]
        [InlineData(1739, 4, 28, KnownLanguage.English, "Friday, Koiahk 28, 1739")]
        [InlineData(1739, 4, 28, KnownLanguage.CopticBohairic, "Ⲡⲓⲥⲟⲟⲩ, Ⲭⲟⲓⲁⲕ 28, 1739")]
        // FIXME: Something somewhere isn't using the BCL's CultureInfo
        //[InlineData(1739, 4, 28, KnownLanguage.Amharic, "1739 ታኅሣሥ 28, ዓርብ")]
        public void CopticDateFormat(int year, int month, int day, KnownLanguage lang, string expected)
        {
            LocalDate date = DateHelper.NewCopticDate(year, month, day);
            var language = new LanguageInfo(lang);
            
            string actual = date.Format(language);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(Samples_Resurrection))]
        public void NextResurrection(LocalDate input, LocalDate expected, LocalDate _)
        {
            var actualCopt = DateHelper.GetNext(input, CopticCalendar.Resurrection);
            var actualGreg = actualCopt.WithGregorian();
            Assert.Equal(expected, actualGreg);
        }

        [Theory]
        [MemberData(nameof(Samples_Resurrection))]
        public void PreviousResurrection(LocalDate input, LocalDate _, LocalDate expected)
        {
            var actualCopt = DateHelper.GetPrevious(input, CopticCalendar.Resurrection);
            var actualGreg = actualCopt.WithGregorian();
            Assert.Equal(expected, actualGreg);
        }

        [Theory]
        [MemberData(nameof(Samples_HosannaSunday))]
        public void NextHosannaSunday(LocalDate input, LocalDate expected, LocalDate _)
        {
            var actualCopt = DateHelper.GetNext(input, CopticCalendar.HosannaSunday);
            var actualGreg = actualCopt.WithGregorian();
            Assert.Equal(expected, actualGreg);
        }

        [Theory]
        [MemberData(nameof(Samples_HosannaSunday))]
        public void PreviousHosannaSunday(LocalDate input, LocalDate _, LocalDate expected)
        {
            var actualCopt = DateHelper.GetPrevious(input, CopticCalendar.HosannaSunday);
            var actualGreg = actualCopt.WithGregorian();
            Assert.Equal(expected, actualGreg);
        }

        [Theory]
        [MemberData(nameof(Samples_ApostlesFeast))]
        public void NextAndPreviousApostlesFeast(LocalDate input, LocalDate nextExpected, LocalDate prevExpected)
        {
            var nextActual = DateHelper.GetNext(input, CopticCalendar.ApostlesFeast).WithGregorian();
            Assert.Equal(nextExpected, nextActual);

            var prevActual = DateHelper.GetPrevious(input, CopticCalendar.ApostlesFeast).WithGregorian();
            Assert.Equal(prevExpected, prevActual);
        }

        [Theory]
        [MemberData(nameof(Samples_ApostlesFast))]
        public void NextAndPreviousApostlesFast(LocalDate input, DatePeriod nextExpected, DatePeriod prevExpected)
        {
            var nextActual = DateHelper.GetNext(input, CopticCalendar.ApostlesFast).WithGregorian();
            Assert.Equal(nextExpected, nextActual);

            var prevActual = DateHelper.GetPrevious(input, CopticCalendar.ApostlesFast).WithGregorian();
            Assert.Equal(prevExpected, prevActual);
        }

        [Theory]
        [MemberData(nameof(Samples_LordsEntryIntoEgypt))]
        public void NextAndPreviousLordsEntryIntoEgypt(LocalDate input, LocalDate nextExpected, LocalDate prevExpected)
        {
            var nextActual = DateHelper.GetNext(input, CopticCalendar.LordsEntryIntoEgypt).WithGregorian();
            Assert.Equal(nextExpected, nextActual);

            var prevActual = DateHelper.GetPrevious(input, CopticCalendar.LordsEntryIntoEgypt).WithGregorian();
            Assert.Equal(prevExpected, prevActual);
        }

        [Theory]
        [MemberData(nameof(Samples_JonahsFast))]
        public void NextAndPreviousJonahsFast(LocalDate input, DatePeriod nextExpected, DatePeriod prevExpected)
        {
            var nextActual = DateHelper.GetNext(input, CopticCalendar.JonahsFast).WithGregorian();
            Assert.Equal(nextExpected, nextActual);

            var prevActual = DateHelper.GetPrevious(input, CopticCalendar.JonahsFast).WithGregorian();
            Assert.Equal(prevExpected, prevActual);
        }

        [Theory]
        [MemberData(nameof(Samples_Pascha))]
        public void NextAndPreviousPascha(LocalDate input, DatePeriod nextExpected, DatePeriod prevExpected)
        {
            var nextActual = DateHelper.GetNext(input, CopticCalendar.Pascha).WithGregorian();
            Assert.Equal(nextExpected, nextActual);

            var prevActual = DateHelper.GetPrevious(input, CopticCalendar.Pascha).WithGregorian();
            Assert.Equal(prevExpected, prevActual);
        }

        /// <summary>
        /// Samples for the Feast of the Resurrection, to test the Computus algorithm.
        /// </summary>
        public static readonly IEnumerable<object[]> Samples_Resurrection = new object[][]
        {
            // Gregorian year boundary
            new object[] { new LocalDate(2022, 12, 31, CalendarSystem.Gregorian),
                new LocalDate(2023, 4, 16, CalendarSystem.Gregorian), new LocalDate(2022, 4, 24, CalendarSystem.Gregorian) },

            // Coptic year boundary
            new object[] { new LocalDate(2022, 9, 11, CalendarSystem.Gregorian),
                new LocalDate(2023, 4, 16, CalendarSystem.Gregorian), new LocalDate(2022, 4, 24, CalendarSystem.Gregorian) },

            // On Hosanna Sunday, during a Gregorian and Coptic leap year
            new object[] { new LocalDate(2016, 4, 24, CalendarSystem.Gregorian),
                new LocalDate(2016, 5, 1, CalendarSystem.Gregorian), new LocalDate(2015, 4, 12, CalendarSystem.Gregorian) },

            // Beginning of current century, 2000-2100
            new object[] { new LocalDate(2001, 4, 7, CalendarSystem.Gregorian),
                new LocalDate(2001, 4, 15, CalendarSystem.Gregorian), new LocalDate(2000, 4, 30, CalendarSystem.Gregorian) },
        };

        /// <summary>
        /// Samples for Hosanna Sunday, to test dates that are directly dependent on
        /// the Feast of the Resurrection.
        /// </summary>
        public static readonly IEnumerable<object[]> Samples_HosannaSunday = new object[][]
        {
            // Gregorian year boundary
            new object[] { new LocalDate(2022, 12, 31, CalendarSystem.Gregorian),
                new LocalDate(2023, 4, 9, CalendarSystem.Gregorian), new LocalDate(2022, 4, 17, CalendarSystem.Gregorian) },

            // Coptic year boundary
            new object[] { new LocalDate(2022, 9, 11, CalendarSystem.Gregorian),
                new LocalDate(2023, 4, 9, CalendarSystem.Gregorian), new LocalDate(2022, 4, 17, CalendarSystem.Gregorian) },

            // On Hosanna Sunday, during a Gregorian and Coptic leap year
            new object[] { new LocalDate(2016, 4, 24, CalendarSystem.Gregorian),
                new LocalDate(2016, 4, 24, CalendarSystem.Gregorian), new LocalDate(2015, 4, 5, CalendarSystem.Gregorian) },

            // Beginning of current century, 2000-2100
            new object[] { new LocalDate(2001, 4, 7, CalendarSystem.Gregorian),
                new LocalDate(2001, 4, 8, CalendarSystem.Gregorian), new LocalDate(2000, 4, 23, CalendarSystem.Gregorian) },
        };

        /// <summary>
        /// Samples for the Apostle's Feast, to test dates that are indirectly dependent
        /// on the Feast of the Resurrection.
        /// </summary>
        public static readonly IEnumerable<object[]> Samples_ApostlesFeast = new object[][]
        {
            // Gregorian year boundary
            new object[] { new LocalDate(2022, 12, 31, CalendarSystem.Gregorian),
                new LocalDate(2023, 7, 12, CalendarSystem.Gregorian), new LocalDate(2022, 7, 12, CalendarSystem.Gregorian) },

            // Coptic year boundary
            new object[] { new LocalDate(2022, 9, 11, CalendarSystem.Gregorian),
                new LocalDate(2023, 7, 12, CalendarSystem.Gregorian), new LocalDate(2022, 7, 12, CalendarSystem.Gregorian) },

            // On Hosanna Sunday, during a Gregorian and Coptic leap year
            new object[] { new LocalDate(2016, 4, 24, CalendarSystem.Gregorian),
                new LocalDate(2016, 7, 12, CalendarSystem.Gregorian), new LocalDate(2015, 7, 12, CalendarSystem.Gregorian) },

            // Beginning of current century, 2000-2100
            new object[] { new LocalDate(2001, 4, 7, CalendarSystem.Gregorian),
                new LocalDate(2001, 7, 12, CalendarSystem.Gregorian), new LocalDate(2000, 7, 12, CalendarSystem.Gregorian) },
        };

        /// <summary>
        /// Samples for the Apostle's Fast, to test multi-day events that are indirectly dependent
        /// on the Feast of the Resurrection.
        /// </summary>
        public static readonly IEnumerable<object[]> Samples_ApostlesFast = new object[][]
        {
            // Gregorian year boundary
            new object[] { new LocalDate(2023, 1, 1, CalendarSystem.Gregorian),
                new DatePeriod(new(2023, 6, 5, CalendarSystem.Gregorian), new LocalDate(2023, 7, 11, CalendarSystem.Gregorian)),
                new DatePeriod(new(2022, 6, 13, CalendarSystem.Gregorian), new LocalDate(2022, 7, 11, CalendarSystem.Gregorian)) },

            // Coptic year boundary
            new object[] { new LocalDate(2022, 9, 11, CalendarSystem.Gregorian),
                new DatePeriod(new(2023, 6, 5, CalendarSystem.Gregorian), new LocalDate(2023, 7, 11, CalendarSystem.Gregorian)),
                new DatePeriod(new(2022, 6, 13, CalendarSystem.Gregorian), new LocalDate(2022, 7, 11, CalendarSystem.Gregorian)) },

            // On Hosanna Sunday, during a Gregorian and Coptic leap year
            new object[] { new LocalDate(2016, 4, 24, CalendarSystem.Gregorian),
                new DatePeriod(new(2016, 6, 20, CalendarSystem.Gregorian), new LocalDate(2016, 7, 11, CalendarSystem.Gregorian)),
                new DatePeriod(new(2015, 6, 1, CalendarSystem.Gregorian), new LocalDate(2015, 7, 11, CalendarSystem.Gregorian)) },

            // Beginning of current century, 2000-2100
            new object[] { new LocalDate(2001, 4, 7, CalendarSystem.Gregorian),
                new DatePeriod(new(2001, 6, 4, CalendarSystem.Gregorian), new LocalDate(2001, 7, 11, CalendarSystem.Gregorian)),
                new DatePeriod(new(2000, 6, 19, CalendarSystem.Gregorian), new LocalDate(2000, 7, 11, CalendarSystem.Gregorian)) },

            // During fast
            new object[] { new LocalDate(2001, 6, 24, CalendarSystem.Gregorian),
                new DatePeriod(new(2001, 6, 4, CalendarSystem.Gregorian), new LocalDate(2001, 7, 11, CalendarSystem.Gregorian)),
                new DatePeriod(new(2000, 6, 19, CalendarSystem.Gregorian), new LocalDate(2000, 7, 11, CalendarSystem.Gregorian)) },
        };

        /// <summary>
        /// Samples for the Lord Christ's Entry in the Land of Egypt, to test dates
        /// that are fixed in the Coptic calendar.
        /// </summary>
        public static readonly IEnumerable<object[]> Samples_LordsEntryIntoEgypt = new object[][]
        {
            // Gregorian year boundary
            new object[] { new LocalDate(2022, 12, 31, CalendarSystem.Gregorian),
                new LocalDate(2023, 6, 1, CalendarSystem.Gregorian), new LocalDate(2022, 6, 1, CalendarSystem.Gregorian) },

            // Coptic year boundary
            new object[] { new LocalDate(2022, 9, 11, CalendarSystem.Gregorian),
                new LocalDate(2023, 6, 1, CalendarSystem.Gregorian), new LocalDate(2022, 6, 1, CalendarSystem.Gregorian) },

            // On Hosanna Sunday, during a Gregorian and Coptic leap year
            new object[] { new LocalDate(2016, 4, 24, CalendarSystem.Gregorian),
                new LocalDate(2016, 6, 1, CalendarSystem.Gregorian), new LocalDate(2015, 6, 1, CalendarSystem.Gregorian) },

            // Beginning of current century, 2000-2100
            new object[] { new LocalDate(2001, 4, 7, CalendarSystem.Gregorian),
                new LocalDate(2001, 6, 1, CalendarSystem.Gregorian), new LocalDate(2000, 6, 1, CalendarSystem.Gregorian) },
        };

        /// <summary>
        /// Samples for the Annunciation Feast, to test dates that are not celebrated every year.
        /// </summary>
        public static readonly IEnumerable<object[]> Samples_Annunciation = new object[][]
        {
            // Gregorian year boundary
            new object[] { new LocalDate(2022, 12, 31, CalendarSystem.Gregorian),
                new LocalDate(2024, 4, 7, CalendarSystem.Gregorian), new LocalDate(2022, 4, 7, CalendarSystem.Gregorian) },

            // Coptic year boundary
            new object[] { new LocalDate(2022, 9, 11, CalendarSystem.Gregorian),
                new LocalDate(2024, 4, 7, CalendarSystem.Gregorian), new LocalDate(2022, 4, 7, CalendarSystem.Gregorian) },

            // On Hosanna Sunday, during a Gregorian and Coptic leap year
            new object[] { new LocalDate(2016, 4, 24, CalendarSystem.Gregorian),
                new LocalDate(2017, 4, 7, CalendarSystem.Gregorian), new LocalDate(2016, 4, 7, CalendarSystem.Gregorian) },

            // Beginning of current century, 2000-2100
            new object[] { new LocalDate(2001, 4, 7, CalendarSystem.Gregorian),
                new LocalDate(2002, 4, 7, CalendarSystem.Gregorian), new LocalDate(2000, 4, 7, CalendarSystem.Gregorian) },

        };

        /// <summary>
        /// Samples for Jonah's Fast, to test multi-day events that are defined using an end date
        /// and duration.
        /// </summary>
        public static readonly IEnumerable<object[]> Samples_JonahsFast = new object[][]
        {
            // Gregorian year boundary
            new object[] { new LocalDate(2022, 12, 31, CalendarSystem.Gregorian),
                new DatePeriod(new(2023, 2, 6, CalendarSystem.Gregorian), new LocalDate(2023, 2, 8, CalendarSystem.Gregorian)),
                new DatePeriod(new(2022, 2, 14, CalendarSystem.Gregorian), new LocalDate(2022, 2, 16, CalendarSystem.Gregorian)) },

            // Coptic year boundary
            new object[] { new LocalDate(2022, 9, 11, CalendarSystem.Gregorian),
                new DatePeriod(new(2023, 2, 6, CalendarSystem.Gregorian), new LocalDate(2023, 2, 8, CalendarSystem.Gregorian)),
                new DatePeriod(new(2022, 2, 14, CalendarSystem.Gregorian), new LocalDate(2022, 2, 16, CalendarSystem.Gregorian)) },

            // On Hosanna Sunday, during a Gregorian and Coptic leap year
            new object[] { new LocalDate(2016, 4, 24, CalendarSystem.Gregorian),
                new DatePeriod(new(2017, 2, 6, CalendarSystem.Gregorian), new LocalDate(2017, 2, 8, CalendarSystem.Gregorian)),
                new DatePeriod(new(2016, 2, 22, CalendarSystem.Gregorian), new LocalDate(2016, 2, 24, CalendarSystem.Gregorian)) },

            // Beginning of current century, 2000-2100
            new object[] { new LocalDate(2001, 4, 7, CalendarSystem.Gregorian),
                new DatePeriod(new(2002, 2, 25, CalendarSystem.Gregorian), new LocalDate(2002, 2, 27, CalendarSystem.Gregorian)),
                new DatePeriod(new(2001, 2, 5, CalendarSystem.Gregorian), new LocalDate(2001, 2, 7, CalendarSystem.Gregorian)) },

            // During fast
            new object[] { new LocalDate(2023, 2, 8, CalendarSystem.Gregorian),
                new DatePeriod(new(2023, 2, 6, CalendarSystem.Gregorian), new LocalDate(2023, 2, 8, CalendarSystem.Gregorian)),
                new DatePeriod(new(2022, 2, 14, CalendarSystem.Gregorian), new LocalDate(2022, 2, 16, CalendarSystem.Gregorian)) },
        };

        /// <summary>
        /// Samples for Pascha, to test multi-day events that are defined using a start date
        /// and duration.
        /// </summary>
        public static readonly IEnumerable<object[]> Samples_Pascha = new object[][]
        {
            // Gregorian year boundary
            new object[] { new LocalDate(2022, 12, 31, CalendarSystem.Gregorian),
                new DatePeriod(new(2023, 4, 10, CalendarSystem.Gregorian), new LocalDate(2023, 4, 12, CalendarSystem.Gregorian)),
                new DatePeriod(new(2022, 4, 18, CalendarSystem.Gregorian), new LocalDate(2022, 4, 20, CalendarSystem.Gregorian)) },

            // Coptic year boundary
            new object[] { new LocalDate(2022, 9, 11, CalendarSystem.Gregorian),
                new DatePeriod(new(2023, 4, 10, CalendarSystem.Gregorian), new LocalDate(2023, 4, 12, CalendarSystem.Gregorian)),
                new DatePeriod(new(2022, 4, 18, CalendarSystem.Gregorian), new LocalDate(2022, 4, 20, CalendarSystem.Gregorian)) },

            // On Hosanna Sunday, during a Gregorian and Coptic leap year
            new object[] { new LocalDate(2016, 4, 24, CalendarSystem.Gregorian),
                new DatePeriod(new(2016, 4, 25, CalendarSystem.Gregorian), new LocalDate(2016, 4, 27, CalendarSystem.Gregorian)),
                new DatePeriod(new(2015, 4, 6, CalendarSystem.Gregorian), new LocalDate(2015, 4, 8, CalendarSystem.Gregorian)) },

            // Beginning of current century, 2000-2100
            new object[] { new LocalDate(2001, 4, 7, CalendarSystem.Gregorian),
                new DatePeriod(new(2001, 4, 9, CalendarSystem.Gregorian), new LocalDate(2001, 4, 11, CalendarSystem.Gregorian)),
                new DatePeriod(new(2000, 4, 24, CalendarSystem.Gregorian), new LocalDate(2000, 4, 26, CalendarSystem.Gregorian)) },

            // During fast and across Gregorian month boundary
            new object[] { new LocalDate(2024, 4, 30, CalendarSystem.Gregorian),
                new DatePeriod(new(2024, 4, 29, CalendarSystem.Gregorian), new LocalDate(2024, 5, 1, CalendarSystem.Gregorian)),
                new DatePeriod(new(2023, 4, 10, CalendarSystem.Gregorian), new LocalDate(2023, 4, 12, CalendarSystem.Gregorian)) },
        };
    }
}
