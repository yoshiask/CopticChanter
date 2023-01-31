using CoptLib.Writing;
using System;

namespace CoptLib
{
    partial class DateHelper
    {
        /// <summary>
        /// Gets the name of the given Coptic/Ethiopian month.
        /// </summary>
        /// <param name="monthNum">The month to get the name of.</param>
        /// <param name="language">The language to return.</param>
        /// <returns>The month name translated to the given language.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The month number was greater than 13, or less than or equal to 0.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// No translations are available for the given language.
        /// </exception>
        public static string GetMonthName(int monthNum, KnownLanguage language)
        {
            if (monthNum > 13 || monthNum <= 0)
                throw new ArgumentOutOfRangeException(nameof(monthNum));

#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
            return language switch
            {
                KnownLanguage.Coptic or
                KnownLanguage.CopticBohairic => monthNum switch
                {
                    1 => "Ⲑⲱⲟⲩⲧ",
                    2 => "Ⲡⲁⲟⲡⲓ",
                    3 => "Ⲁⲑⲱⲣ",
                    4 => "Ⲭⲟⲓⲁⲕ",
                    5 => "Ⲧⲱⲃⲓ",
                    6 => "Ⲙⲉϣⲓⲣ",
                    7 => "Ⲡⲁⲣⲉⲙϩⲁⲧ",
                    8 => "Ⲫⲁⲣⲙⲟⲩⲑⲓ",
                    9 => "Ⲡⲁϣⲟⲛⲥ",
                    10 => "Ⲡⲁⲱⲛⲓ",
                    11 => "Ⲉⲡⲓⲡ",
                    12 => "Ⲙⲉⲥⲱⲣⲓ",
                    13 => "Ⲡⲓⲕⲟⲩϫⲓ ⲛ̀ⲁ̀ⲃⲟⲧ",
                },
                KnownLanguage.CopticSahidic => monthNum switch
                {
                    1 => "Ⲑⲟⲟⲩⲧ",
                    2 => "Ⲡⲁⲱⲡⲉ",
                    3 => "Ϩⲁⲑⲱⲣ",
                    4 => "Ⲕⲟⲓⲁϩⲕ",
                    5 => "Ⲧⲱⲃⲉ",
                    6 => "Ⲙϣⲓⲣ",
                    7 => "Ⲡⲁⲣⲙϩⲟⲧⲡ",
                    8 => "Ⲡⲁⲣⲙⲟⲩⲧⲉ",
                    9 => "Ⲡⲁϣⲟⲛⲥ",
                    10 => "Ⲡⲁⲱⲛⲉ",
                    11 => "Ⲉⲡⲏⲡ",
                    12 => "Ⲙⲉⲥⲱⲣⲏ",
                    13 => "Ⲉⲡⲁⲅⲟⲙⲉⲛⲁⲓ",
                },
                KnownLanguage.English => monthNum switch
                {
                    1 => "Thoout",
                    2 => "Paope",
                    3 => "Hathor",
                    4 => "Koiahk",
                    5 => "Tobe",
                    6 => "Meshir",
                    7 => "Paremhotep",
                    8 => "Parmoute",
                    9 => "Pashons",
                    10 => "Paone",
                    11 => "Epep",
                    12 => "Mesore",
                    13 => "Nesi",
                },
                KnownLanguage.Arabic => monthNum switch
                {
                    1 => "توت",
                    2 => "بابه",
                    3 => "هاتور",
                    4 => "كيهك",
                    5 => "طوبه",
                    6 => "أمشير",
                    7 => "برمهات",
                    8 => "برموده",
                    9 => "بشنس",
                    10 => "بؤنة",
                    11 => "أبيب",
                    12 => "مسرى",
                    13 => "نسيئ",
                },
                KnownLanguage.Amharic => monthNum switch
                {
                    1 => "መስከረም",
                    2 => "ጥቅምት",
                    3 => "ኅዳር",
                    4 => "ታኅሣሥ",
                    5 => "ጥር",
                    6 => "የካቲት",
                    7 => "መጋቢት",
                    8 => "ሚያዝያ",
                    9 => "ግንቦት",
                    10 => "ሰኔ",
                    11 => "ሐምሌ",
                    12 => "ነሐሴ",
                    13 => "ጳጐሜን",
                },

                _ => throw new ArgumentException($"No translation available for '{language}'"),
            };
#pragma warning restore CS8509
        }
    }
}
