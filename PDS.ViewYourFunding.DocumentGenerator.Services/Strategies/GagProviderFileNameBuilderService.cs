using PDS.ViewYourFunding.DocumentGenerator.Services.Constants;
using PDS.ViewYourFunding.DocumentGenerator.Services.Enums;
using PDS.ViewYourFunding.DocumentGenerator.Services.Models;
using System;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Strategies
{
    /// <summary>
    /// The GAG Provider funding stream code file name builder service.
    /// </summary>
    /// <seealso cref="IFileNameBuilder" />
    public class GagProviderFileNameBuilderService : IFileNameBuilder
    {
        private readonly IPopulateFundingMetaData _populateFundingMetaDataService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GagProviderFileNameBuilderService"/> class.
        /// </summary>
        /// <param name="populateFundingMetaDataService">The populate funding meta data service.</param>
        public GagProviderFileNameBuilderService(IPopulateFundingMetaData populateFundingMetaDataService)
        {
            _populateFundingMetaDataService = populateFundingMetaDataService;
        }

        /// <inheritdoc/>
        public string BuildFileName(FundingDetails fundingDetails, string tokenisedFileName, int yearFrom)
        {
            var fileName = tokenisedFileName;
            var fundingDetailsMeta = _populateFundingMetaDataService.GetFundingMetaDetails(fundingDetails, yearFrom);

            var code = GetCode(fundingDetailsMeta);

            fileName = fileName.Replace(FundingMetaDataGagConstants.CodeToken, code);
            fileName = fileName.Replace(FundingMetaDataGagConstants.UkprnToken, fundingDetails.Ukprn);
            return $"{fileName}{FundingMetaDataGagConstants.FileExtension}";
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

        private static string GetCode(FundingMetaDetails fundingDetailsMeta)
        {
            if (fundingDetailsMeta.IsInYearOpener || fundingDetailsMeta.Indicative)
            {
                if (fundingDetailsMeta.IsAcademy)
                {
                    return fundingDetailsMeta.Indicative ?
                        FundingMetaDataGagConstants.AcademyIndicativeCode :
                        FundingMetaDataGagConstants.AcademyInYearOpenerYear1Year2FinalCode;
                }

                if (fundingDetailsMeta.Indicative)
                {
                    if (fundingDetailsMeta.IsSeptemberToMarchOpener)
                    {
                        return FundingMetaDataGagConstants.FreeSchoolSeptemberToMarchIndicativeCode;
                    }

                    if (fundingDetailsMeta.IsAprilToAugustOpener)
                    {
                        return FundingMetaDataGagConstants.FreeSchoolAprilToAugustIndicativeCode;
                    }
                }

                if (fundingDetailsMeta.IsInYearOpener)
                {
                    return fundingDetailsMeta.IsAprilToAugustOpener ?
                        FundingMetaDataGagConstants.FreeSchoolAprilToAugustInYearOpenerCode :
                        FundingMetaDataGagConstants.FreeSchoolSeptemberToMarchInYearOpenerCode;
                }
            }

            return FundingMetaDataGagConstants.InstitutionExistingCode;
        }
    }
}