using System;
using System.Text.RegularExpressions;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Helpers
{
    /// <summary>
    /// Helper methods for working with funding periods.
    /// </summary>
    public static class FundingPeriodHelper
    {
        private static readonly Regex FundingPeriodCodeRegex = new Regex(@"[A-Z]{2}-\d{4}$", RegexOptions.Compiled);

        /// <summary>
        /// Get the years from a funding period code (e.g. FY-1920).
        /// </summary>
        /// <param name="fundingPeriodCode">Funding period code (e.g. FY-1920).</param>
        /// <returns>A tuple, where the first value is the first (from) year, and the second is the second (to) year.</returns>
        public static (int yearFrom, int yearTo) GetYearsFromCode(string fundingPeriodCode)
        {
            var fundingPeriod = GetFundingPeriodFromCode(fundingPeriodCode);

            var fundingPeriodStartYear = 2000 + int.Parse(fundingPeriod.Substring(0, 2));
            var fundingPeriodEndYear = 2000 + int.Parse(fundingPeriod.Substring(2, 2));

            return (fundingPeriodStartYear, fundingPeriodEndYear);
        }

        /// <summary>
        /// Get the period part from a funding period code (e.g. FY-1920).
        /// </summary>
        /// <param name="fundingPeriodCode">Funding period code (e.g. FY-1920).</param>
        /// <returns>A string representing the funding period part of the funding period code.</returns>
        public static string GetFundingPeriodFromCode(string fundingPeriodCode)
        {
            if (string.IsNullOrEmpty(fundingPeriodCode))
            {
                throw new ArgumentNullException(nameof(fundingPeriodCode));
            }

            if (!FundingPeriodCodeRegex.IsMatch(fundingPeriodCode))
            {
                throw new FormatException(nameof(fundingPeriodCode));
            }

            return fundingPeriodCode.Substring(fundingPeriodCode.Length - 4);
        }
    }
}