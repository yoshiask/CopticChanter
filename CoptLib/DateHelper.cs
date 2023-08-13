using CoptLib.Writing;
using NodaTime;
using NodaTime.Extensions;
using System;
using System.Linq;

namespace CoptLib;

public static partial class DateHelper
{
    /// <summary>
    /// The current date in the Coptic calendar, taking into account the <see cref="CopticCalendar.TransitionTime"/>
    /// but ignoring any overrides.
    /// </summary>
    public static LocalDate TodayCoptic() => DateTime.Now.ToLocalDateTime().ToCopticDate();

    /// <summary>
    /// Uses the <see cref="CopticCalendar.TransitionTime"/> to convert the given <paramref name="dateTime"/>
    /// to a <see cref="LocalDate"/>.
    /// </summary>
    public static LocalDate ToCopticDate(this LocalDateTime dateTime)
    {
        var copticDateTime = dateTime.WithCoptic();

        // If it's already after the transition time, snap to the next day.
        if (copticDateTime.TimeOfDay >= CopticCalendar.TransitionTime)
            copticDateTime = copticDateTime.PlusDays(1);

        return copticDateTime.Date;
    }

    /// <summary>
    /// Creates a new <see cref="LocalDate"/> representing the same physical copticYear,
    /// using the Coptic calendar system.
    /// </summary>
    public static LocalDate ToCopticDate(this DateTime date) => LocalDate.FromDateTime(date).WithCoptic();

    /// <summary>
    /// Creates a new <see cref="LocalDate"/> with the given year, month, and day
    /// using the Coptic calendar and Anno Martyrum era.
    /// </summary>
    public static LocalDate NewCopticDate(int copticYear, int month, int day)
        => new(NodaTime.Calendars.Era.AnnoMartyrum, copticYear, month, day, CalendarSystem.Coptic);

    /// <summary>
    /// Formats this date using the provided language and pattern.
    /// </summary>
    /// <param name="date"></param>
    /// <param name="language"></param>
    /// <param name="patternText"></param>
    /// <returns></returns>
    public static string Format(this LocalDate date, LanguageInfo language, string? patternText = null)
    {
        var culture = (System.Globalization.CultureInfo)language.Culture!.Clone();

        if (date.Calendar == CalendarSystem.Coptic)
        {
            // The BCL doesn't support the Coptic calendar at all.
            // The month names even when localized are from ISO, (Kiahk -> April).
            // This is a bit of a hack to use the correct names.

            culture.DateTimeFormat.MonthNames = Enumerable
                .Range(1, 13)
                .Select(m => GetMonthName(m, language.Known))
                .ToArray();
        }

        if (language.Language == "cop")
        {
            // TODO: Add names for other dialects
            culture.DateTimeFormat.DayNames = new[]
            {
                "Ϯⲕⲩⲣⲓⲁⲕⲏ",
                "Ⲡⲓⲥ\u0300ⲛⲁⲩ",
                "Ⲡⲓϣⲟⲙⲧ",
                "Ⲡⲓϥ\u0300ⲧⲟⲩ",
                "Ⲡⲓⲧ\u0300ⲓⲟⲩ",
                "Ⲡⲓⲥⲟⲟⲩ",
                "Ⲡⲓⲥⲁⲃⲃⲁⲧⲟⲛ",
            };
        }

        return date.ToString(patternText, culture);
    }


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
    /// Determines if today is this year's ocurrence of the given event.
    /// </summary>
    /// <param name="dateCalc">
    /// A function that returns the date of an event in given a Coptic year.
    /// </param>
    public static bool IsToday(Func<int, LocalDate> dateCalc)
    {
        var today = TodayCoptic();
        return today == dateCalc(today.YearOfEra);
    }

    /// <summary>
    /// Determines if today is during this year's ocurrence of the given event.
    /// </summary>
    /// <param name="dateCalc">
    /// A function that returns the date of an event in given a Coptic year.
    /// </param>
    public static bool IsToday(Func<int, DatePeriod> dateCalc)
    {
        var today = TodayCoptic();
        return dateCalc(today.YearOfEra).IsDuring(today);
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