using NodaTime;
using NodaTime.Extensions;
using System;

namespace CoptLib
{
    public static partial class DateHelper
    {
        /// <summary>
        /// The current date in the Coptic calendar, taking into account the <see cref="CopticCalendar.TransitionTime"/>.
        /// </summary>
        public static LocalDate NowCoptic()
        {
            var now = DateTime.Now;
            var copticDate = now.ToLocalDateTime().WithCoptic();

            // If it's already after the transition time, snap to the next day.
            if (copticDate.TimeOfDay >= CopticCalendar.TransitionTime)
                copticDate = copticDate.PlusDays(1);

            return copticDate.Date;
        }

        /// <summary>
        /// Creates a new <see cref="LocalDate"/> representing the same physical copticYear,
        /// using the Coptic calendar system.
        /// </summary>
        public static LocalDate ToCopticDate(this DateTime date) => LocalDate.FromDateTime(date).WithCoptic();

        public static LocalDate NewCopticDate(int copticYear, int month, int day)
            => new(NodaTime.Calendars.Era.AnnoMartyrum, copticYear, month, day, CalendarSystem.Coptic);


        /// <summary>
        /// Creates a new <see cref="LocalDate"/> representing the same physical date,
        /// using the Coptic calendar system.
        /// </summary>
        public static LocalDate WithCoptic(this LocalDate date) => date.WithCalendar(CalendarSystem.Coptic);

        /// <summary>
        /// Creates a new <see cref="LocalDateTime"/> representing the same physical date and time,
        /// using the Coptic calendar system.
        /// </summary>
        public static LocalDateTime WithCoptic(this LocalDateTime date) => date.WithCalendar(CalendarSystem.Coptic);

        /// <summary>
        /// Creates a new <see cref="LocalDate"/> representing the same physical date,
        /// using the Gregorian calendar system.
        /// </summary>
        public static LocalDate WithGregorian(this LocalDate date) => date.WithCalendar(CalendarSystem.Gregorian);

        /// <summary>
        /// Creates a new <see cref="LocalDateTime"/> representing the same physical date and time,
        /// using the Gregorian calendar system.
        /// </summary>
        public static LocalDateTime WithGregorian(this LocalDateTime date) => date.WithCalendar(CalendarSystem.Gregorian);


        /// <summary>
        /// Gets the next occurrence of the given event.
        /// </summary>
        /// <param name="date">The current copticYear.</param>
        /// <param name="dateCalc">
        /// A function that returns the date of an event in given a Coptic year.
        /// </param>
        /// <returns>
        /// The Coptic calendar date of the next occurrence of the event.
        /// </returns>
        public static LocalDate GetNext(LocalDate date, Func<int, LocalDate> dateCalc)
        {
            date = date.WithCoptic();
            var year = date.YearOfEra;
            var eventDate = dateCalc(year);

            if (eventDate < date)
                return dateCalc(year + 1);
            else
                return eventDate;
        }

        /// <summary>
        /// Gets the next occurrence of the given event.
        /// </summary>
        /// <param name="date">The current copticYear.</param>
        /// <param name="dateCalc">
        /// A function that returns the date of an event in given a Coptic year.
        /// </param>
        /// <returns>
        /// The Coptic calendar date of the next occurrence of the event.
        /// </returns>
        public static DatePeriod GetNext(LocalDate date, Func<int, DatePeriod> dateCalc)
        {
            date = date.WithCoptic();
            var year = date.YearOfEra;
            var eventDate = dateCalc(year);

            if (eventDate.EndDate < date)
                return dateCalc(year + 1);
            else
                return eventDate;
        }


        /// <summary>
        /// Gets the most recent occurrence of this event that has already passed.
        /// </summary>
        /// <param name="date">The current copticYear.</param>
        /// <param name="dateCalc">
        /// A function that returns the date of an event in given a Coptic year.
        /// </param>
        /// <returns>
        /// The Coptic calendar date of the previous occurrence of the event.
        /// </returns>
        public static LocalDate GetPrevious(LocalDate date, Func<int, LocalDate> dateCalc)
        {
            date = date.WithCoptic();
            var year = date.YearOfEra;
            var eventDate = dateCalc(year);

            if (eventDate >= date)
                return dateCalc(year - 1);
            else
                return eventDate;
        }

        /// <summary>
        /// Gets the most recent occurrence of this event that has already passed.
        /// </summary>
        /// <param name="date">The current copticYear.</param>
        /// <param name="dateCalc">
        /// A function that returns the date of an event in given a Coptic year.
        /// </param>
        /// <returns>
        /// The Coptic calendar date of the previous occurrence of the event.
        /// </returns>
        public static DatePeriod GetPrevious(LocalDate date, Func<int, DatePeriod> dateCalc)
        {
            date = date.WithCoptic();
            var year = date.YearOfEra;
            var eventDate = dateCalc(year);

            if (eventDate.EndDate >= date)
                return dateCalc(year - 1);
            else
                return eventDate;
        }
    }
}
