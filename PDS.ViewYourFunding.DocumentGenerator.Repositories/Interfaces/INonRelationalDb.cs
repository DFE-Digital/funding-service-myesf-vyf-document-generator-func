using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.DocumentGenerator.Repositories
{
    /// <summary>
    /// An interface representing a non relation db.
    /// </summary>
    public interface INonRelationalDb
    {
        /// <summary>
        /// Get a document by its id.
        /// </summary>
        /// <param name="id">The id of a document to lookup.</param>
        /// <returns>A dictionary of string/object.</returns>
        Task<Dictionary<string, object>> GetDocumentById(string id);

        /// <summary>
        /// Save a document.
        /// </summary>
        /// <param name="document">The document as a dictionary of string/object.</param>
        /// <returns>An awaitable task.</returns>
        Task SaveDocument(Dictionary<string, string> document);

        /// <summary>
        /// Patch a document.
        /// </summary>
        /// <param name="id">The id of the document to patch.</param>
        /// <param name="partitionKey">The partition key of the document to patch.</param>
        /// <param name="patchOperations">The list of patch operations to perform.</param>
        /// <returns>An awaitable task.</returns>
        Task PatchDocument(string id, string partitionKey, List<PatchOperation> patchOperations);

        /// <summary>
        /// Patch a document.
        /// </summary>
        /// <param name="sqlQuery">The sql query used to collect relevant documents to patch.</param>
        /// <param name="patchOperations">The list of patch operations to perform.</param>
        /// <returns>An awaitable task.</returns>
        Task PatchDocuments(string sqlQuery, List<PatchOperation> patchOperations);

        /// <summary>
        /// Patch a document.
        /// </summary>
        /// <param name="idsAndPartitionKeys">The list of ids and partition keys separated by colon.</param>
        /// <param name="patchOperations">The list of patch operations to perform.</param>
        /// <returns>An awaitable task.</returns>
        Task PatchDocuments(List<string> idsAndPartitionKeys, List<PatchOperation> patchOperations);

        /// <summary>
        /// Get a documents for sql query.
        /// </summary>
        /// <param name="sqlQuery">The sql query to be executed.</param>
        /// <returns>A list of matching objects.</returns>
        Task<List<T>> GetDocumentsForSqlQuery<T>(string sqlQuery)
            where T : class;
    }
}