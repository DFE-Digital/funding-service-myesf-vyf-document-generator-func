using System;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Interfaces
{
    /// <summary>
    /// The Date time service.
    /// </summary>
    public interface IDateTimeService
    {
        /// <summary>
        /// Gets the date time folder name.
        /// </summary>
        /// <returns>The folder name where statement lands.</returns>
        string GetDateTimePathComponent();

        /// <summary>
        /// Gets the date time folder name.
        /// </summary>
        /// <param name="customDateTime">Custom date time to be converted.</param>
        /// <returns>The folder name where statement lands.</returns>
        string GetDateTimePathComponent(DateTime customDateTime);
    }
}
