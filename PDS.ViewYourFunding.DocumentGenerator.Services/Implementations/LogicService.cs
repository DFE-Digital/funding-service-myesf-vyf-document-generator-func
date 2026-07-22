using Microsoft.WindowsAzure.Storage;
using MoreLinq;
using Newtonsoft.Json.Linq;
using Pds.Core.Logging;
using PDS.ViewYourFunding.DocumentGenerator.Services.Helpers;
using PDS.ViewYourFunding.DocumentGenerator.Services.Interfaces;
using PDS.ViewYourFunding.DocumentGenerator.Services.Messages;
using PDS.ViewYourFunding.DocumentGenerator.Services.Models;
using PDS.ViewYourFunding.DocumentGenerator.Services.Strategies;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.DocumentGenerator.Services
{
    /// <summary>
    /// The function main logic.
    /// </summary>
    public class LogicService : ILogicService
    {
        #region Private fields

        private readonly IHttpService _httpService;
        private readonly IPDFConverterService _pdfConverterService;
        private readonly IFileSharePdfComparerService _fileSharePdfComparerService;
        private readonly ILayoutService _layoutService;
        private readonly IFundingService _fundingService;
        private readonly IProviderFundingService _providerFundingService;
        private readonly ISaveService _saveService;
        private readonly IAuditLogService _auditLogService;
        private readonly ISettingService _settingService;
        private readonly IEnumerable<IFileNameBuilder> _fileNameBuilders;
        private readonly IDateTimeService _dateTimeService;
        private ILoggerAdapter<LogicService> _logger;

        #endregion Private fields


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LogicService"/> class.
        /// </summary>
        /// <param name="pdfConverterService">The PDF converter service.</param>
        /// <param name="fileSharePdfComparerService">The file share PDF comparer service.</param>
        /// <param name="httpService">The HTTP service.</param>
        /// <param name="layoutService">The layout service.</param>
        /// <param name="fundingService">The funding service.</param>
        /// <param name="providerFundingService">The provider funding service.</param>
        /// <param name="saveService">The save service.</param>
        /// <param name="auditLogService">The audit log service.</param>
        /// <param name="settingService">The setting service.</param>
        /// <param name="fileNameBuilders">The file name builders.</param>
        /// <param name="dateTimeService">The Date Time service.</param>
        /// <param name="logger">The logger adapter object.</param>
        public LogicService(
            IPDFConverterService pdfConverterService,
            IFileSharePdfComparerService fileSharePdfComparerService,
            IHttpService httpService,
            ILayoutService layoutService,
            IFundingService fundingService,
            IProviderFundingService providerFundingService,
            ISaveService saveService,
            IAuditLogService auditLogService,
            ISettingService settingService,
            IEnumerable<IFileNameBuilder> fileNameBuilders,
            IDateTimeService dateTimeService,
            ILoggerAdapter<LogicService> logger)
        {
            _pdfConverterService = pdfConverterService;
            _fileSharePdfComparerService = fileSharePdfComparerService;
            _httpService = httpService;
            _layoutService = layoutService;
            _fundingService = fundingService;
            _providerFundingService = providerFundingService;
            _saveService = saveService;
            _auditLogService = auditLogService;
            _settingService = settingService;
            _fileNameBuilders = fileNameBuilders;
            _dateTimeService = dateTimeService;
            _logger = logger;
        }

        #endregion Constructor

        /// <inheritdoc/>
        public async Task<IEnumerable<string>> RunGenerateSingleDocument(string fundingId, string providerFundingId, string fundingStreamCode, string ukprn, string fundingPeriodCode, string providerType, string providerSubType, string laName, string laCode, string cutoffDate)
        {
            try
            {
                var fundingDetail = new FundingDetails
                {
                    FundingId = fundingId,
                    ProviderFundingId = providerFundingId,
                    FundingStreamCode = fundingStreamCode,
                    Ukprn = ukprn,
                    FundingPeriodCode = fundingPeriodCode,
                    ProviderType = providerType,
                    ProviderSubType = providerSubType,
                    LAName = laName,
                    LACode = laCode,
                    CutoffDate = cutoffDate
                };

                await UpdateFundingDetails(providerType, providerSubType, fundingDetail);

                return await GenerateDocuments(fundingDetail, providerFundingId != null, false);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task RunDocumentGeneratorTimer(CancellationToken token = default)
        {
            var noRunningInstanceOfFeedReader = await _auditLogService.CheckNoRunningInstanceOfFeedReader();

            if (noRunningInstanceOfFeedReader)
            {
                token.ThrowIfCancellationRequested();

                await ProcessProviderFundings(false, token);

                token.ThrowIfCancellationRequested();

                await ProcessProviderFundings(true, token);
            }
        }

        /// <inheritdoc/>
        public async Task RunGenerateFundingReports(string groupTypeCode, string excludedGroupTypeCode, string groupTypeReason, string fundingPeriodCode)
        {
            var noRunningInstanceOfFeedReader = await _auditLogService.CheckNoRunningInstanceOfFeedReader();

            if (noRunningInstanceOfFeedReader)
            {
                var folderName = _dateTimeService.GetDateTimePathComponent();
                var fundings = await _fundingService.GetFundingDetailsForDocuments(groupTypeCode, excludedGroupTypeCode ?? string.Empty, groupTypeReason, fundingPeriodCode, folderName);

                _logger?.LogInformation(
                    $"Found {fundings.Count()} - for groupTypeCode : {groupTypeCode}, excludedGroupTypeCode : {excludedGroupTypeCode}, groupTypeReason : {groupTypeReason}, fundingPeriodCode : {fundingPeriodCode}");

                if (fundings.Any())
                {
                    await ProcessFundings(fundings, false);
                }
            }
        }

        /// <inheritdoc/>
        public async Task RunRerunDocumentGeneration(ResetAttributeRequest resetAttributeRequest)
        {
            var rerunDate = DateTimeOffset.Now;

            if (resetAttributeRequest.ResetFunding)
            {
                _logger?.LogInformation("Resetting document attribute for fundings started");

                await _fundingService.AddRerunDateAttribute(
                    resetAttributeRequest.SinceCreatedDate,
                    resetAttributeRequest.FundingStreamCode,
                    resetAttributeRequest.EndDateTime,
                    rerunDate);

                _logger?.LogInformation("Resetting document attribute for fundings ended");
            }

            if (resetAttributeRequest.ResetProviderFunding)
            {
                _logger?.LogInformation("Resetting document attribute for provider fundings started");

                await _providerFundingService.AddRerunDateAttribute(
                    resetAttributeRequest.SinceCreatedDate,
                    resetAttributeRequest.FundingStreamCode,
                    resetAttributeRequest.EndDateTime,
                    rerunDate);

                _logger?.LogInformation("Resetting document attribute for provider fundings ended");
            }
        }

        /// <inheritdoc/>
        public async Task RunPdfComparison(string fundingStreamCode, string fundingPeriodCode, string folderSource, string folderDestination)
        {
            _logger?.LogInformation($"Comparing pdfs for {fundingStreamCode} {fundingPeriodCode} {folderSource}  started");

            var parallelRunSize = int.Parse(_settingService.GetSetting("Processing_Run_Size_Comparison"));
            await _fileSharePdfComparerService.ComparePdfs(fundingStreamCode, fundingPeriodCode, folderSource, folderDestination, parallelRunSize);

            _logger?.LogInformation($"Comparing pdfs for {fundingStreamCode} {fundingPeriodCode} {folderSource} ended");
        }

        #region Helper methods

        private async Task UpdateFundingDetails(
            string providerType,
            string providerSubType,
            FundingDetails fundingDetail)
        {
            fundingDetail.Indicative
                = nameof(fundingDetail.Indicative).Equals(
                                           providerType,
                                           StringComparison.InvariantCultureIgnoreCase) &&
                                       nameof(fundingDetail.Indicative).Equals(
                                           providerSubType,
                                           StringComparison.InvariantCultureIgnoreCase);
            var providerDetails =
                await _providerFundingService.GetProviderFundingDetails(
                    fundingDetail.ProviderFundingId, false);

            if (providerDetails != null)
            {
                fundingDetail.OriginalProviderType = providerDetails.OriginalProviderType;
                fundingDetail.DateOpenedRaw = providerDetails.DateOpenedRaw;
                fundingDetail.FundingValue = providerDetails.FundingValue;
                fundingDetail.ChannelVersions = providerDetails.ChannelVersions;
                fundingDetail.StatementChannelVersion = providerDetails.StatementChannelVersion;
                fundingDetail.FolderName = _dateTimeService.GetDateTimePathComponent();
            }
        }

        private async Task ProcessProviderFundings(bool indicative = false, CancellationToken token = default)
        {
            var providerFundings = await _providerFundingService.GetProviderFundingDetailsForDocuments(indicative);

            token.ThrowIfCancellationRequested();

            var fundingDetailsList = providerFundings.ToList();
            if (fundingDetailsList.Any())
            {
                fundingDetailsList.ForEach(x =>
                {
                    x.FolderName = _dateTimeService.GetDateTimePathComponent(x.StatusChangedDate);
                    if (indicative)
                    {
                        x.Indicative = true;
                        x.ProviderType = nameof(x.Indicative);
                        x.ProviderSubType = nameof(x.Indicative);
                    }
                });

                token.ThrowIfCancellationRequested();

                await ProcessFundings(fundingDetailsList, true, token);
            }
        }

        private async Task ProcessFundings(IEnumerable<FundingDetails> fundingDetails, bool isProviderFunding, CancellationToken token = default)
        {
            var parallelRunBatchSize = int.Parse(_settingService.GetSetting("Parallel_Run_Batch_Size"));

            var batchesOfFundings = fundingDetails.Batch(parallelRunBatchSize);

            foreach (var batchesOfFunding in batchesOfFundings)
            {
                var tasks = new List<Task<IEnumerable<string>>>(batchesOfFunding.Count());
                batchesOfFunding.ForEach(fundings =>
                                                {
                                                    tasks.Add(GenerateDocuments(fundings, isProviderFunding, true, token));
                                                });
                await Task.WhenAll(tasks);

                token.ThrowIfCancellationRequested();
            }
        }

        private async Task AddDocumentGeneratedAttribute(bool isProviderFunding, FundingDetails fundingDetails)
        {
            if (isProviderFunding)
            {
                await _providerFundingService.AddDocumentGeneratedAttribute(fundingDetails.ProviderFundingId, fundingDetails.PartitionKey, Constants.DocumentConstants.DocumentGeneratedTrue);
            }
            else
            {
                await _fundingService.AddDocumentGeneratedAttribute(fundingDetails.FundingId, fundingDetails.PartitionKey, Constants.DocumentConstants.DocumentGeneratedTrue);
            }
        }

        private async Task<IEnumerable<string>> GenerateDocuments(FundingDetails fundingDetails, bool isProviderFunding, bool addAttribute, CancellationToken token = default)
        {
            var id = isProviderFunding ? fundingDetails.ProviderFundingId : fundingDetails.FundingId;
            var providerPart = isProviderFunding ? "provider " : string.Empty;
            var result = new List<string>();

            try
            {
                _logger?.LogInformation($"Generating document for {providerPart}funding -{id} -started");

                var cutoffDate = DateTime.Parse(fundingDetails.CutoffDate, CultureInfo.InvariantCulture).Date.AddYears(1).ToString("yyyy-MM-dd");

                var layoutDetails = _layoutService.LookupLayoutId(fundingDetails.FundingStreamCode, fundingDetails.FundingPeriodCode, cutoffDate, fundingDetails.ProviderType, fundingDetails.ProviderSubType);
                var index = 0;

                foreach (var layoutId in layoutDetails.layoutIds)
                {
                    token.ThrowIfCancellationRequested();

                    var dtStart = DateTime.Now;
                    _logger?.LogInformation($"Getting html from API for {providerPart}funding -{id} -started");

                    var layoutFile = await _layoutService.GetLayout(layoutId);
                    var totalSeconds = (DateTime.Now - dtStart).TotalSeconds;

                    _logger?.LogInformation($"Getting html from API for {providerPart}funding -{id} -ended in {totalSeconds} seconds");

                    token.ThrowIfCancellationRequested();

                    var tokenisedFileName = _layoutService.LookupFileNameFormat(layoutDetails.layoutKey, index++);

                    var fileType = _layoutService.LookupFileType(layoutDetails.layoutKey);

                    var years = FundingPeriodHelper.GetYearsFromCode(fundingDetails.FundingPeriodCode);
                    var fundingPeriod = FundingPeriodHelper.GetFundingPeriodFromCode(fundingDetails.FundingPeriodCode);

                    if (!isProviderFunding
                        && tokenisedFileName.Contains(nameof(FundingDetails.LACode), StringComparison.InvariantCultureIgnoreCase)
                        && string.IsNullOrEmpty(fundingDetails.LACode))
                    {
                        await AddLocalAuthorityDetailsToFunding(fundingDetails);
                    }

                    token.ThrowIfCancellationRequested();

                    string filename;
                    var fileNameBuilderService = _fileNameBuilders?.FirstOrDefault(service =>
                        service.AppliesTo(fundingDetails.FundingStreamCode, isProviderFunding));

                    if (fileNameBuilderService != null)
                    {
                        filename = fileNameBuilderService.BuildFileName(fundingDetails, tokenisedFileName, years.yearFrom);
                    }
                    else
                    {
                        filename = DestinationSaveHelper.BuildFilename(tokenisedFileName, fundingDetails.FundingStreamCode, fundingDetails.Ukprn, fundingPeriod, years.yearFrom.ToString(), years.yearTo.ToString(), fundingDetails.LAName, fundingDetails.LACode ?? "LA Code", fundingDetails.FolderName, fileType);
                    }

                    dtStart = DateTime.Now;

                    byte[] generatedData;

                    if (fileType == Constants.DocumentConstants.Ods)
                    {
                        _logger?.LogInformation($"Creating document from file for {providerPart}funding -{id} -started");

                        generatedData = await GetFileFromApi(fundingDetails.FundingId, fundingDetails.ProviderFundingId, fundingDetails.FundingStreamCode, fundingDetails.FundingPeriodCode, cutoffDate, fundingDetails.Ukprn, layoutId);

                        totalSeconds = (DateTime.Now - dtStart).TotalSeconds;
                        _logger?.LogInformation($"Creating document from file for {providerPart}funding -{id} -ended in {totalSeconds} seconds");
                    }
                    else
                    {
                        _logger?.LogInformation($"Creating pdf from html for {providerPart}funding -{id} -started");

                        var renderedHtml = await GetHtmlFromApi(fundingDetails.FundingId, fundingDetails.ProviderFundingId, fundingDetails.FundingStreamCode, fundingDetails.FundingPeriodCode, cutoffDate, fundingDetails.Ukprn, layoutId);

                        generatedData = CreatePDF(renderedHtml, layoutFile, fundingDetails.FundingPeriodCode, filename);

                        totalSeconds = (DateTime.Now - dtStart).TotalSeconds;
                        _logger?.LogInformation($"Creating pdf from html for {providerPart}funding -{id} -ended in {totalSeconds} seconds");
                    }

                    token.ThrowIfCancellationRequested();

                    dtStart = DateTime.Now;

                    _logger?.LogInformation($"Save to fileshare for {providerPart}funding -{id} -started");

                    await _saveService.CreateFileDirectories(fundingDetails.FundingStreamCode, fundingDetails.FundingPeriodCode, fundingDetails.FolderName);
                    result.Add(await _saveService.Save(fundingDetails, filename, generatedData));

                    totalSeconds = (DateTime.Now - dtStart).TotalSeconds;
                    _logger?.LogInformation($"Save to fileshare for {providerPart}funding -{id} -ended in {totalSeconds} seconds");

                    if (addAttribute)
                    {
                        await AddDocumentGeneratedAttribute(isProviderFunding, fundingDetails);
                    }
                }

                _logger?.LogInformation($"Generating document for {providerPart}funding -{id} -ended");
            }
            catch (OperationCanceledException exception)
            {
                _logger?.LogInformation(exception, $"Host requested Cancellation and we gracefully canceled current task for {providerPart}funding -{id}!");
                throw;
            }
            catch (StorageException exception)
            {
                _logger?.LogInformation(exception, $"Azure Storage exception occured - Stopping execution - {exception.Message} ");
                throw;
            }
            catch (Exception exception)
            {
                _logger?.LogInformation(exception, $"Error occured while generating document for {providerPart}funding -{id}- Not stopping execution. - {exception.Message}");
                return new List<string>();
            }

            return result;
        }

        private async Task AddLocalAuthorityDetailsToFunding(FundingDetails fundingDetails)
        {
            if (fundingDetails.ProviderFundings.Any())
            {
                var providerDetails =
                    await _providerFundingService.GetProviderFundingDetails(
                        fundingDetails.ProviderFundings.First(), true);

                if (providerDetails != null)
                {
                    fundingDetails.LACode = providerDetails.LACode;
                    fundingDetails.LAName = providerDetails.LAName;
                }
            }
        }

        private async Task<string> GetHtmlFromApi(string fundingId, string providerFundingId, string fundingStreamCode, string fundingPeriodCode, string cutoffDate, string ukprn, string layoutId)
        {
            var requestUri = GetRequestUri(fundingId, providerFundingId, fundingStreamCode, fundingPeriodCode, cutoffDate, ukprn, layoutId);
            return await _httpService.ReadAsStringAsync(requestUri);
        }

        private string GetRequestUri(string fundingId, string providerFundingId, string fundingStreamCode, string fundingPeriodCode, string cutoffDate, string ukprn, string layoutId)
        {
            var requestUri = "view-latest-funding/api/external/render";
            requestUri += $"?fundingId={fundingId}&providerFundingId={providerFundingId}&fundingStreamCode={fundingStreamCode}&fundingPeriodCode={fundingPeriodCode}&cutoffDate={cutoffDate}&ukprn={ukprn}&layoutId={layoutId}";

            return requestUri;
        }

        private async Task<byte[]> GetFileFromApi(string fundingId, string providerFundingId, string fundingStreamCode, string fundingPeriodCode, string cutoffDate, string ukprn, string layoutId)
        {
            var requestUri = GetRequestUriForFiles(fundingId, providerFundingId, fundingStreamCode, fundingPeriodCode, cutoffDate, ukprn, layoutId);
            return await _httpService.ReadAsByteArrayAsync(requestUri);
        }

        private string GetRequestUriForFiles(string fundingId, string providerFundingId, string fundingStreamCode, string fundingPeriodCode, string cutoffDate, string ukprn, string layoutId)
        {
            var requestUri = "view-latest-funding/api/external/getFile";
            requestUri += $"?fundingId={fundingId}&providerFundingId={providerFundingId}&fundingStreamCode={fundingStreamCode}&fundingPeriodCode={fundingPeriodCode}&publicationDate={cutoffDate}&cutoffDate={cutoffDate}&ukprn={ukprn}&layoutId={layoutId}";

            return requestUri;
        }

        private byte[] CreatePDF(string html, Dictionary<string, object> layoutJson, string fundingPeriodCode, string fileName)
        {
            var variables = GetVariables(layoutJson);

            var widthMM = GetDoubleValue("widthMM", variables);
            var heightMM = GetDoubleValue("heightMM", variables);
            var topMarginMM = GetDoubleValue("topMarginMM", variables);
            var rightMarginMM = GetDoubleValue("rightMarginMM", variables);
            var bottomMarginMM = GetDoubleValue("bottomMarginMM", variables);
            var leftMarginMM = GetDoubleValue("leftMarginMM", variables);
            var bookmarks = GetBookmarks(layoutJson, fundingPeriodCode);

            return _pdfConverterService.CreatePdfFromHtml(html, widthMM, heightMM, topMarginMM, rightMarginMM, bottomMarginMM, leftMarginMM, bookmarks, fileName);
        }

        private double? GetDoubleValue(string name, IEnumerable<Variable> variables)
        {
            return variables.Any(variable => variable.Name == name) == true
                && double.TryParse(variables.First(variable => variable.Name == name)?.Value, out var result) ? result : (double?)null;
        }

        private IEnumerable<Variable> GetVariables(Dictionary<string, object> jsonModel)
        {
            if (!jsonModel.ContainsKey("variables"))
            {
                yield break;
            }

            foreach (var variable in (JArray)jsonModel["variables"])
            {
                var variableDict = variable.ToObject<Dictionary<string, string>>();

                yield return new Variable
                {
                    Name = variableDict["name"],
                    Value = variableDict.ContainsKey("value") ? variableDict["value"] : null
                };
            }
        }

        private IEnumerable<Bookmark> GetBookmarks(Dictionary<string, object> jsonModel, string fundingPeriodCode)
        {
            if (!jsonModel.ContainsKey("bookmarks"))
            {
                yield break;
            }

            var years = FundingPeriodHelper.GetYearsFromCode(fundingPeriodCode);

            foreach (var bookmarkObject in (JArray)jsonModel["bookmarks"])
            {
                var bookmarkDictionary = bookmarkObject.ToObject<Dictionary<string, string>>();
                var bookmark = new Bookmark
                {
                    Title = ReplaceText(bookmarkDictionary["title"], years.yearFrom, years.yearTo),
                    TextToFind = ReplaceText(bookmarkDictionary["textToFind"], years.yearFrom, years.yearTo),
                };

                if (bookmarkDictionary.ContainsKey("instanceToMatch"))
                {
                    bookmark.InstanceToMatch = int.Parse(bookmarkDictionary["instanceToMatch"]);
                }

                yield return bookmark;
            }
        }

        private string ReplaceText(string text, int year1, int year2)
        {
            return text?
                .Replace("@year1short", year1.ToString().Substring(2))
                .Replace("@year2short", year2.ToString().Substring(2))
                .Replace("@year1", year1.ToString())
                .Replace("@year2", year2.ToString());
        }

        #endregion Helper methods
    }
}