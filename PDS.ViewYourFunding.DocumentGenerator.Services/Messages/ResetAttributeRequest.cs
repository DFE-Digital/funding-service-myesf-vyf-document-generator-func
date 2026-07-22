namespace PDS.ViewYourFunding.DocumentGenerator.Services.Messages
{
    /// <summary>
    /// The ResetAttributeRequest message class.
    /// </summary>
    public class ResetAttributeRequest
    {
        /// <summary>
        /// Gets or sets the since created date.
        /// </summary>
        /// <value>
        /// The since created date.
        /// </value>
        public string SinceCreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the funding stream code.
        /// </summary>
        /// <value>
        /// The funding stream code.
        /// </value>
        public string FundingStreamCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to reset provider fundings.
        /// </summary>
        public bool ResetProviderFunding { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to reset fundings.
        /// </summary>
        public bool ResetFunding { get; set; } = true;

        /// <summary>
        /// Gets or sets the end date time.
        /// </summary>
        /// <value>
        /// The end date time.
        /// </value>
        public string EndDateTime { get; set; }
    }
}