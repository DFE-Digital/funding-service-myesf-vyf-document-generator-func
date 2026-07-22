using PDS.ViewYourFunding.DocumentGenerator.Services.Interfaces;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Models
{
    /// <summary>
    /// A schema version 1.0 representation of a funding value object in the data provided by CFS.
    /// </summary>
    public class FundingValueNested10 : IFundingValueNested
    {
        /// <inheritdoc/>
        public double SchemaVersion { get; set; }

        /// <inheritdoc/>
        public double? TotalValue { get; set; }

        /// <summary>
        /// Gets or sets the funding lines array.
        /// </summary>
        public FundingLine[] FundingLines { get; set; }

        /// <summary>
        /// Gets or sets the calculations array.
        /// </summary>
        public Calculation[] Calculations { get; set; }
    }
}