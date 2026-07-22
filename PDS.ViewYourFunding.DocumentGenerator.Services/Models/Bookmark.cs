using Newtonsoft.Json;

namespace PDS.ViewYourFunding.DocumentGenerator.Services
{
    /// <summary>
    /// Represents a bookmark in a JSON layout file.
    /// </summary>
    public class Bookmark
    {
        /// <summary>
        /// Gets or sets the title of the bookmark.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the text to find.
        /// </summary>
        [JsonProperty("textToFind")]
        public string TextToFind { get; set; }

        /// <summary>
        /// Gets or sets the instance to match (1 indexed).
        /// </summary>
        [JsonProperty("instanceToMatch")]
        public int InstanceToMatch { get; set; } = 1;
    }
}