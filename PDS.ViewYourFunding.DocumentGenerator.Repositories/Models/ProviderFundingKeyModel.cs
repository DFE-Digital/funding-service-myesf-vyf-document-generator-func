using Newtonsoft.Json;

namespace PDS.ViewYourFunding.DocumentGenerator.Repositories.Models
{
    /// <summary>
    /// The Provider Funding Key Model is used to patch a document.
    /// </summary>
    public class ProviderFundingKeyModel
    {
        /// <summary>
        /// Gets or sets the id of the document to patch.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the partition key of the document to patch.
        /// </summary>
        [JsonProperty("partitionKey")]
        public string PartitionKey { get; set; }
    }
}
