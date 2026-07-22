using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pds.Core.Logging;
using Pds.Core.Telemetry.ApplicationInsights;
using Pds.Core.Utils;
using PDS.ViewYourFunding.DocumentGenerator.FunctionApp;
using PDS.ViewYourFunding.DocumentGenerator.Repositories;
using PDS.ViewYourFunding.DocumentGenerator.Services;
using PDS.ViewYourFunding.DocumentGenerator.Services.Implementations;
using PDS.ViewYourFunding.DocumentGenerator.Services.Interfaces;
using PDS.ViewYourFunding.DocumentGenerator.Services.Strategies;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Configuration.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);

builder.Configuration.AddEnvironmentVariables();

var configuration = ConfigHelper.GetServiceConfiguration();

builder.Services.AddLogging()
    .AddLoggerAdapter()
    .AddPdsUtils()
    .AddSingleton<ISettingService, EnvironmentVariableSettingService>()
    .AddSingleton<ILogicService, LogicService>()
    .AddSingleton<IDateTimeService, DateTimeService>()
    .AddSingleton<IPDFConverterService, AsposeHtmlPDFConverterService>()
    .AddSingleton<ILayoutService, NonRelationalDbLayoutService>()
    .AddSingleton<IFileNameBuilder, GagProviderFileNameBuilderService>()
    .AddSingleton<IPopulateFundingMetaData, GagProviderFundingMetaDataService>()
    .AddSingleton<ITimerTriggerControlService, TimerTriggerControlService>()
    .AddSingleton<INonRelationalDb>(s =>
    {
        var connectionMode = configuration.NonRelationalDb_CosmosDbConfiguration_ConnectionMode.Equals("Gateway", System.StringComparison.InvariantCultureIgnoreCase) ? ConnectionMode.Gateway : ConnectionMode.Direct;

        return new CosmosDb(
            configuration.NonRelationalDb_CosmosDbConfiguration_ConnectionString,
            configuration.NonRelationalDb_CosmosDbConfiguration_DatabaseName,
            configuration.NonRelationalDb_CosmosDbConfiguration_LayoutCollection,
            s.GetRequiredService<ILoggerAdapter<CosmosDb>>(),
            connectionMode);
    })
    .AddSingleton<IAuditLogService>(s =>
    {
        var connectionMode = configuration.Auditing_CosmosDbConfiguration_ConnectionMode.Equals("Gateway", System.StringComparison.InvariantCultureIgnoreCase) ? ConnectionMode.Gateway : ConnectionMode.Direct;

        var cosmosDb = new CosmosDb(
            configuration.Auditing_CosmosDbConfiguration_ConnectionString,
            configuration.Auditing_CosmosDbConfiguration_DatabaseName,
            configuration.Auditing_CosmosDbConfiguration_CollectionName,
            s.GetRequiredService<ILoggerAdapter<CosmosDb>>(),
            connectionMode);

        return new NonRelationalDbAuditLogService(cosmosDb);
    })
    .AddSingleton<IFundingService>(s =>
    {
        var connectionMode = configuration.NonRelationalDb_CosmosDbConfiguration_ConnectionMode.Equals("Gateway", System.StringComparison.InvariantCultureIgnoreCase) ? ConnectionMode.Gateway : ConnectionMode.Direct;

        var cosmosDb = new CosmosDb(
            configuration.NonRelationalDb_CosmosDbConfiguration_ConnectionString,
            configuration.NonRelationalDb_CosmosDbConfiguration_DatabaseName,
            configuration.NonRelationalDb_CosmosDbConfiguration_FundingCollection,
            s.GetRequiredService<ILoggerAdapter<CosmosDb>>(),
            connectionMode);

        return new NonRelationalDbFundingService(s.GetRequiredService<ISettingService>(), s.GetRequiredService<IAuditLogService>(), cosmosDb);
    })
    .AddSingleton<IFileSharePdfComparerService>(s =>
    {
        return new FileSharePdfComparerService(
            s.GetRequiredService<IPDFConverterService>(),
            configuration.FileRepoStorage_ConnectionString,
            configuration.FileRepoStorage_Compare_ConnectionString,
            configuration.FileRepoStorageName_Business,
            s.GetRequiredService<ILoggerAdapter<FileSharePdfComparerService>>());
    })
    .AddSingleton<IProviderFundingService>(s =>
    {
        var connectionMode = configuration.NonRelationalDb_CosmosDbConfiguration_ConnectionMode.Equals("Gateway", System.StringComparison.InvariantCultureIgnoreCase) ? ConnectionMode.Gateway : ConnectionMode.Direct;

        var cosmosDb = new CosmosDb(
            configuration.NonRelationalDb_CosmosDbConfiguration_ConnectionString,
            configuration.NonRelationalDb_CosmosDbConfiguration_DatabaseName,
            configuration.NonRelationalDb_CosmosDbConfiguration_ProviderFundingCollection,
            s.GetRequiredService<ILoggerAdapter<CosmosDb>>(),
            connectionMode);

        return new NonRelationalDbProviderFundingService(s.GetRequiredService<ISettingService>(), s.GetRequiredService<IAuditLogService>(), cosmosDb);
    })
    .AddSingleton<IHttpService>(s =>
    {
        return new HttpService(configuration.FundingApiSecretKey, configuration.BaseSiteUrl);
    })
    .AddSingleton<ISaveService>(s =>
    {
        var connectionString = configuration.FileRepoStorage_ConnectionString;
        var internalFileRepoStorageName = configuration.FileRepoStorageName_Internal;
        var businessFileRepoStorageName = configuration.FileRepoStorageName_Business;

        return new FileShareSaveService(connectionString, internalFileRepoStorageName, businessFileRepoStorageName);
    })
    .AddPdsApplicationInsightsTelemetry(options =>
    {
        options.InstrumentationKey = System.Environment.GetEnvironmentVariable("PdsApplicationInsights:InstrumentationKey");
        options.Environment = System.Environment.GetEnvironmentVariable("Environment");
        options.Component = typeof(Program).Assembly.GetName().Name;
    });

builder.Build().Run();