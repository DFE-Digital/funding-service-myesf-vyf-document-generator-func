namespace PDS.ViewYourFunding.DocumentGenerator.Services.Models
{
    /// <summary>
    /// Represents the comparison result of pdfs.
    /// </summary>
    public class ComparisonResult
    {
        /// <summary>
        /// Gets or sets the name of file getting compared.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the result of file comparison.
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// Gets or sets the texts missing in compared file with respect to original file.
        /// </summary>
        public string TextMissingInComparedFile { get; set; }

        /// <summary>
        /// Gets or sets the texts missing in original file with respect to compared file.
        /// </summary>
        public string TextMissingInOriginalFile { get; set; }
    }
}
