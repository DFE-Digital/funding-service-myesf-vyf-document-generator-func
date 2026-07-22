using PDS.ViewYourFunding.DocumentGenerator.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.DocumentGenerator.Services
{
    /// <summary>
    /// A service for dealing with funding documents.
    /// </summary>
    public interface IFundingService : IBaseFundingService
    {
        /// <summary>
        /// Get the fundings for which documents are to be created.
        /// </summary>
        /// <param name="groupTypeCode">The group type code.</param>
        /// <param name="excludedGroupTypeCode">The group type code to be excluded.</param>
        /// <param name="groupTypeReason">The group type reason.</param>
        /// <param name="fundingPeriodId">The funding period id.</param>
        /// <param name="folderName">The folder name for the documents to be saved to.</param>
        /// <returns>A list of funding details for applicable fundings.</returns>
        Task<IEnumerable<FundingDetails>> GetFundingDetailsForDocuments(string groupTypeCode, string excludedGroupTypeCode, string groupTypeReason, string fundingPeriodId, string folderName);
    }
}