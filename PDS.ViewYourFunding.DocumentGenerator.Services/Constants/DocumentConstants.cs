namespace PDS.ViewYourFunding.DocumentGenerator.Services.Constants
{
    /// <summary>
    /// Document processing constants for funding and provider funding documents.
    /// </summary>
    public static class DocumentConstants
    {
        /// <summary>
        /// Attribute representing if a pdf has been generated for a document.
        /// </summary>
        public const string PdfGeneratedAttribute = "pdfGenerated";

        /// <summary>
        /// Attribute representing if a document has been generated for a document.
        /// </summary>
        public const string DocumentGeneratedAttribute = "documentGenerated";

        /// <summary>
        /// Attribute representing the date and time which a document was requested to be regenerated.
        /// </summary>
        public const string RerunDateAttribute = "rerunDate";

        /// <summary>
        /// Value representing if a document has been generated for a document.
        /// </summary>
        public const string DocumentGeneratedTrue = "true";

        /// <summary>
        /// Value representing if a document was not generated for a document.
        /// </summary>
        public const string DocumentGeneratedFalse = "false";

        /// <summary>
        /// Message representing if the two files compared are different.
        /// </summary>
        public const string FileContentsDifferentMessage = "File Contents are different.";

        /// <summary>
        /// Message representing if the a file is missing.
        /// </summary>
        public const string FileNotFoundMessage = "File not found.";

        /// <summary>
        /// Message representing if the a file is blank.
        /// </summary>
        public const string FileIsBlankMessage = "File is blank.";

        /// <summary>
        /// Message representing additional file at source.
        /// </summary>
        public const string AdditionalFileAtSource = "Additional file at source.";

        /// <summary>
        /// The file extension for pdf.
        /// </summary>
        public const string Pdf = "pdf";

        /// <summary>
        /// The file extension for ods.
        /// </summary>
        public const string Ods = "ods";
    }
}
