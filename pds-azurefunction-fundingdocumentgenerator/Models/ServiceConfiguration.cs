namespace PDS.ViewYourFunding.DocumentGenerator.FunctionApp
{
    /// <summary>
    /// A class used to represent configuration settings.
    /// </summary>
    public class ServiceConfiguration
    {
        /// <summary>
        /// Gets or sets the cosmos connection string for the non relational db.
        /// </summary>
        public string NonRelationalDb_CosmosDbConfiguration_ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the cosmos db name for the non relational db.
        /// </summary>
        public string NonRelationalDb_CosmosDbConfiguration_DatabaseName { get; set; } = "funding";

        /// <summary>
        /// Gets or sets the cosmos layout collection name for the non relational db.
        /// </summary>
        public string NonRelationalDb_CosmosDbConfiguration_LayoutCollection { get; set; } = "layout";

        /// <summary>
        /// Gets or sets the cosmos funding collection name for the non relational db.
        /// </summary>
        public string NonRelationalDb_CosmosDbConfiguration_FundingCollection { get; set; } = "funding";

        /// <summary>
        /// Gets or sets the cosmos provider funding collection name for the non relational db.
        /// </summary>
        public string NonRelationalDb_CosmosDbConfiguration_ProviderFundingCollection { get; set; } = "providerfunding";

        /// <summary>
        /// Gets or sets the Cosmos Db ConnectionMode for non relational db.
        /// </summary>
        public string NonRelationalDb_CosmosDbConfiguration_ConnectionMode { get; set; } = "Direct";

        /// <summary>
        /// Gets or sets the Funding Api secret key.
        /// </summary>
        public string FundingApiSecretKey { get; set; }

        /// <summary>
        /// Gets or sets the base site url.
        /// </summary>
        public string BaseSiteUrl { get; set; }

        /// <summary>
        /// Gets or sets the Cosmos Db connection string used for auditing.
        /// </summary>
        public string Auditing_CosmosDbConfiguration_ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the Cosmos Db database name used for auditing.
        /// </summary>
        public string Auditing_CosmosDbConfiguration_DatabaseName { get; set; } = "funding";

        /// <summary>
        /// Gets or sets the Cosmos Db container name used for auditing.
        /// </summary>
        public string Auditing_CosmosDbConfiguration_CollectionName { get; set; } = "audit";

        /// <summary>
        /// Gets or sets the Cosmos Db ConnectionMode for Auditing.
        /// </summary>
        public string Auditing_CosmosDbConfiguration_ConnectionMode { get; set; } = "Direct";

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

        /// <summary>
        /// Gets or sets the file share storage connection string for the location where pdf comparison will happen.
        /// </summary>
        public string FileRepoStorage_Compare_ConnectionString { get; set; }
    }
}