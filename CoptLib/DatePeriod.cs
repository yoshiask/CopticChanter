using NodaTime;
using System;

namespace CoptLib
{
    /// <summary>
    /// Represents a period of time between two dates, inclusive.
    /// </summary>
    public readonly struct DatePeriod : IEquatable<DatePeriod>
    {
        /// <summary>
        /// Constructs a new <see cref="DatePeriod"/> that
        /// starts and ends on the given dates.
        /// </summary>
        /// <param name="endDate">The first day of the period.</param>
        /// <param name="endDate">The last day of the period.</param>
        public DatePeriod(LocalDate startDate, LocalDate endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        /// <summary>
        /// Constructs a new <see cref="DatePeriod"/> of the given length
        /// that starts on the given start date.
        /// </summary>
        /// <param name="startDate">The first day of the period.</param>
        /// <param name="period">The length of the period.</param>
        public DatePeriod(LocalDate startDate, Period period)
        {
            StartDate = startDate;
            EndDate = startDate + period;
        }

        public LocalDate StartDate { get; }

        public LocalDate EndDate { get; }

        /// <summary>
        /// Determines whether the given date occurs during the period.
        /// </summary>
        /// <param name="date">The date to test.</param>
        public bool IsDuring(LocalDate date) => date >= StartDate && date <= EndDate;

        /// <summary>
        /// Creates a new <see cref="DatePeriod"/> representing the same physical
        /// period, using the Coptic calendar system.
        /// </summary>
        public DatePeriod WithCoptic() => new(StartDate.WithCoptic(), EndDate.WithCoptic());

        /// <summary>
        /// Creates a new <see cref="DatePeriod"/> representing the same physical
        /// period, using the Gregorian calendar system.
        /// </summary>
        public DatePeriod WithGregorian() => new(StartDate.WithGregorian(), EndDate.WithGregorian());

        public bool Equals(DatePeriod other) => StartDate == other.StartDate && EndDate == other.EndDate;

        public override string ToString() => $"[{StartDate}, {EndDate}]";

        /// <summary>
        /// Constructs a new <see cref="DatePeriod"/> of the given length
        /// that ends on the given end date.
        /// </summary>
        /// <param name="endDate">The last day of the period.</param>
        /// <param name="period">The length of the period.</param>
        public static DatePeriod FromEndDate(LocalDate endDate, Period period) => new((endDate - period).PlusDays(1), endDate);
    }
}
