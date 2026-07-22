using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Implementations.Extensions
{
    /// <summary>
    /// Extension for string type.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Adds single quotes to comma separated be string.
        /// </summary>
        /// <param name="data">The string to act on.</param>
        /// <returns>The decorated string.</returns>
        public static string DecorateWithSingleQuotes(this string data)
        {
            return string.IsNullOrWhiteSpace(data) ? string.Empty : "'" + data.Replace(",", "','") + "'";
        }

        /// <summary>
        /// Returns the list of differences between two strings.
        /// </summary>
        /// <param name="originalString">The string to use as base.</param>
        /// <param name="stringToCompare">The string to compare with.</param>
        /// <returns>The differences identified.</returns>
        public static Tuple<string, string> FindDifferences(this string originalString, string stringToCompare)
        {
            var originalStringSanitised = originalString.RemoveMultipleSpaces().Split(" ");
            var stringToCompareSanitised = stringToCompare.RemoveMultipleSpaces().Split(" ");

            var differencesInOriginalString = string.Join("|", originalStringSanitised.Except(stringToCompareSanitised));
            var differencesInResultantString = string.Join("|", stringToCompareSanitised.Except(originalStringSanitised));

            return Tuple.Create(differencesInOriginalString, differencesInResultantString);
        }

        private static string RemoveMultipleSpaces(this string data)
        {
            return Regex.Replace(data, @"\s+", " ");
        }
    }
}