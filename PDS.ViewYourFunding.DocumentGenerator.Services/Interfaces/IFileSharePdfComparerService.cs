using System.Threading.Tasks;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Interfaces
{
    /// <summary>
    /// The file share pdf comparer service.
    /// </summary>
    public interface IFileSharePdfComparerService
    {
        /// <summary>
        /// Compare the pdfs in the given folder locations.
        /// </summary>
        /// <param name="fundingStreamCode">The funding stream part of folder location.</param>
        /// <param name="fundingPeriodCode">The funding period part of folder location.</param>
        /// <param name="folderSource">The folder name for source.</param>
        /// <param name="folderDestination">The folder name for destination.</param>
        /// <param name="parallelRunSize">Number of pdf comparison to run in parallel.</param>
        /// <returns>The awaitable task.</returns>
        Task ComparePdfs(string fundingStreamCode, string fundingPeriodCode, string folderSource, string folderDestination, int parallelRunSize);
    }
}
