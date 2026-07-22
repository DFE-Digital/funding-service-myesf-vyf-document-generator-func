using PDS.ViewYourFunding.DocumentGenerator.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.DocumentGenerator.Services
{
    /// <summary>
    /// A service for dealing with provider funding documents.
    /// </summary>
    public interface IProviderFundingService : IBaseFundingService
    {
        /// <summary>
        /// Get the provider fundings for which documents are to be created.
        /// </summary>
        /// <param name="indicative">Whether to get indicative provider fundings.</param>
        /// <returns>A list of funding details for applicable provider fundings.</returns>
        Task<IEnumerable<FundingDetails>> GetProviderFundingDetailsForDocuments(bool indicative);

        /// <summary>
        /// Gets the provider funding details.
        /// </summary>
        /// <param name="providerFundingId">The provider funding identifier.</param>
        /// <param name="forLocalAuthorityName">For Local authority name.</param>
        /// <returns>The funding details.</returns>
        Task<FundingDetails> GetProviderFundingDetails(string providerFundingId, bool forLocalAuthorityName);
    }
}