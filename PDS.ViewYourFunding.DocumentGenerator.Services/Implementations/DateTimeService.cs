using Pds.Core.Utils;
using PDS.ViewYourFunding.DocumentGenerator.Services.Interfaces;
using System;


namespace PDS.ViewYourFunding.DocumentGenerator.Services.Implementations
{
    /// <summary>
    /// Utility class for date time.
    /// </summary>
    public class DateTimeService : IDateTimeService
    {
        private readonly ISystemProvider _systemProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeService"/> class.
        /// The parameterised constructor.
        /// </summary>
        /// <param name="systemProvider">The System Provider.</param>
        public DateTimeService(ISystemProvider systemProvider)
        {
            _systemProvider = systemProvider;
        }

        /// <inheritdoc />
        public string GetDateTimePathComponent()
        {
           return _systemProvider.DateTime.ConvertToUKTime(_systemProvider.DateTime.Now()).ToString("yyyy-MM-dd HH\\hmm\\m");
        }

        /// <inheritdoc />
        public string GetDateTimePathComponent(DateTime customDateTime)
        {
            return _systemProvider.DateTime.ConvertToUKTime(customDateTime).ToString("yyyy-MM-dd HH\\hmm\\m");
        }
    }
}
