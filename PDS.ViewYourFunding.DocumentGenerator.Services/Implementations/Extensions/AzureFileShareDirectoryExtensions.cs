using Pds.Core.AzureStorage.Services;
using PDS.ViewYourFunding.DocumentGenerator.Services.Constants;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Implementations.Extensions
{
    /// <summary>
    /// Helper for AzureFileShareDirectory.
    /// </summary>
    public static class AzureFileShareDirectoryExtensions
    {
        /// <summary>
        /// Creates a directory structure if it doesn't exist.
        /// </summary>
        /// <param name="azureFileShareDirectory">The azure file share directory to work with.</param>
        /// <param name="fundingStreamCode">The funding stream code.</param>
        /// <param name="academicYear">The academic year.</param>
        /// <param name="folderName">The folder name.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public static async Task CreateDirectoryForFunding(this AzureFileShareDirectory azureFileShareDirectory, string fundingStreamCode, string academicYear, string folderName)
        {
            var dateFolderName = folderName;

            await azureFileShareDirectory.CreateDirectory($"{fundingStreamCode}");

            await azureFileShareDirectory.CreateDirectory($"{fundingStreamCode}\\{academicYear}");

            await azureFileShareDirectory.CreateDirectory($"{fundingStreamCode}\\{academicYear}\\{dateFolderName}");

            await azureFileShareDirectory.CreateDirectory($"{fundingStreamCode}\\{academicYear}\\{dateFolderName}\\{FolderNames.NewStatements}");

            await azureFileShareDirectory.CreateDirectory($"{fundingStreamCode}\\{academicYear}\\{dateFolderName}\\{FolderNames.UpdatedStatements}");

            await azureFileShareDirectory.CreateDirectory($"{fundingStreamCode}\\{academicYear}\\{dateFolderName}\\{FolderNames.IndicativeStatements}");
        }
    }
}
