using PDS.ViewYourFunding.DocumentGenerator.Services.Constants;
using PDS.ViewYourFunding.DocumentGenerator.Services.Enums;
using PDS.ViewYourFunding.DocumentGenerator.Services.Helpers;
using PDS.ViewYourFunding.DocumentGenerator.Services.Models;
using System;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Strategies
{
    /// <summary>
    /// The Funding Meta data populator service.
    /// </summary>
    /// <seealso cref="IPopulateFundingMetaData" />
    public class GagProviderFundingMetaDataService : IPopulateFundingMetaData
    {
        /// <inheritdoc/>
        public FundingMetaDetails GetFundingMetaDetails(FundingDetails fundingDetails, int yearFrom)
        {
            var calculationAndFundingCollection =
              fundingDetails.FundingValue.GetSchemaBasedCalculationAndFundingLinesCollection(fundingDetails.SchemaVersion);

            var fundingMetaDetails = new FundingMetaDetails
            {
                Indicative = fundingDetails.Indicative,
                IsAcademy = fundingDetails.OriginalProviderType?.Contains(
                    FundingMetaDataGagConstants.AcademyProviderType, StringComparison.InvariantCultureIgnoreCase) == true,
                IsFreeSchool = fundingDetails.OriginalProviderType?.Contains(
                    FundingMetaDataGagConstants.FreeSchoolProviderType, StringComparison.InvariantCultureIgnoreCase) == true
            };

            fundingMetaDetails.UpdateInYearOpenerStatus(fundingDetails, yearFrom, calculationAndFundingCollection);

            return fundingMetaDetails;
        }


        /// <inheritdoc/>
        public bool AppliesTo(string fundingStreamCode, bool isProviderFunding)
        {
            return isProviderFunding == IsProviderFunding &&
                   FundingStreamCode.ToString().Equals(fundingStreamCode, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <inheritdoc/>
        public FundingStreamCode FundingStreamCode => FundingStreamCode.Gag;

        /// <inheritdoc/>
        public bool IsProviderFunding => true;
    }
}