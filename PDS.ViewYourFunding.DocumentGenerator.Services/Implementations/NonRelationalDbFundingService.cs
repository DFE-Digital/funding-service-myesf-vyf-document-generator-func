using MoreLinq;
using PDS.ViewYourFunding.DocumentGenerator.Repositories;
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
    /// Get fundings from a non-relational db.
    /// </summary>
    public class NonRelationalDbFundingService : BaseNonRelationalDbFundingService, IFundingService
    {
        private readonly ISettingService _settingService;
        private readonly INonRelationalDb _db;
        private readonly IAuditLogService _auditLogService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NonRelationalDbFundingService"/> class.
        /// </summary>
        /// <param name="settingService">The setting service to use.</param>
        /// <param name="auditLogService">The audit service to use.</param>
        /// <param name="db">The db to use.</param>
        public NonRelationalDbFundingService(ISettingService settingService, IAuditLogService auditLogService, INonRelationalDb db)
            : base(db, "fundingStream.code")
        {
            _settingService = settingService;
            _auditLogService = auditLogService;
            _db = db;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<FundingDetails>> GetFundingDetailsForDocuments(string groupTypeCode, string excludedGroupTypeCode, string groupTypeReason, string fundingPeriodId, string folderName)
        {
            var filteredFundingStreams = _settingService.GetSetting("FilteredFundingStreams_Fundings").DecorateWithSingleQuotes();
            var createdSinceDatetime = await _auditLogService.GetLastSuccessfulRunTime();

            var excludedSqlQuery = "SELECT DISTINCT value " +
                "c.partitionKey " +
                "FROM c " +
                $"where ARRAY_CONTAINS([{filteredFundingStreams}], c.fundingStream.code) " +
                $" and c.groupingReason = '{groupTypeReason}' " +
                $"and c.organisationGroup.groupTypeCode = '{excludedGroupTypeCode}' " +
                $"and (is_defined(c.createdDate) and c.createdDate < '{createdSinceDatetime}') " +
                $"and c.fundingPeriod.id = '{fundingPeriodId}'";
            var excludedResult = await _db.GetDocumentsForSqlQuery<string>(excludedSqlQuery);

            var sqlQuery = "SELECT " +
                "c.partitionKey as Ukprn, " +
                "c.fundingVersion as FundingVersion, " +
                "(is_defined(c.channelVersion) ? c.channelVersion : []) as ChannelVersions, " +
                "(select value max(SV['value']) from SV in c.channelVersion where SV.type = 'Statement') as StatementChannelVersion, " +
                "c.id as FundingId, " +
                "c.fundingStream.code as FundingStreamCode, " +
                "c.organisationGroup.groupTypeCode as ProviderType, " +
                "c.groupingReason as  ProviderSubType, " +
                "c.organisationGroup.name as LAName, " +
                "c.fundingPeriod.id as FundingPeriodCode, " +
                "c.statusChangedDate as CutoffDate, " +
                "c.providerFundings " +
                "FROM c " +
                $"where ARRAY_CONTAINS([{filteredFundingStreams}], c.fundingStream.code) " +
                $"and c.groupingReason = '{groupTypeReason}' " +
                $"and c.organisationGroup.groupTypeCode = '{groupTypeCode}' " +
                $"and (is_defined(c.createdDate) and c.createdDate < '{createdSinceDatetime}') " +
                $"and c.fundingPeriod.id = '{fundingPeriodId}'";

            var result = await _db.GetDocumentsForSqlQuery<FundingDetails>(sqlQuery);

            var filteredResults = await FilteredResults(result, excludedResult ?? new List<string>());

            filteredResults.ForEach(x => x.FolderName = folderName);

            return filteredResults;
        }

        /// <summary>
        /// Gets the latest funding list.
        /// </summary>
        /// <param name="fundingDetails">The list of all funding.</param>
        /// <returns>The filtered list of latest funding.</returns>
        private async Task<IEnumerable<FundingDetails>> FilteredResults(List<FundingDetails> fundingDetails, List<string> excludedFundingDetails)
        {
            var fundingGroups = fundingDetails
              .OrderByDescending(funding => DateTime.Parse(funding.CutoffDate, CultureInfo.InvariantCulture))
              .GroupBy(funding => new { Value = funding.LAName, funding.ProviderType, funding.ProviderSubType, funding.FundingStreamCode, funding.FundingPeriodCode });

            var filteredValidList = new List<FundingDetails>();

            var filteredItemsToMarkAsObsolete = new List<FundingDetails>();

            foreach (var fundingGroup in fundingGroups)
            {
                filteredValidList.Add(fundingGroup.First());
                filteredItemsToMarkAsObsolete.AddRange(fundingGroup.Skip(1).ToList());
            }

            List<string> fundingIdsAndPartitions = new List<string>();
            filteredItemsToMarkAsObsolete.ForEach(x => fundingIdsAndPartitions.Add($"{x.FundingId}:{x.Ukprn}"));
            await AddDocumentGeneratedAttributeBatch(fundingIdsAndPartitions, Constants.DocumentConstants.DocumentGeneratedFalse);

            return filteredValidList.Where(item => !excludedFundingDetails.Contains(item.Ukprn));
        }
    }
}