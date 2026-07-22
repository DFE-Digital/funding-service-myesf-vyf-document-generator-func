using System.Collections.Generic;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.DocumentGenerator.Services
{
    /// <summary>
    /// A service for dealing with layouts.
    /// </summary>
    public interface ILayoutService
    {
        /// <summary>
        /// Get a layout, given its id.
        /// </summary>
        /// <param name="layoutId">The layout id (a guid).</param>
        /// <returns>A dictionary of string/object.</returns>
        Task<Dictionary<string, object>> GetLayout(string layoutId);

        /// <summary>
        /// Lookup a layout id.
        /// </summary>
        /// <param name="fundingStreamCode">The funding stream code (e.g. PSG).</param>
        /// <param name="fundingPeriodCode">The funding period code (e.g. AY-2021).</param>
        /// <param name="cutoffDate">The cut off date (e.g. 2030-01-01).</param>
        /// <param name="providerType">The provider type (e.g. FE, academy).</param>
        /// <param name="providerSubType">The provider sub type.</param>
        /// <returns>The layout config key and the ids.</returns>
        (string layoutKey, List<string> layoutIds) LookupLayoutId(string fundingStreamCode, string fundingPeriodCode, string cutoffDate, string providerType, string providerSubType);

        /// <summary>
        /// Look up file name format.
        /// </summary>
        /// <param name="layoutId">The layout id setting.</param>
        /// <param name="index">The index of filename format in case there are multiple comma separated file name formats.</param>
        /// <returns>The filename format.</returns>
        string LookupFileNameFormat(string layoutId, int index);

        /// <summary>
        /// Look up file type.
        /// </summary>
        /// <param name="layoutId">The layout id setting.</param>
        /// <returns>The file type.</returns>
        string LookupFileType(string layoutId);
    }
}