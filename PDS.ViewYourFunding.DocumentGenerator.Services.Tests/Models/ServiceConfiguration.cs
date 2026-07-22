namespace PDS.ViewYourFunding.DocumentGenerator.Services.Tests.Models
{
    /// <summary>
    /// A class used to represent configuration settings.
    /// </summary>
    public class ServiceConfiguration
    {
        /// <summary>
        /// Gets or sets the file share storage connection string.
        /// </summary>
        public string FileRepoStorage_ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the internal file share storage name.
        /// </summary>
        public string FileRepoStorageName_Internal { get; set; }

        /// <summary>
        /// Gets or sets the business file share storage name.
        /// </summary>
        public string FileRepoStorageName_Business { get; set; }
    }
}