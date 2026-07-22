using PDS.ViewYourFunding.DocumentGenerator.Services.Interfaces;
using System.Collections.Generic;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Models
{
    /// <summary>
    /// A schema version 1.2 and above representation of a funding value object in the data provided by CFS.
    /// </summary>
    public class FundingValueNested12 : IFundingValueNested
    {
        /// <inheritdoc/>
        public double SchemaVersion { get; set; }

        /// <inheritdoc/>
        public double? TotalValue { get; set; }

        /// <summary>
        /// Gets or sets the funding lines object.
        /// </summary>
        public IList<FundingLineNoNesting> FundingLines { get; set; }

        /// <summary>
        /// Gets or sets the calculations dictionary.
        /// </summary>
        public IList<CalculationNoNesting> Calculations { get; set; }
    }
}