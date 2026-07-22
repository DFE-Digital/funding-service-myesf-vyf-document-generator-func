using Microsoft.Azure.Cosmos;
using PDS.ViewYourFunding.DocumentGenerator.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.DocumentGenerator.Services
{
    /// <summary>
    /// Get funding/provider funding from a non-relational db.
    /// </summary>
    public abstract class BaseNonRelationalDbFundingService : IBaseFundingService
    {
        private readonly INonRelationalDb _db;
        private readonly string _fundingStreamCodeFieldName;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseNonRelationalDbFundingService"/> class.
        /// </summary>
        /// <param name="db">The db to use.</param>
        /// <param name="fundingStreamCodeFieldName">The funding stream code field name.</param>
        public BaseNonRelationalDbFundingService(INonRelationalDb db, string fundingStreamCodeFieldName)
        {
            _db = db;
            _fundingStreamCodeFieldName = fundingStreamCodeFieldName;
        }

        /// <inheritdoc/>
        public async Task AddDocumentGeneratedAttribute(string id, string partitionKey, string value)
        {
            List<PatchOperation> patchOperations = new List<PatchOperation>
            {
                PatchOperation.Add($"/{Constants.DocumentConstants.DocumentGeneratedAttribute}", value)
            };

            await _db.PatchDocument(id, partitionKey, patchOperations);
        }

        /// <inheritdoc/>
        public async Task AddDocumentGeneratedAttributeBatch(List<string> idsAndPartitionKeys, string value)
        {
            List<PatchOperation> patchOperations = new List<PatchOperation>
            {
                PatchOperation.Add($"/{Constants.DocumentConstants.DocumentGeneratedAttribute}", value)
            };

            await _db.PatchDocuments(idsAndPartitionKeys, patchOperations);
        }

        /// <inheritdoc/>
        public async Task AddRerunDateAttribute(string createdSinceDatetime, string fundingStreamCode, string endDatetime, DateTimeOffset rerunDate)
        {
            var sqlQuery = "select c.id, c.partitionKey from c " +
                           "where " +
                           $"c.createdDate > '{createdSinceDatetime}' " +
                           $"and c.createdDate < '{endDatetime}' " +
                           $"and c.{_fundingStreamCodeFieldName} = '{fundingStreamCode}' " +
                           "and is_defined(c.documentGenerated) " +
                           "OFFSET 0 LIMIT 100";

            List<PatchOperation> patchOperations = new List<PatchOperation>
            {
                PatchOperation.Remove($"/{Constants.DocumentConstants.DocumentGeneratedAttribute}"),
                PatchOperation.Add($"/{Constants.DocumentConstants.RerunDateAttribute}", rerunDate)
            };

            await _db.PatchDocuments(sqlQuery, patchOperations);

            sqlQuery = "select c.id, c.partitionKey from c " +
                           "where " +
                           $"c.createdDate > '{createdSinceDatetime}' " +
                           $"and c.createdDate < '{endDatetime}' " +
                           $"and c.{_fundingStreamCodeFieldName} = '{fundingStreamCode}' " +
                           "and is_defined(c.pdfGenerated) " +
                           "OFFSET 0 LIMIT 100";

            patchOperations = new List<PatchOperation>
            {
                PatchOperation.Remove($"/{Constants.DocumentConstants.PdfGeneratedAttribute}"),
                PatchOperation.Add($"/{Constants.DocumentConstants.RerunDateAttribute}", rerunDate)
            };

            await _db.PatchDocuments(sqlQuery, patchOperations);
        }
    }
}