using PDS.ViewYourFunding.DocumentGenerator.Services.Enums;
using PDS.ViewYourFunding.DocumentGenerator.Services.Models;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Strategies
{
    /// <summary>
    /// The file name builder Interface.
    /// </summary>
    public interface IFileNameBuilder
    {
        /// <summary>
        /// Builds the name of the file.
        /// </summary>
        /// <param name="fundingDetails">The funding details.</param>
        /// <param name="tokenisedFileName">Name of the tokenised file.</param>
        /// <param name="yearFrom">The year from.</param>
        /// <returns>The file name.</returns>
        string BuildFileName(FundingDetails fundingDetails, string tokenisedFileName, int yearFrom);

        /// <summary>
        /// Applies to.
        /// </summary>
        /// <param name="fundingStreamCode">The funding stream code.</param>
        /// <param name="isProviderFunding">if set to <c>true</c> [is provider funding].</param>
        /// <returns>True if it applies to this funding stream code and funding details type.</returns>
        bool AppliesTo(string fundingStreamCode, bool isProviderFunding);

        /// <summary>
        /// Gets the funding stream code.
        /// </summary>
        /// <value>
        /// The funding stream code.
        /// </value>
        FundingStreamCode FundingStreamCode { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is provider funding.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is provider funding; otherwise, <c>false</c>.
        /// </value>
        bool IsProviderFunding { get; }
    }
}
