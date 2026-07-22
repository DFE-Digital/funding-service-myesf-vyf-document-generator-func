using Newtonsoft.Json;

namespace PDS.ViewYourFunding.DocumentGenerator.Services
{
    /// <summary>
    /// Represents a variable in a JSON layout file.
    /// </summary>
    public class Variable
    {
        /// <summary>
        /// Gets or sets the name of the variable.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the variable.
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}