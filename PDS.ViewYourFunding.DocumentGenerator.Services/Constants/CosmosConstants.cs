namespace PDS.ViewYourFunding.DocumentGenerator.Services.Constants
{
    /// <summary>
    /// The Cosmos constants class.
    /// </summary>
    public static class CosmosConstants
    {
        /// <summary>
        /// The provider funding select statement.
        /// </summary>
        public const string ProviderFundingSelectStatement = "SELECT Distinct " +
                                                             "c.id as ProviderFundingId, " +
                                                             "(is_defined(c.channelVersion) ? c.channelVersion : []) as ChannelVersions, " +
                                                             "(select value max(SV['value']) from SV in c.channelVersion where SV.type = 'Statement') as StatementChannelVersion, " +
                                                             "c.fundingStreamCode as FundingStreamCode, " +
                                                             "c.provider.providerType as ProviderType, " +
                                                             "c.provider.providerType as OriginalProviderType, " +
                                                             "c.provider.providerSubType as ProviderSubType, " +
                                                             "c.fundingPeriodId as FundingPeriodCode, " +
                                                             "c.statusChangedDate as CutoffDate, " +
                                                             "tostring(c.fundingValue) as FundingValue, " +
                                                             "c.provider.providerDetails.dateOpened as DateOpenedRaw, " +
                                                             "c.schemaVersion as SchemaVersion, " +
                                                             "c.provider.identifier as Ukprn, " +
                                                             "(is_defined(c.rerunDate) ? c.rerunDate : (select value max(PI['statusChangedDate']) from PI in c.parentInformation)) as StatusChangedDateRaw, " +
                                                             "c.partitionKey as PartitionKey " +
                                                             "FROM c ";
    }
}