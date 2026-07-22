using Newtonsoft.Json;

namespace PDS.ViewYourFunding.DocumentGenerator.Repositories.Tests
{
    /// <summary>
    /// The cosmos document base class.
    /// </summary>
    public abstract class CosmosDocument
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The name of the collection.
        /// </value>
        [JsonProperty("partitionKey")]
        public string PartitionKey { get; set; }
    }
}