using System.Threading.Tasks;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Interfaces
{
    /// <summary>
    /// An interface for auditing.
    /// </summary>
    public interface IAuditLogService
    {
        /// <summary>
        /// Log a message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <returns>An awaitable task.</returns>
        Task Log(string message);

        /// <summary>
        /// Checks whether there is an active feed reader instance running.
        /// </summary>
        /// <returns>True if no feed reader instance is running. False if feed reader instance is running.</returns>
        Task<bool> CheckNoRunningInstanceOfFeedReader();

        /// <summary>
        /// Get the last successful run that was audited.
        /// </summary>
        /// <returns>The run datetime as string.</returns>
        Task<string> GetLastSuccessfulRunTime();
    }
}