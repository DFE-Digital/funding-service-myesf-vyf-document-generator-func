using Microsoft.Azure.Cosmos;
using Pds.Core.Logging;
using PDS.ViewYourFunding.DocumentGenerator.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.DocumentGenerator.Repositories
{
    /// <summary>
    /// A cosmos db implementation of a non-relational db.
    /// </summary>
    public class CosmosDb : INonRelationalDb
    {
        private readonly Container _container;
        private readonly ILoggerAdapter<CosmosDb> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CosmosDb"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string to use.</param>
        /// <param name="dbName">The db name to use.</param>
        /// <param name="containerName">The container name to use.</param>
        /// <param name="logger">The logger adapter object.</param>
        /// <param name="connectionMode">For Local Development set this to Gatway to connect to Cosmos without issue</param>
        public CosmosDb(string connectionString, string dbName, string containerName, ILoggerAdapter<CosmosDb> logger, ConnectionMode connectionMode = ConnectionMode.Direct)
        {
            CosmosClientOptions clientOptions = new ()
            {
                ConnectionMode = connectionMode,
                AllowBulkExecution = true,
                MaxRetryAttemptsOnRateLimitedRequests = 50,
                MaxRetryWaitTimeOnRateLimitedRequests = TimeSpan.FromMinutes(5)
            };

            var cosmosClient = new CosmosClient(connectionString, clientOptions);
            _container = cosmosClient.GetDatabase(dbName).GetContainer(containerName);
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<Dictionary<string, object>> GetDocumentById(string id)
        {
            var layoutResponse = await _container.ReadItemAsync<Dictionary<string, object>>(id, new PartitionKey(id));

            if (layoutResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Document could not be found with id '{id}'");
            }

            return layoutResponse.Resource;
        }

        /// <inheritdoc/>
        public async Task SaveDocument(Dictionary<string, string> document)
        {
            var partitionKey = DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss");
            document.Add("partitionKey", partitionKey);

            await _container.CreateItemAsync(document, new PartitionKey(partitionKey));
        }

        /// <inheritdoc/>
        public async Task PatchDocument(string id, string partitionKey, List<PatchOperation> patchOperations)
        {
            await _container.PatchItemAsync<string>(id, new PartitionKey(partitionKey), patchOperations, new PatchItemRequestOptions()
            {
                EnableContentResponseOnWrite = false
            });
        }

        /// <inheritdoc/>
        public async Task PatchDocuments(string sqlQuery, List<PatchOperation> patchOperations)
        {
            List<ProviderFundingKeyModel> documents;
            do
            {
                documents = await GetDocumentsForSqlQuery<ProviderFundingKeyModel>(sqlQuery);

                if (documents.Any())
                {
                    var documentBatches = documents.Chunk(4);

                    try
                    {
                        foreach (var batch in documentBatches)
                        {
                            List<Task> tasks = new List<Task>(batch.Length);

                            foreach (var document in batch)
                            {
                                tasks.Add(_container.PatchItemAsync<string>(
                                    document.Id,
                                    new PartitionKey(document.PartitionKey),
                                    patchOperations,
                                    new PatchItemRequestOptions()
                                    {
                                        EnableContentResponseOnWrite = false
                                    }));
                            }

                            await Task.WhenAll(tasks);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex is AggregateException ag && ag.InnerExceptions.Any(a => a.Message.Contains("Response status code does not indicate success: TooManyRequests (429)")
                            || a.Message.Contains("Response status code does not indicate success: RequestTimeout (408)")))
                        {
                            _logger?.LogInformation("Too many requests or a request timeout error happened. It will retry the missing records in the next iteration.");
                            await Task.Delay(2000);
                        }
                        else if (ex.Message.Contains("Response status code does not indicate success: TooManyRequests (429)")
                            || ex.Message.Contains("Response status code does not indicate success: RequestTimeout (408)"))
                        {
                            _logger?.LogInformation("Too many requests or a request timeout error happened. It will retry the missing records in the next iteration.");
                            await Task.Delay(2000);
                        }
                        else
                        {
                            _logger?.LogInformation(ex.Message);
                            throw;
                        }
                    }
                }
            }
            while (documents.Any());
        }

        /// <inheritdoc/>
        public async Task PatchDocuments(List<string> idsAndPartitionKeys, List<PatchOperation> patchOperations)
        {
            if (idsAndPartitionKeys.Any())
            {
                var documentBatches = idsAndPartitionKeys.Chunk(4);

                try
                {
                    foreach (var batch in documentBatches)
                    {
                        List<Task> tasks = new List<Task>(batch.Length);

                        foreach (var document in batch)
                        {
                            tasks.Add(_container.PatchItemAsync<string>(
                                document.Split(':').First(),
                                new PartitionKey(document.Split(':').Last()),
                                patchOperations,
                                new PatchItemRequestOptions()
                                {
                                    EnableContentResponseOnWrite = false
                                }));
                        }

                        await Task.WhenAll(tasks);
                    }
                }
                catch (Exception ex)
                {
                    if (ex is AggregateException ag && ag.InnerExceptions.Any(a => a.Message.Contains("Response status code does not indicate success: TooManyRequests (429)")
                        || a.Message.Contains("Response status code does not indicate success: RequestTimeout (408)")))
                    {
                        _logger?.LogInformation("Too many requests or a request timeout error happened. It will retry the missing records in the next iteration.");
                        await Task.Delay(2000);
                    }
                    else if (ex.Message.Contains("Response status code does not indicate success: TooManyRequests (429)")
                        || ex.Message.Contains("Response status code does not indicate success: RequestTimeout (408)"))
                    {
                        _logger?.LogInformation("Too many requests or a request timeout error happened. It will retry the missing records in the next iteration.");
                        await Task.Delay(2000);
                    }
                    else
                    {
                        _logger?.LogInformation(ex.Message);
                        throw;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public async Task<List<T>> GetDocumentsForSqlQuery<T>(string sqlQuery)
            where T : class
        {
            var queryDefinition = new QueryDefinition(sqlQuery);
            var result = new List<T>();

            using (var feedIterator = _container.GetItemQueryIterator<T>(queryDefinition))
            {
                while (feedIterator.HasMoreResults)
                {
                    foreach (var item in await feedIterator.ReadNextAsync())
                    {
                        result.Add(item);
                    }
                }
            }

            return result;
        }
    }
}