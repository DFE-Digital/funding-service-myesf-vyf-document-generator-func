using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Microsoft.Extensions.Options;
using MoreLinq;
using Pds.Core.AzureStorage.Models;
using Pds.Core.AzureStorage.Services;
using Pds.Core.Logging;
using PDS.ViewYourFunding.DocumentGenerator.Services.Helpers;
using PDS.ViewYourFunding.DocumentGenerator.Services.Interfaces;
using PDS.ViewYourFunding.DocumentGenerator.Services.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.DocumentGenerator.Services
{
    /// <summary>
    /// Compares pdfs between two fileshare locations.
    /// </summary>
    public class FileSharePdfComparerService : IFileSharePdfComparerService
    {
        private readonly IPDFConverterService _pdfService;
        private readonly AzureFileShareDirectory _fileShareClientSource;
        private readonly AzureFileShareDirectory _fileShareClientDestination;
        private readonly ShareDirectoryClient _fileShareDirectoryDestination;
        private readonly ShareDirectoryClient _fileShareDirectorySource;
        private readonly ILoggerAdapter<FileSharePdfComparerService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSharePdfComparerService"/> class.
        /// </summary>
        /// <param name="pdfService">The pdf service.</param>
        /// <param name="connectionStringSource">The connection string for base fileshare.</param>
        /// <param name="connectionStringDestination">The connection string for destination fileshare.</param>
        /// <param name="shareName">The file share name.</param>
        /// <param name="logger">The logger adapater object.</param>
        public FileSharePdfComparerService(IPDFConverterService pdfService, string connectionStringSource, string connectionStringDestination, string shareName, ILoggerAdapter<FileSharePdfComparerService> logger)
        {
            _pdfService = pdfService;

            var configurationOptionsForClientSource = Options.Create(
              new AzureFileShareConfiguration
              {
                  FileShareName = shareName,
                  Directory = string.Empty
              });

            var configurationOptionsForClientDestination = Options.Create(
              new AzureFileShareConfiguration
              {
                  FileShareName = shareName,
                  Directory = string.Empty
              });

            _fileShareClientSource = new AzureFileShareDirectory(
                    configurationOptionsForClientSource,
                    new AzureCloudStorageAccount(connectionStringSource));

            _fileShareClientDestination = new AzureFileShareDirectory(
                    configurationOptionsForClientDestination,
                    new AzureCloudStorageAccount(connectionStringDestination));

            _fileShareDirectoryDestination = new ShareDirectoryClient(connectionStringDestination, shareName, string.Empty);

            _fileShareDirectorySource = new ShareDirectoryClient(connectionStringSource, shareName, string.Empty);

            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task ComparePdfs(string fundingStreamCode, string fundingPeriodCode, string folderSource, string folderDestination, int parallelRunSize)
        {
            var pathsSource = DestinationSaveHelper.BuildDirectoryPath(fundingStreamCode, fundingPeriodCode, folderSource);
            var pathsDestination = DestinationSaveHelper.BuildDirectoryPath(fundingStreamCode, fundingPeriodCode, folderDestination);
            var index = 0;

            foreach (var path in pathsDestination)
            {
                var comparisonResult = new List<ComparisonResult>();

                var subDirectoryDestination = _fileShareDirectoryDestination.GetSubdirectoryClient(path);
                var filesInDestination = subDirectoryDestination.GetFilesAndDirectories();

                var pathSource = pathsSource[index];
                var subDirectorySource = _fileShareDirectorySource.GetSubdirectoryClient(pathSource);
                var filesInSource = subDirectorySource.GetFilesAndDirectories();

                var additionalFilesInSource = filesInSource
                    .Where(source => !filesInDestination.Where(dest => dest.Name == source.Name).Any());
                additionalFilesInSource.ForEach(item =>
                {
                  comparisonResult.Add(new ComparisonResult { FileName = item.Name, Result = Constants.DocumentConstants.AdditionalFileAtSource });
                });

                var filesInDestinationBatched = filesInDestination.Batch(parallelRunSize);
                var batchNumber = 1;

                foreach (var batch in filesInDestinationBatched)
                {
                    _logger?.LogInformation($"Processing comparison batch {batchNumber} - started");
                    var tasks = new List<Task<ComparisonResult>>();

                    batch.ForEach(item => { tasks.Add(ComparePdf(item, path, pathSource)); });

                    var results = await Task.WhenAll(tasks);

                    comparisonResult.AddRange(results.Where(result => result != null));

                    batchNumber++;
                }

                _logger?.LogInformation($"Creating comparison result document - started");

                var comparisonDocument = _pdfService.CreateComparisonResultDocument(comparisonResult);

                using (var stream = new MemoryStream(comparisonDocument, writable: false))
                {
                    await _fileShareClientSource.Save(stream, pathSource + $"ComparisonResult_{fundingStreamCode}_{fundingPeriodCode}_{folderSource}.csv");
                }

                _logger?.LogInformation($"Creating comparison result document - ended");

                index++;
            }
        }

        private async Task<ComparisonResult> ComparePdf(ShareFileItem item, string pathDestination, string pathSouce)
        {
            if (!item.IsDirectory && !item.Name.StartsWith("ComparisonResult"))
            {
                _logger?.LogInformation($"Comparing pdf {item.Name} - started");
                var filePathSource = $"{pathSouce}{item.Name}";
                var filePathDestination = $"{pathDestination}{item.Name}";

                if (!await _fileShareClientSource.FileExists(filePathSource))
                {
                    return new ComparisonResult { FileName = item.Name, Result = Constants.DocumentConstants.FileNotFoundMessage };
                }

                var fileSource = await _fileShareClientSource.Read($"{filePathSource}");
                var fileDestination = await _fileShareClientDestination.Read($"{filePathDestination}");

                var comparisonDetails = _pdfService.ComparePdfs(fileSource, fileDestination);

                if (!comparisonDetails.Result)
                {
                    _logger?.LogInformation($"Comparing pdf {item.Name} - ended");

                    return new ComparisonResult
                    {
                        FileName = item.Name,
                        Result = Constants.DocumentConstants.FileContentsDifferentMessage,
                        TextMissingInOriginalFile = comparisonDetails.TextMissingInOriginalFile,
                        TextMissingInComparedFile = comparisonDetails.TextMissingInComparedFile
                    };
                }
            }

            _logger?.LogInformation($"Comparing pdf {item.Name} - ended");

            return null;
        }
    }
}