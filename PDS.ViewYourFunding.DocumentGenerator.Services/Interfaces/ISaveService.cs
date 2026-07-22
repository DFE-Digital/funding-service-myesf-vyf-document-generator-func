using PDS.ViewYourFunding.DocumentGenerator.Services.Models;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.DocumentGenerator.Services
{
    /// <summary>
    /// An interface for saving files.
    /// </summary>
    public interface ISaveService
    {
        /// <summary>
        /// Save a file.
        /// </summary>
        /// <param name="fundingDetails">The funding details.</param>
        /// <param name="fileName">The file name.</param>
        /// <param name="data">The data as a byte array.</param>
        /// <returns>The filename.</returns>
        Task<string> Save(FundingDetails fundingDetails, string fileName, byte[] data);

        /// <summary>
        /// Creates a directory structure.
        /// </summary>
        /// <param name="fundingStreamCode">The funding stream code (e.g. PSG).</param>
        /// <param name="academicYear">The academic year.</param>
        /// <param name="folderName">The folder name.</param>
        /// <returns>The awaited task.</returns>
        Task CreateFileDirectories(string fundingStreamCode, string academicYear, string folderName);
    }
}