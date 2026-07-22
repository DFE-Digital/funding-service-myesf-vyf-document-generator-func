using Newtonsoft.Json;

namespace PDS.ViewYourFunding.DocumentGenerator.Repositories.Tests
{
    /// <summary>
    /// The Test Cosmos Document class.
    /// </summary>
    /// <seealso cref="CosmosDocument" />
    public class TestCosmosDocument : CosmosDocument
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the collection.
        /// </summary>
        /// <value>
        /// The name of the collection.
        /// </value>
        [JsonProperty("collectionName")]
        public string CollectionName { get; set; }
    }
}