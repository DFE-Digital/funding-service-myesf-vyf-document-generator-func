using PDS.ViewYourFunding.DocumentGenerator.Repositories;
using PDS.ViewYourFunding.DocumentGenerator.Services.Constants;
using PDS.ViewYourFunding.DocumentGenerator.Services.Implementations.Extensions;
using PDS.ViewYourFunding.DocumentGenerator.Services.Interfaces;
using PDS.ViewYourFunding.DocumentGenerator.Services.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.DocumentGenerator.Services
{
    /// <summary>
    /// Get provider fundings from a non-relational db.
    /// </summary>
    public class NonRelationalDbProviderFundingService : BaseNonRelationalDbFundingService, IProviderFundingService
    {
        private readonly ISettingService _settingService;
        private readonly INonRelationalDb _db;
        private readonly IAuditLogService _auditLogService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NonRelationalDbProviderFundingService"/> class.
        /// </summary>
        /// <param name="settingService">The setting service to use.</param>
        /// <param name="auditLogService">The audit service to use.</param>
        /// <param name="db">The db to use.</param>
        public NonRelationalDbProviderFundingService(ISettingService settingService, IAuditLogService auditLogService, INonRelationalDb db)
            : base(db, "fundingStreamCode")
        {
            _settingService = settingService;
            _auditLogService = auditLogService;
            _db = db;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<FundingDetails>> GetProviderFundingDetailsForDocuments(bool indicative)
        {
            var filteredVariations = _settingService.GetSetting("FilteredVariations_ProviderFundings");

            var variationQueryPart = string.Empty;

            if (!string.IsNullOrEmpty(filteredVariations))
            {
                var variations = filteredVariations.Split(',').ToList();
                variations.ForEach(variation =>
                {
                    variationQueryPart += $"or ARRAY_CONTAINS(c.variationReasons, '{variation}') ";
                });
            }

            var sqlQuery = await GetProviderFundingSqlQuery(indicative, variationQueryPart);

            var result = await _db.GetDocumentsForSqlQuery<FundingDetails>(sqlQuery);

            return await FilteredResults(result);
        }

        /// <inheritdoc/>
        public async Task<FundingDetails> GetProviderFundingDetails(string providerFundingId, bool forLocalAuthorityName)
        {
            string sqlQuery;
            if (forLocalAuthorityName)
            {
                sqlQuery = "SELECT Distinct " +
                           "c.id as ProviderFundingId, " +
                           "(is_defined(c.channelVersion) ? c.channelVersion : []) as ChannelVersions, " +
                           "(select value max(SV['value']) from SV in c.channelVersion where SV.type = 'Statement') as StatementChannelVersion, " +
                           "c.fundingStreamCode as FundingStreamCode, " +
                           "c.provider.providerType as ProviderType, " +
                           "c.provider.providerSubType as ProviderSubType, " +
                           "c.fundingPeriodId as FundingPeriodCode, " +
                           "c.statusChangedDate as CutoffDate, " +
                           "c.provider.identifier as Ukprn, " +
                           "(is_defined(c.rerunDate) ? c.rerunDate : (select value max(PI['statusChangedDate']) from PI in c.parentInformation)) as StatusChangedDateRaw, " +
                           "c.partitionKey as PartitionKey, " +
                           "parentInfo['group'].name as LaName, " +
                           "parentInfo['group'].groupTypeIdentifier['value'] as LaCode " +
                           "FROM c " +
                           "join parentInfo in c.parentInformation " +
                           $"where c.id = '{providerFundingId}' " +
                           "and parentInfo['group'].groupTypeCode ='LocalAuthority' " +
                           "and parentInfo['group'].groupTypeIdentifier.type ='LACode'";
            }
            else
            {
                sqlQuery = CosmosConstants.ProviderFundingSelectStatement +
                           $"where c.id = '{providerFundingId}' ";
            }

            var result = await _db.GetDocumentsForSqlQuery<FundingDetails>(sqlQuery);

            return result.FirstOrDefault();
        }

        private async Task<string> GetProviderFundingSqlQuery(
        bool indicative,
        string variationQueryPart)
        {
            var filteredFundingStreams = _settingService.GetSetting("FilteredFundingStreams_ProviderFundings").DecorateWithSingleQuotes();

            var indicativeConfiguration = _settingService.GetIndicativeConfiguration();

            var createdSinceDatetime = await _auditLogService.GetLastSuccessfulRunTime();
            string sqlQuery;
            if (indicative)
            {
                sqlQuery = CosmosConstants.ProviderFundingSelectStatement +
                           "join parentInfo in c.parentInformation " +
                           $"where ARRAY_CONTAINS([{filteredFundingStreams}], c.fundingStreamCode) " +
                           $"and (is_defined(c.createdDate) and c.createdDate < '{createdSinceDatetime}') " +
                           $"and(c.fundingVersion = '1_0' {variationQueryPart}) " +
                           $"and ARRAY_CONTAINS([{indicativeConfiguration.IndicativeProviderStatusList.DecorateWithSingleQuotes()}], c.provider.providerDetails.status) " +
                           $"and parentInfo.groupingReason = '{indicativeConfiguration.IndicativeGroupingReason}' " +
                           "and (NOT is_defined(c.pdfGenerated) and NOT is_defined(c.documentGenerated))";
            }
            else
            {
                sqlQuery = CosmosConstants.ProviderFundingSelectStatement +
                           $"where ARRAY_CONTAINS([{filteredFundingStreams}], c.fundingStreamCode) " +
                           $"and (is_defined(c.createdDate) and c.createdDate < '{createdSinceDatetime}') " +
                           $"and(c.fundingVersion = '1_0' {variationQueryPart}) " +
                           $"and NOT ARRAY_CONTAINS([{indicativeConfiguration.IndicativeProviderStatusList.DecorateWithSingleQuotes()}], c.provider.providerDetails.status) " +
                           "and (NOT is_defined(c.pdfGenerated) and NOT is_defined(c.documentGenerated))";
            }

            return sqlQuery;
        }

        /// <summary>
        /// Gets the latest funding list.
        /// </summary>
        /// <param name="fundingDetails">The list of all funding.</param>
        /// <returns>The filtered list of latest funding.</returns>
        private async Task<IEnumerable<FundingDetails>> FilteredResults(List<FundingDetails> fundingDetails)
        {
            var fundingGroups = fundingDetails
                .OrderByDescending(funding => DateTime.Parse(funding.CutoffDate, CultureInfo.InvariantCulture))
                .GroupBy(funding => new { Value = funding.Ukprn, funding.FundingStreamCode, funding.FundingPeriodCode });

            var filteredValidList = new List<FundingDetails>();

            var filteredItemsToMarkAsObsolete = new List<FundingDetails>();

            foreach (var fundingGroup in fundingGroups)
            {
                filteredValidList.Add(fundingGroup.First());
                filteredItemsToMarkAsObsolete.AddRange(fundingGroup.Skip(1).ToList());
            }

            List<string> fundingIdsAndPartitions = new List<string>();
            filteredItemsToMarkAsObsolete.ForEach(x => fundingIdsAndPartitions.Add($"{x.ProviderFundingId}:{x.PartitionKey}"));
            await AddDocumentGeneratedAttributeBatch(fundingIdsAndPartitions, DocumentConstants.DocumentGeneratedFalse);

            return filteredValidList;
        }
    }
}