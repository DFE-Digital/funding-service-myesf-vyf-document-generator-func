using System;
using System.Collections.Generic;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Models
{
    /// <summary>
    /// Class with information about a new or updated version of funding has been added.
    /// </summary>
    public class FundingDetails
    {
        /// <summary>
        /// Gets or sets the funding version.
        /// </summary>
        public string FundingVersion { get; set; }

        /// <summary>
        /// Gets or sets the channel version.
        /// </summary>
        public IEnumerable<ChannelVersion> ChannelVersions { get; set; }

        /// <summary>
        /// Gets or sets the version from statement channel.
        /// </summary>
        public int? StatementChannelVersion { get; set; }

        /// <summary>
        /// Gets or sets the funding id.
        /// </summary>
        public string FundingId { get; set; }

        /// <summary>
        /// Gets or sets the provider funding id.
        /// </summary>
        public string ProviderFundingId { get; set; }

        /// <summary>
        /// Gets or sets the FundingStreamCode for the funding.
        /// </summary>
        public string FundingStreamCode { get; set; }

        /// <summary>
        /// Gets or sets provider type (e.g. School, Academy, Special School) - not enumerated as this isn't controlled by CFS, but passed through from the Provider info (GIAS).
        /// </summary>
        public string ProviderType { get; set; }

        /// <summary>
        /// Gets or sets the original provider type (e.g. School, Academy, Special School) - not enumerated as this isn't controlled by CFS, but passed through from the Provider info (GIAS).
        /// </summary>
        public string OriginalProviderType { get; set; }

        /// <summary>
        /// Gets or sets provider sub type (e.g. Academy special converter) - not enumerated as this isn't controlled by CFS, but passed through from the Provider info (GIAS).
        /// </summary>
        public string ProviderSubType { get; set; }

        /// <summary>
        /// Gets or sets the Ukprn for the funding.
        /// </summary>
        public string Ukprn { get; set; }

        /// <summary>
        /// Gets or sets the FundingPeriodCode for the funding.
        /// </summary>
        public string FundingPeriodCode { get; set; }

        /// <summary>
        /// Gets or sets the CutoffDate for the funding.
        /// </summary>
        public string CutoffDate { get; set; }

        /// <summary>
        /// Gets or sets the local authority name.
        /// </summary>
        public string LAName { get; set; }

        /// <summary>
        /// Gets or sets the local authority code.
        /// </summary>
        public string LACode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FundingDetails"/> is indicative.
        /// </summary>
        public bool Indicative { get; set; }

        /// <summary>
        /// Gets or sets the provider fundings.
        /// </summary>
        /// <value>
        /// The provider fundings.
        /// </value>
        public IEnumerable<string> ProviderFundings { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the funding value.
        /// </summary>
        /// <value>
        /// The funding value.
        /// </value>
        public string FundingValue { get; set; }


        /// <summary>
        /// Gets or sets the date opened.
        /// </summary>
        /// <value>
        /// The date opened.
        /// </value>
        public string DateOpenedRaw { get; set; }

        /// <summary>
        /// Gets the date opened.
        /// </summary>
        /// <value>
        /// The date opened.
        /// </value>
        public DateTime DateOpened
        {
            get
            {
                DateTime.TryParse(DateOpenedRaw, out var date);

                return date;
            }
        }

        /// <summary>
        /// Gets or sets the status changed date raw string value.
        /// </summary>
        public string StatusChangedDateRaw { get; set; }

        /// <summary>
        /// Gets the status changed date.
        /// </summary>
        public DateTime StatusChangedDate
        {
            get
            {
                DateTime.TryParse(StatusChangedDateRaw, out var date);

                return date;
            }
        }

        /// <summary>
        /// Gets or sets the partition key.
        /// </summary>
        public string PartitionKey { get; set; }

        /// <summary>
        /// Gets or sets the folder name for the document to be saved to.
        /// </summary>
        public string FolderName { get; set; }

        /// <summary>
        /// Gets or sets the schema version.
        /// </summary>
        /// <value>
        /// The schema version.
        /// </value>
        public string SchemaVersion { get; set; }
    }
}