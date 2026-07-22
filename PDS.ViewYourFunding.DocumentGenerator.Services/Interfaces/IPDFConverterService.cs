using PDS.ViewYourFunding.DocumentGenerator.Services.Models;
using System.Collections.Generic;
using System.IO;

namespace PDS.ViewYourFunding.DocumentGenerator.Services
{
    /// <summary>
    /// A service for creating PDFs.
    /// </summary>
    public interface IPDFConverterService
    {
        /// <summary>
        /// Create a PDF from an HTML string.
        /// </summary>
        /// <param name="html">An HTML string.</param>
        /// <param name="widthMM">The width in millimeters.</param>
        /// <param name="heightMM">The height in millimeters.</param>
        /// <param name="topMarginMM">The top margin in millimeters.</param>
        /// <param name="rightMarginMM">The right margin in millimeters.</param>
        /// <param name="bottomMarginMM">The bottom margin in millimeters.</param>
        /// <param name="leftMarginMM">The left margin in millimeters.</param>
        /// <param name="bookmarks">An optional bookmarks collection.</param>
        /// <param name="title">The title.</param>
        /// <returns>The PDF as a byte array.</returns>
        byte[] CreatePdfFromHtml(
            string html,
            double? widthMM,
            double? heightMM,
            double? topMarginMM,
            double? rightMarginMM,
            double? bottomMarginMM,
            double? leftMarginMM,
            IEnumerable<Bookmark> bookmarks,
            string title);

        /// <summary>
        /// Compares pdfs.
        /// </summary>
        /// <param name="sourceFile">The source file stream for comparison.</param>
        /// <param name="destinationFile">The destination file stream for comparison.</param>
        /// <returns>Returns true or false on basis of if the files are same or different.</returns>
        FileComparisonDetails ComparePdfs(Stream sourceFile, Stream destinationFile);

        /// <summary>
        /// Generates an excel document containing the file comparison result.
        /// </summary>
        /// <param name="fileContent">Content for the file.</param>
        /// <returns>The excel as a byte array.</returns>
        byte[] CreateComparisonResultDocument(List<ComparisonResult> fileContent);
    }
}