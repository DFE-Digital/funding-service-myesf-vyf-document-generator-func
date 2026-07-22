using System;
using System.Collections.Generic;
using System.Linq;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Helpers
{
    /// <summary>
    /// The Date helper.
    /// </summary>
    public static class DateHelper
    {
        /// <summary>
        /// The april to august months.
        /// </summary>
        private static readonly IEnumerable<int> AprilToAugustMonths = new List<int> { 4, 5, 6, 7, 8 };

        /// <summary>
        /// The september to march months.
        /// </summary>
        private static readonly IEnumerable<int> SeptemberToMarchMonths = new List<int> { 9, 10, 11, 12, 1, 2, 3 };

        /// <summary>
        /// Determines whether [is april to august opener] [the specified month].
        /// </summary>
        /// <param name="month">The month.</param>
        /// <returns>
        ///   <c>true</c> if [is april to august opener] [the specified month]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAprilToAugustOpener(this int month)
        {
            return AprilToAugustMonths.Contains(month);
        }

        /// <summary>
        /// Determines whether [is september to march opener] [the specified month].
        /// </summary>
        /// <param name="month">The month.</param>
        /// <returns>
        ///   <c>true</c> if [is september to march opener] [the specified month]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSeptemberToMarchOpener(this int month)
        {
            return SeptemberToMarchMonths.Contains(month);
        }

        /// <summary>
        /// Determines whether [is second year in year opener] [the specified year from].
        /// </summary>
        /// <param name="inputDateTime">The input date time.</param>
        /// <param name="yearFrom">The year from.</param>
        /// <returns>
        ///   <c>true</c> if [is second year in year opener] [the specified year from]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSecondYearInYearOpener(this DateTime inputDateTime, int yearFrom)
        {
            var iyoPostAprilSecondYearStartDate = new DateTime(yearFrom, 4, 1);
            var iyoPostAprilSecondYearEndDate = new DateTime(yearFrom, 8, 31);
            return inputDateTime >= iyoPostAprilSecondYearStartDate && inputDateTime <= iyoPostAprilSecondYearEndDate;
        }

        /// <summary>
        /// Determines whether the specified date opened is an academic year in year opener.
        /// </summary>
        /// <param name="dateOpened">The date opened.</param>
        /// <param name="yearFrom">The year from.</param>
        /// <param name="yearTo">The year to.</param>
        /// <returns>
        ///  True if the specified date opened is an academic year in year opener.
        /// </returns>
        public static bool IsAcademicYearInYearOpener(this DateTime dateOpened, int yearFrom, int yearTo)
        {
            var iyoStartDate = new DateTime(yearFrom, 8, 31);
            var iyoEndDate = new DateTime(yearTo, 7, 31);

            return dateOpened >= iyoStartDate && dateOpened <= iyoEndDate;
        }
    }
}