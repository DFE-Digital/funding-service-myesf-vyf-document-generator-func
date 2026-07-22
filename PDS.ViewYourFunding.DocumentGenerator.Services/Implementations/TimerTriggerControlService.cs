using Pds.Core.Logging;
using PDS.ViewYourFunding.DocumentGenerator.Services.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Implementations
{
    /// <inheritdoc/>
    public class TimerTriggerControlService : ITimerTriggerControlService
    {
        private readonly ILoggerAdapter<TimerTriggerControlService> _logger;

        /// <inheritdoc/>
        public bool IsRerunHttpTriggerFunctionInProgress { get; set; } = false;

        /// <inheritdoc/>
        public bool IsTimerTriggerFunctionInProgress { get; set; } = false;

        /// <inheritdoc/>
        public bool IsFundingReportHttpTriggerFunctionInProgress { get; set; } = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimerTriggerControlService"/> class.
        /// </summary>
        /// <param name="logger">The logger adapter object.</param>
        public TimerTriggerControlService(ILoggerAdapter<TimerTriggerControlService> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task WaitTillTimerOrFundingReportToFinish(CancellationToken token = default)
        {
            while (IsTimerTriggerFunctionInProgress || IsFundingReportHttpTriggerFunctionInProgress)
            {
                var currentRunningProcess = IsTimerTriggerFunctionInProgress ? "Timer Trigger" : "Funding Report Http Trigger";
                _logger.LogInformation($"[{currentRunningProcess}] is currently running and [Rerun job] will wait till it complete.");
                await Task.Delay(5000, token);
            }
        }

        /// <inheritdoc/>
        public async Task WaitTillTimerOrRerunToFinish(CancellationToken token = default)
        {
            while (IsTimerTriggerFunctionInProgress || IsRerunHttpTriggerFunctionInProgress)
            {
                var currentRunningProcess = IsTimerTriggerFunctionInProgress ? "Timer Trigger" : "Rerun Http Trigger";
                _logger.LogInformation($"[{currentRunningProcess}] is currently running and [Generate Funding Reports job] will wait till it complete.");
                await Task.Delay(5000, token);
            }
        }
    }
}
