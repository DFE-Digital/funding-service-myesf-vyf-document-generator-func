using Microsoft.Extensions.Options;
using Pds.Core.AzureStorage.Models;
using Pds.Core.AzureStorage.Services;
using PDS.ViewYourFunding.DocumentGenerator.Services.Helpers;
using PDS.ViewYourFunding.DocumentGenerator.Services.Implementations.Extensions;
using PDS.ViewYourFunding.DocumentGenerator.Services.Models;
using System.IO;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.DocumentGenerator.Services
{
    /// <summary>
    /// Save files to file share storage.
    /// </summary>
    public class FileShareSaveService : ISaveService
    {
        private readonly AzureFileShareDirectory _fileShareClientInternal;
        private readonly AzureFileShareDirectory _fileShareClientBusiness;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileShareSaveService"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string to use.</param>
        /// <param name="fileShareNameInternal">The file share name for VYF internal.</param>
        /// <param name="fileShareNameBusiness">The file share name for business.</param>
        public FileShareSaveService(string connectionString, string fileShareNameInternal, string fileShareNameBusiness)
        {
            var azureCloudStorageAccount = new AzureCloudStorageAccount(connectionString);

            var configurationOptionsForClientInternal = Options.Create(
              new AzureFileShareConfiguration
              {
                  FileShareName = fileShareNameInternal,
                  Directory = string.Empty
              });

            var configurationOptionsForClientBusiness = Options.Create(
              new AzureFileShareConfiguration
              {
                  FileShareName = fileShareNameBusiness,
                  Directory = string.Empty
              });

            _fileShareClientInternal = new AzureFileShareDirectory(
                    configurationOptionsForClientInternal,
                    azureCloudStorageAccount);
            _fileShareClientBusiness = new AzureFileShareDirectory(
                    configurationOptionsForClientBusiness,
                    azureCloudStorageAccount);
        }

        /// <inheritdoc/>
        public async Task<string> Save(FundingDetails fundingDetails, string fileName, byte[] data)
        {
            var savePath = DestinationSaveHelper.BuildFileNameWithPath(
                fundingDetails.FundingStreamCode,
                fundingDetails.FundingPeriodCode,
                fundingDetails.FundingId ?? fundingDetails.ProviderFundingId,
                fundingDetails.Indicative,
                fundingDetails.StatementChannelVersion,
                fileName,
                fundingDetails.FolderName);

            await using (var stream = new MemoryStream(data, false))
            {
                await _fileShareClientInternal.Save(stream, savePath);
                await _fileShareClientBusiness.Save(stream, savePath);
            }

            return savePath;
        }

        /// <inheritdoc/>
        public async Task CreateFileDirectories(string fundingStreamCode, string academicYear, string folderName)
        {
            await _fileShareClientInternal.CreateDirectoryForFunding(fundingStreamCode, academicYear, folderName);
            await _fileShareClientBusiness.CreateDirectoryForFunding(fundingStreamCode, academicYear, folderName);
        }
    }
}