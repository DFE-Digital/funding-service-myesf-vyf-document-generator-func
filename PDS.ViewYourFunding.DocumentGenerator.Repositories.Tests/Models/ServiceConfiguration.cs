namespace PDS.ViewYourFunding.DocumentGenerator.Repositories.Tests
{
    /// <summary>
    /// A class used to represent configuration settings.
    /// </summary>
    public class ServiceConfiguration
    {
        /// <summary>
        /// Gets or sets the Cosmos Db connection string.
        /// </summary>
        public string CosmosDB_ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the Cosmos Db database name.
        /// </summary>
        public string CosmosDB_DbName { get; set; } = "funding";

        /// <summary>
        /// Gets or sets the Cosmos Db container name.
        /// </summary>
        public string CosmosDB_ContainerName { get; set; } = "pdfGeneratorIntegrationTests";


        /// <summary>
        /// Gets or sets the Cosmos Db connection mode. Default is "Gateway".
        /// </summary>
        public string CosmosDB_ConnectionMode { get; set; } = "Gateway";
    }
}