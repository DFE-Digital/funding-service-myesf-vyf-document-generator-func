using System.Threading;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Interfaces
{
    /// <summary>
    /// A service for dealing with Restart Function App.
    /// </summary>
    public interface ITimerTriggerControlService
    {
        /// <summary>
        /// Gets or sets a value indicating whether the Document Generator Rerun is in Progress.
        /// </summary>
        public bool IsRerunHttpTriggerFunctionInProgress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Document Generator Timer Tigger Function is in Progress.
        /// </summary>
        public bool IsTimerTriggerFunctionInProgress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Document Generator Timer Tigger Function is in Progress.
        /// </summary>
        public bool IsFundingReportHttpTriggerFunctionInProgress { get; set; }

        /// <summary>
        /// Wait Till Timer Or FundingReport To Finish.
        /// </summary>
        /// <param name="token">Cancellation Token.</param>
        /// <returns>Task to be awaited.</returns>
        Task WaitTillTimerOrFundingReportToFinish(CancellationToken token = default);

        /// <summary>
        /// Wait Till Timer Or Rerun To Finish.
        /// </summary>
        /// <param name="token">Cancellation Token.</param>
        /// <returns>Task to be awaited.</returns>
        Task WaitTillTimerOrRerunToFinish(CancellationToken token = default);
    }
}
