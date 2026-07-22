using PDS.ViewYourFunding.DocumentGenerator.Services.Constants;
using PDS.ViewYourFunding.DocumentGenerator.Services.Models;
using System;
using System.Linq;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Helpers
{
    /// <summary>
    /// The Funding Value Helper.
    /// </summary>
    public static class FundingValueHelper
    {
        /// <summary>
        /// Extracts the calculation value.
        /// </summary>
        /// <param name="calculationAndFundingLinesCollection">The calculation and funding lines collection.</param>
        /// <param name="templateCalculationId">The template calculation identifier.</param>
        /// <returns>The calculation value.</returns>
        public static int? ExtractCalculationValue(this CalculationAndFundingLinesCollection calculationAndFundingLinesCollection, int templateCalculationId)
        {
            var calcValue = calculationAndFundingLinesCollection?.Calculations?
                .FirstOrDefault(c => c.Key == templateCalculationId).Value?.Value;
            return calcValue != null ? Convert.ToInt32(calcValue) : default;
        }

        /// <summary>
        /// Gets the in year opener status.
        /// </summary>
        /// <param name="fundingMetaDetails">The funding meta details.</param>
        /// <param name="fundingDetails">The funding details.</param>
        /// <param name="yearFrom">Year from.</param>
        /// <param name="calculationAndFundingCollection">The calculation and funding collection.</param>
        public static void UpdateInYearOpenerStatus(
            this FundingMetaDetails fundingMetaDetails, FundingDetails fundingDetails, int yearFrom, CalculationAndFundingLinesCollection calculationAndFundingCollection)
        {
            fundingMetaDetails.IsAprilToAugustOpener = fundingDetails.DateOpened.Month.IsAprilToAugustOpener();
            fundingMetaDetails.IsSeptemberToMarchOpener = fundingDetails.DateOpened.Month.IsSeptemberToMarchOpener();
            fundingMetaDetails.IsSecondYearInYearOpener = fundingDetails.DateOpened.IsSecondYearInYearOpener(yearFrom);

            var isAcademicInYearOpener = fundingDetails.DateOpened.IsAcademicYearInYearOpener(yearFrom, yearFrom + 1);
            var fullYearDays = calculationAndFundingCollection.ExtractCalculationValue(TemplateCalculationId.DaysInFullYear);
            var daysOpen = calculationAndFundingCollection.ExtractCalculationValue(TemplateCalculationId.DaysOpenInYear);

            fundingMetaDetails.IsInYearOpener = fullYearDays != daysOpen || fundingMetaDetails.IsSecondYearInYearOpener || isAcademicInYearOpener;
        }
    }
}