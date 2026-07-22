using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Pds.Core.Logging;
using PDS.ViewYourFunding.DocumentGenerator.Services;
using PDS.ViewYourFunding.DocumentGenerator.Services.Interfaces;
using PDS.ViewYourFunding.DocumentGenerator.Services.Messages;

namespace PDS.ViewYourFunding.DocumentGenerator.FunctionApp
{
    /// <summary>
    /// The function entry point files.
    /// </summary>
    public class EntryPoints
    {
        private readonly ILogicService _logicService;
        private readonly IAuditLogService _auditLoggerService;
        private readonly ITimerTriggerControlService _timerTriggerControlService;
        private readonly ILoggerAdapter<EntryPoints> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntryPoints"/> class.
        /// </summary>
        /// <param name="logicService">The main logic.</param>
        /// <param name="auditLoggerService">The auditlogger.</param>
        /// <param name="timerTriggerControlService">The timerTriggerControlService.</param>
        /// <param name="logger">The logger adapter object.</param>
        public EntryPoints(ILogicService logicService, IAuditLogService auditLoggerService, ITimerTriggerControlService timerTriggerControlService, ILoggerAdapter<EntryPoints> logger)
        {
            _logicService = logicService;
            _auditLoggerService = auditLoggerService;
            _timerTriggerControlService = timerTriggerControlService;
            _logger = logger;
        }

        /// <summary>
        /// Entry point via service bus (main use case).
        /// </summary>
        /// <param name="myTimer">The timer info.</param>
        /// <param name="token">CancellationToken which provided by Host if there are any issues/Timeout due to which host tries to cancel the Function Execution.</param>
        /// <returns>An awaitable task.</returns>
        [Function("DocumentGeneratorTimerFunction")]
        public async Task Run_Timer_DocumentGenerator([TimerTrigger("%DocumentGeneratorScheduleTriggerTime%")] TimerInfo myTimer, CancellationToken token)
        {
            if (_timerTriggerControlService.IsRerunHttpTriggerFunctionInProgress)
            {
                _logger.LogInformation($"Currently DocumentGeneratorFunctionHttpRerun is executing. Current Instance of DocumentGeneratorTimerFunction Function is exiting!");
            }
            else if (_timerTriggerControlService.IsFundingReportHttpTriggerFunctionInProgress)
            {
                _logger.LogInformation($"Currently DocumentGeneratorFunctionHttpFundingReports is executing. Current Instance of DocumentGeneratorTimerFunction Function is exiting!");
            }
            else
            {
                _logger.LogInformation($"DocumentGeneratorTimerFunction - started");
                _timerTriggerControlService.IsTimerTriggerFunctionInProgress = true;

                try
                {
                    token.ThrowIfCancellationRequested();
                    await _logicService.RunDocumentGeneratorTimer(token);
                }
                catch (OperationCanceledException exception)
                {
                    _logger?.LogInformation(exception, $"Host requested Cancellation and we gracefully canceled all tasks successfully!");
                }
                catch (Exception ex)
                {
                    await _auditLoggerService.Log($"Error running DocumentGeneratorTimerFunction: {ex.Message} - see function app logging for more detail");
                    throw;
                }
                finally
                {
                    _timerTriggerControlService.IsTimerTriggerFunctionInProgress = false;
                    _logger.LogInformation($"DocumentGeneratorTimerFunction - finished");
                }
            }
        }

        /// <summary>
        /// Entry point via HTTP.
        /// </summary>
        /// <param name="request">The HttpRequest.</param>
        /// <returns>An OK or bad result.</returns>
        [Function("DocumentGeneratorFunctionHttp")]
        public async Task<IActionResult> Run_Http_GenerateSingleDocument([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest request)
        {
            _logger.LogInformation($"DocumentGeneratorFunctionHttp - started");
            var query = request.Query;
            Exception exception = null;

            try
            {
                var path = await _logicService.RunGenerateSingleDocument(query["fundingId"], query["providerFundingId"], query["fundingStreamCode"], query["ukprn"], query["fundingPeriodCode"], query["providerType"], query["providerSubType"], query["laName"], query["laCode"], query["cutoffDate"]);

                return new OkObjectResult(path);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error running DocumentGeneratorFunctionHttp: {ex.Message} - see function app logging for more detail");
                return new BadRequestObjectResult($"ERROR: {ex.Message}");
            }
            finally
            {
                await LogAuditInfo(exception, "http", query["fundingStreamCode"], query["ukprn"], query["fundingPeriodCode"], query["cutoffDate"], query["providerType"], query["providerSubType"]);
                _logger.LogInformation($"DocumentGeneratorFunctionHttp - finished");
            }
        }

        /// <summary>
        /// Entry point via HTTP.
        /// </summary>
        /// <param name="request">The HttpRequest.</param>
        /// <returns>An OK or bad result.</returns>
        [Function("DocumentGeneratorFunctionHttpFundingReports")]
        public async Task<IActionResult> Run_Http_GenerateFundingReports([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest request)
        {
                await _timerTriggerControlService.WaitTillTimerOrRerunToFinish();

                _logger.LogInformation($"DocumentGeneratorFunctionHttpFundingReports - started");

                _timerTriggerControlService.IsFundingReportHttpTriggerFunctionInProgress = true;

                var query = request.Query;
                try
                {
                    await _logicService.RunGenerateFundingReports(query["groupTypeCode"], query["excludedGroupTypeCode"], query["groupTypeReason"], query["fundingPeriodId"]);

                    return new OkObjectResult("OK");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error running DocumentGeneratorFunctionHttpFundingReports: {ex.Message} - see function app logging for more detail");
                    return new BadRequestObjectResult($"ERROR: {ex.Message}");
                }
                finally
                {
                    _timerTriggerControlService.IsFundingReportHttpTriggerFunctionInProgress = false;
                    _logger.LogInformation($"DocumentGeneratorFunctionHttpFundingReports - finished");
                }
        }

        /// <summary>
        /// Entry point via HTTP to reset document generated attribute.
        /// </summary>
        /// <param name="request">The HttpRequest.
        /// example query string:  ?fundingStreamCode=NMSS&SinceCreatedDate=2021-02-22T13:33:13.6021893Z&EndDateTime=2021-02-25T13:33:13.6021893Z&ResetProviderFunding=false.</param>
        /// <returns>An OK or bad result.</returns>
        [Function("DocumentGeneratorFunctionHttpRerun")]
        public async Task<IActionResult> Run_Http_RerunDocumentGeneration([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest request)
        {
            await _timerTriggerControlService.WaitTillTimerOrFundingReportToFinish();

            _logger.LogInformation("DocumentGeneratorFunctionHttpRerun - started");

            _timerTriggerControlService.IsRerunHttpTriggerFunctionInProgress = true;

            try
            {
                var resetAttributeRequest = new ResetAttributeRequest();

                foreach (var (key, value) in request.Query)
                {
                    var propertyInfo = resetAttributeRequest.GetType()
                        .GetProperties().FirstOrDefault(property => property.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase));
                    if (propertyInfo != null)
                    {
                        propertyInfo.SetValue(resetAttributeRequest, Convert.ChangeType(value.ToString(), propertyInfo.PropertyType), null);
                    }
                }

                await _logicService.RunRerunDocumentGeneration(resetAttributeRequest);

                return new OkObjectResult("OK");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error running DocumentGeneratorFunctionHttpRerun: {ex.Message} - see function app logging for more detail");
                return new BadRequestObjectResult($"ERROR: {ex.Message}");
            }
            finally
            {
                _timerTriggerControlService.IsRerunHttpTriggerFunctionInProgress = false;
                _logger.LogInformation($"DocumentGeneratorFunctionHttpRerun - finished");
            }
        }

        /// <summary>
        /// Entry point via HTTP to compare pdfs.
        /// </summary>
        /// <param name="request">The HttpRequest. Send created Date in format [2021-01-21T21:56:07.7909217Z].</param>
        /// <returns>An OK or bad result.</returns>
        [Function("DocumentGeneratorFunctionHttpPdfComparer")]
        public async Task<IActionResult> Run_Http_PdfComparison([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest request)
        {
            _logger.LogInformation($"DocumentGeneratorFunctionHttpPdfComparer - started");
            var query = request.Query;

            try
            {
                await _logicService.RunPdfComparison(query["fundingStreamCode"], query["fundingPeriodCode"], query["folderSource"], query["folderDestination"]);

                return new OkObjectResult("OK");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error running DocumentGeneratorFunctionHttpPdfComparer: {ex.Message} - see function app logging for more detail");
                return new BadRequestObjectResult($"ERROR: {ex.Message}");
            }
            finally
            {
                _logger.LogInformation($"DocumentGeneratorFunctionHttpPdfComparer - finished");
            }
        }

        private async Task LogAuditInfo(Exception error, string method, string fundingStreamCode, string ukprn, string fundingPeriodCode, string cutoffDate, string providerType, string providerSubType)
        {
            var coreMessage = $"Document generation ({fundingStreamCode}, {ukprn}, {fundingPeriodCode}, {cutoffDate}, {providerType}, {providerSubType}) via {method} finished";

            if (error != null)
            {
                await _auditLoggerService.Log($"Error: {coreMessage} - {error.Message} - see function app logging for more detail");
                return;
            }

            await _auditLoggerService.Log($"OK - {coreMessage}");
        }
    }
}