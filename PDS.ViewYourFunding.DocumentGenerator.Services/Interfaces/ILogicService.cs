using PDS.ViewYourFunding.DocumentGenerator.Services.Messages;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.DocumentGenerator.Services
{
    /// <summary>
    /// The main logic service.
    /// </summary>
    public interface ILogicService
    {
        /// <summary>
        /// Run the main logic loop.
        /// </summary>
        /// <param name="fundingId">The funding id.</param>
        /// <param name="providerFundingId">The provider funding id.</param>
        /// <param name="fundingStreamCode">The funding stream code (e.g. PSG).</param>
        /// <param name="ukprn">The UKPRN (e.g. 12345678).</param>
        /// <param name="fundingPeriodCode">The funding period code (e.g. AY-2021).</param>
        /// <param name="providerType">The provider type (e.g. FE).</param>
        /// <param name="providerSubType">The provider sub type.</param>
        /// <param name="laName">The local authority name.</param>
        /// <param name="laCode">The local authority code.</param>
        /// <param name="cutoffDate">The cut off date (e.g. 2030-01-01).</param>
        /// <returns>The returned file path.</returns>
        Task<IEnumerable<string>> RunGenerateSingleDocument(string fundingId, string providerFundingId, string fundingStreamCode, string ukprn, string fundingPeriodCode, string providerType, string providerSubType, string laName, string laCode, string cutoffDate);

        /// <summary>
        /// Run the main logic loop.
        /// </summary>
        /// <param name="token">CancellationToken which provided by Host if there are any issues/Timeout due to which host tries to cancel the Function Execution.</param>
        /// <returns>The awaitable task.</returns>
        Task RunDocumentGeneratorTimer(CancellationToken token = default);

        /// <summary>
        /// Run the main logic loop for funding reports.
        /// </summary>
        /// <param name="groupTypeCode">The group type code.</param>
        /// <param name="excludedGroupTypeCode">The group type code to be excluded.</param>
        /// <param name="groupTypeReason">The group type reason.</param>
        /// <param name="fundingPeriodId">The funding period id.</param>
        /// <returns>The awaitable task.</returns>
        Task RunGenerateFundingReports(string groupTypeCode, string excludedGroupTypeCode, string groupTypeReason, string fundingPeriodId);

        /// <summary>
        /// Resets the document generated attribute for all fundings/provider fundings created since provided date.
        /// </summary>
        /// <param name="resetAttributeRequest">The reset attribute request.</param>
        /// <returns>The awaitable task.</returns>
        Task RunRerunDocumentGeneration(ResetAttributeRequest resetAttributeRequest);

        /// <summary>
        /// Compares pdfs between two fileshare locations.
        /// </summary>
        /// <param name="fundingStreamCode">The funding stream code for file comparison location.</param>
        /// <param name="fundingPeriodCode">The funding period code for file comparison location.</param>
        /// <param name="folderSource">The folder where files to be compared are located.</param>
        /// <param name="folderDestination">The folder where files which are to be compared against are located.</param>
        /// <returns>The awaitable task.</returns>
        Task RunPdfComparison(string fundingStreamCode, string fundingPeriodCode, string folderSource, string folderDestination);
    }
}