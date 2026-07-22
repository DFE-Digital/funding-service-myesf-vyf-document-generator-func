using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.DocumentGenerator.Services
{
    /// <summary>
    /// A service for dealing with funding documents.
    /// </summary>
    public interface IBaseFundingService
    {
        /// <summary>
        /// Add document attribute to funding document.
        /// </summary>
        /// <param name="id">The id of the document.</param>
        /// <param name="partitionKey">The partition key of the document.</param>
        /// <param name="value">The attribute value.</param>
        /// <returns>An awaitable task.</returns>
        Task AddDocumentGeneratedAttribute(string id, string partitionKey, string value);

        /// <summary>
        /// Add document attribute to funding document.
        /// </summary>
        /// <param name="idsAndPartitionKeys">The list of ids and partition keys of documents.</param>
        /// <param name="value">The attribute value.</param>
        /// <returns>An awaitable task.</returns>
        Task AddDocumentGeneratedAttributeBatch(List<string> idsAndPartitionKeys, string value);

        /// <summary>
        /// Adds the rerun date attribute for all fundings created since the datatime provided.
        /// </summary>
        /// <param name="createdSinceDatetime">The datetime to match on.</param>
        /// <param name="fundingStreamCode">The funding stream code.</param>
        /// <param name="endDatetime">The end date time.</param>
        /// <param name="rerunDate">The rerun date to be used for folder name generation when documents are regenerated.</param>
        /// <returns>An awaitable task.</returns>
        Task AddRerunDateAttribute(string createdSinceDatetime, string fundingStreamCode, string endDatetime, DateTimeOffset rerunDate);
    }
}