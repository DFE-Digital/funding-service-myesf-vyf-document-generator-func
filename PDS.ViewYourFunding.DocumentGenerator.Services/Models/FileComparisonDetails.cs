namespace PDS.ViewYourFunding.DocumentGenerator.Services.Models
{
    /// <summary>
    /// Represents the differences found in two files.
    /// </summary>
    public class FileComparisonDetails
    {
        /// <summary>
        /// Gets or sets a value indicating whether files are same or not.
        /// </summary>
        public bool Result { get; set; }

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
