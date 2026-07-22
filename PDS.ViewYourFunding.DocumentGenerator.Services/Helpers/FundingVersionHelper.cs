using PDS.ViewYourFunding.DocumentGenerator.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Helpers
{
    /// <summary>
    /// Helper methods for working with funding versions.
    /// </summary>
    public static class FundingVersionHelper
    {
        /// <summary>
        /// Get master version or statement version from channel versions.
        /// </summary>
        /// <param name="channelVersions">The channel version string array.</param>
        /// <returns>The statement channel value.</returns>
        public static int? GetStatementChannelVersion(IEnumerable<ChannelVersion> channelVersions)
        {
            if (channelVersions != null && channelVersions.Any())
            {
                return channelVersions.Where(channelType => channelType.Type.Equals("Statement", StringComparison.OrdinalIgnoreCase)).First().Value;
            }

            return null;
        }

        /// <summary>
        /// Returns if this funding is the first version.
        /// </summary>
        /// <param name="fundingOrProviderFundingID">Funding or Provider Funding ID.</param>
        /// <param name="statementChannelVersion">Version of Statement Channel.</param>
        /// <returns>If the funding is first version it returns true else it return false.</returns>
        public static bool IsFirstVersionOfFunding(string fundingOrProviderFundingID, int? statementChannelVersion)
        {
            if (statementChannelVersion == 1)
            {
                return true;
            }
            else if (statementChannelVersion > 1)
            {
                return false;
            }
            else if (fundingOrProviderFundingID is not null)
            {
                return fundingOrProviderFundingID.EndsWith("-1_0");
            }
            else
            {
                return false;
            }
        }
    }
}
