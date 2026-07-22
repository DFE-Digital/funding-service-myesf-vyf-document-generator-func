using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace PDS.ViewYourFunding.DocumentGenerator.Repositories.Tests
{
    /// <summary>
    /// Tests for the cosmos db service.
    /// </summary>
    [TestClass, TestCategory("Integration")]
    public class CosmosDbTests
    {
        private const string GetAllQuery = "SELECT * FROM c";

        private readonly Microsoft.Azure.Cosmos.CosmosClient _cosmosClient;
        private readonly Container _cosmosContainer;
        private readonly TestCosmosDocument _testDocument;

        /// <summary>
        /// Initializes a new instance of the <see cref="CosmosDbTests"/> class.
        /// </summary>
        public CosmosDbTests()
        {
            _cosmosClient = InitializeCosmosClientInstanceAsync().GetAwaiter().GetResult();

            _testDocument = new TestCosmosDocument
            {
                Id = "TEST_DOCUMENT",
                PartitionKey = "TEST_DOCUMENT",
            };

            _cosmosContainer = _cosmosClient.GetContainer(ConfigHelper.GetServiceConfiguration().CosmosDB_DbName, "pdfGeneratorIntegrationTests");
        }

        /// <summary>
        /// Test GetDocumentById returns the correct document.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [TestMethod]
        public async Task GetDocumentById_RealService_ReturnsCorrectDocument()
        {
            var service = GetCosmosDbService();

            // Arrange
            SeedCosmosDb().GetAwaiter().GetResult();

            var expected = new Dictionary<string, object>
            {
                { "id", "TEST_DOCUMENT" },
                { "partitionKey", "TEST_DOCUMENT" }
            };

            // Assert
            var actual = await service.GetDocumentById("TEST_DOCUMENT");
            actual.Remove("_rid");
            actual.Remove("_self");
            actual.Remove("_etag");
            actual.Remove("_attachments");
            actual.Remove("_ts");
            actual.Remove("description");
            actual.Remove("collectionName");

            // Act
            actual.Should().BeEquivalentTo(expected);
        }

        /// <summary>
        /// Test SaveDocument doesn't throw an error.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [TestMethod]
        public async Task SaveDocument_RealService_DoesntThrowException()
        {
            // Arrange
            var service = GetCosmosDbService();

            var id = $"TEST_{Guid.NewGuid()}";

            var properties = await _cosmosContainer.ReadContainerAsync();

            // Assert
            await service.SaveDocument(new Dictionary<string, string>
            {
                { "id", id }
            });

            await ClearCosmosDb();
        }

        private async Task<CosmosClient> InitializeCosmosClientInstanceAsync()
        {
            var appConfig = ConfigHelper.GetServiceConfiguration();
            var clientBuilder =
                new Microsoft.Azure.Cosmos.Fluent.CosmosClientBuilder(appConfig.CosmosDB_ConnectionString);
            CosmosClient client;

            if (appConfig?.CosmosDB_ConnectionMode == "Gateway")
            {
                client = clientBuilder
                .WithConnectionModeGateway()
                .Build();
            }
            else
            {
                client = clientBuilder
                .WithConnectionModeDirect()
                .Build();
            }

            var database = await client.CreateDatabaseIfNotExistsAsync(appConfig.CosmosDB_DbName);
            await database.Database.CreateContainerIfNotExistsAsync("pdfGeneratorIntegrationTests", "/partitionKey");

            return client;
        }

        private async Task SeedCosmosDb()
        {
            var results = await GetAllTestCosmosDocuments();
            if (!results.Any(x => x.Id == "TEST_DOCUMENT"))
            {
                await _cosmosContainer.CreateItemAsync(_testDocument);
            }
        }

        private CosmosDb GetCosmosDbService()
        {
            var configuration = ConfigHelper.GetServiceConfiguration();
            return new CosmosDb(
                configuration.CosmosDB_ConnectionString,
                configuration.CosmosDB_DbName,
                configuration.CosmosDB_ContainerName,
                GetMockLoggerAdapter().Object,
                configuration?.CosmosDB_ConnectionMode == "Gateway" ? ConnectionMode.Gateway : ConnectionMode.Direct);
        }

        private async Task<List<TestCosmosDocument>> GetAllTestCosmosDocuments()
        {
            var query = _cosmosContainer.GetItemQueryIterator<TestCosmosDocument>(new QueryDefinition(GetAllQuery));
            var results = new List<TestCosmosDocument>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        private async Task ClearCosmosDb()
        {
            var results = await GetAllTestCosmosDocuments();

            foreach (var result in results)
            {
                await _cosmosContainer.DeleteItemAsync<TestCosmosDocument>(result.Id, new PartitionKey(result.PartitionKey));
            }
        }

        private Mock<ILoggerAdapter<CosmosDb>> GetMockLoggerAdapter()
        {
            Mock<ILoggerAdapter<CosmosDb>> mockLogger = new Mock<ILoggerAdapter<CosmosDb>>(MockBehavior.Strict);

            mockLogger.Setup(l => l.LogInformation(It.IsAny<string>()));
            mockLogger.Setup(l => l.LogError(It.IsAny<Exception>(), It.IsAny<string>()));

            return mockLogger;
        }
    }
}