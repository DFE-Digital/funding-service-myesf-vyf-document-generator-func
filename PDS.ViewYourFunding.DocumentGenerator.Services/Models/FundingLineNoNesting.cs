namespace PDS.ViewYourFunding.DocumentGenerator.Services.Models
{
    /// <summary>
    /// A funding line (simplified).
    /// </summary>
    public class FundingLineNoNesting
    {
        /// <summary>
        /// Gets or sets a template line id.
        /// </summary>
        public int TemplateLineId { get; set; }

        /// <summary>
        /// Gets or sets an object that contains a value.
        /// </summary>
        public object Value { get; set; }
    }
}