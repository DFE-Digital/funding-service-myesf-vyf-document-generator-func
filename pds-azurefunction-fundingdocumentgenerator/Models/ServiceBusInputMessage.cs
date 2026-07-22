namespace PDS.ViewYourFunding.DocumentGenerator.FunctionApp.Models
{
    /// <summary>
    /// Service bus queue message stating a new or updated version of provider funding has been added.
    /// </summary>
    public class ServiceBusInputMessage
    {
        /// <summary>
        /// Gets or sets the funding id.
        /// </summary>
        public string FundingId { get; set; }

        /// <summary>
        /// Gets or sets the provider funding id.
        /// </summary>
        public string ProviderFundingId { get; set; }

        /// <summary>
        /// Gets or sets the FundingStreamCode for the provider funding.
        /// </summary>
        public string FundingStreamCode { get; set; }

        /// <summary>
        /// Gets or sets the Ukprn for the provider funding.
        /// </summary>
        public string Ukprn { get; set; }

        /// <summary>
        /// Gets or sets the FundingPeriodCode for the provider funding.
        /// </summary>
        public string FundingPeriodCode { get; set; }

        /// <summary>
        /// Gets or sets the CutoffDate for the provider funding.
        /// </summary>
        public string CutoffDate { get; set; }

        /// <summary>
        /// Gets or sets the provider type (e.g. Academies).
        /// </summary>
        public string ProviderType { get; set; }

        /// <summary>
        /// Gets or sets the provider sub type (e.g. Academy special converter).
        /// </summary>
        public string ProviderSubType { get; set; }

        /// <summary>
        /// Gets or sets the local authority name.
        /// </summary>
        public string LAName { get; set; }

        /// <summary>
        /// Gets or sets the local authority code.
        /// </summary>
        public string LACode { get; set; }
    }
}