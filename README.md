# Manage Your Education and Skills Funding Document Generator Function

The Manage Your Education and Skills Funding (MYESF) Document Generator Function is an Azure Function application responsible for generating funding-related documents for View Your Funding (VYF).

The service retrieves funding data from supporting MYESF services and APIs, processes funding information, and generates documents used by providers and internal users. The function supports scheduled and automated document generation processes, enabling funding information to be prepared and made available within the wider MYESF platform.

## Provider

[The Department for Education](https://www.gov.uk/government/organisations/department-for-education)

## About this project

This project is a .NET Azure Functions application running on Azure Function App infrastructure.

The application integrates with multiple Azure services including:

- Azure Functions
- Azure Service Bus
- Azure Cosmos DB
- Azure Storage
- Azure Blob Storage
- Application Insights
 
The service is responsible for processing document generation requests and generating funding statements, allocation documents and reports used within the MYESF ecosystem.

# Local Configuration Guide

In order to run the application locally, a valid `local.settings.json` file will need to be created in the Function App project.

Included in the repository is an `local.settings.example.json` file which can be used as a base and populated with the required values, which can be obtained from Azure resources and application registrations.

## Local Settings (`local.settings.json`)

```json
{
  "IsEncrypted": false,
  "Values": {
    "Auditing_CosmosDbConfiguration_CollectionName": "",
    "Auditing_CosmosDbConfiguration_ConnectionString": "",
    "Auditing_CosmosDbConfiguration_DatabaseName": "",
    "AzureWebJobsStorage": "",
    "AzureWebJobsDashboard": "UseDevelopmentStorage=true",
    "BaseSiteUrl": "",
    "BlobStorage_ConnectionString": "",
    "BlobStorage_ContainerName": "",
    "DestinationStorage": "",
    "DocumentGeneratorScheduleTriggerTime": "* * * * *",
    "Environment": "local",
    "FileRepoStorage_Compare_ConnectionString": null,
    "FileRepoStorage_ConnectionString": "",
    "FileRepoStorageName_Business": "",
    "FileRepoStorageName_Internal": "",
    "Filtered_FundingGroupReasonsTypeCodes": "",
    "FilteredFundingStreams_Fundings": "",
    "FilteredFundingStreams_ProviderFundings": "",
    "FilteredVariations_Funding": "",
    "FilteredVariations_ProviderFundings": "",
    "FUNCTIONS_EXTENSION_VERSION": "~4",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "FundingApiSecretKey": "",
    "IndicativeGroupingReason": "",
    "IndicativeProviderStatusList": "",
    "Logging:ApplicationInsights:Loglevel:Default": "Information",
    "Logging:ApplicationInsights:Loglevel:Microsoft": "Error",
    "Logging:LogLevel:Default": "Information",
    "NonRelationalDb_CosmosDbConfiguration_ConnectionString": "",
    "NonRelationalDb_CosmosDbConfiguration_DatabaseName": "",
    "NonRelationalDb_CosmosDbConfiguration_FundingCollection": "",
    "NonRelationalDb_CosmosDbConfiguration_LayoutCollection": "",
    "NonRelationalDb_CosmosDbConfiguration_ProviderFundingCollection": "",
    "Parallel_Run_Batch_Size": "10",
    "PdsApplicationInsights:InstrumentationKey": "",
    "Processing_Batch_Size": "4000",
    "Processing_Run_Size_Comparison": "10",
    "QueueName": "",
    "ServiceBusConnectionString": ""
  }
}
```

### Setting Details

- **`Auditing_CosmosDbConfiguration_CollectionName`**  
  The Cosmos DB collection used for audit records.

- **`Auditing_CosmosDbConfiguration_DatabaseName`**  
  The Cosmos DB database containing the audit collection.

- **`Auditing_CosmosDbConfiguration_ConnectionString`**  
  The Cosmos DB connection string used to access audit data.

- **`AzureWebJobsStorage`** 
  The Azure Storage connection string required by the Azure Functions runtime for operation and trigger management.

- **`AzureWebJobsDashboard`** 
  The Azure Storage jobs dashboard configuration setting to resolve issues with local running.

- **`BaseSiteUrl`**  
  The base URL used when generating links and references within generated documents.

- **`BlobStorage_ConnectionString`**  
  The Azure Blob Storage connection string used for storing generated files.

- **`BlobStorage_ContainerName`**  
  The blob container where generates documents are stored.

- **`DestinationStorage`**  
  Determines the storage mechanism used by generates document processing.

- **`DocumentGeneratorScheduleTriggerTime`**  
  The CRON expression defining the schedule used by the timer-triggered document generation process.

- **`Environment`**
  The environment in which the application is running.

- **`FileRepoStorage_ConnectionString`**  
  Storage account connection string used to store generated document files

- **`FileRepoStorage_Compare_ConnectionString`**  
  Storage account connection string used for document comparison processing.

- **`FileRepoStorageName_Business`**  
  Storage container used for business-facing generated documents.

- **`FileRepoStorageName_Internal`**  
  Storage container used for internal and backup document storage.

- **`Filtered_FundingGroupReasonsTypeCodes`**  
  Funding group reason codes used during document processing.

- **`FilteredFundingStreams_Fundings`**  
  Funding streams processed by funding document generation processes.

- **`FilteredFundingStreams_ProviderFundings`**  
  Provider funding streams processed by provider funding document generation processes.

- **`FilteredVariations_Fundings`**  
  Valid variation reasons to be processed by funding document generation processes.

- **`FilteredVariations_ProviderFundings`**  
  Valid variation reasons to be processed by provider funding document generation processes.

- **`FUNCTIONS_EXTENSION_VERSION`**  
  The Azure Functions runtime version used by the application.

- **`FUNCTIONS_WORKER_RUNTIME`**  
  The worker runtime used by the Function App. This application uses the .NET Isolated worker model.

- **`FundingApiSecretKey`**  
  Secret key used when communicating with supporting funding APIs.

- **`IndicativeGroupingReason`**  
  The grouping reason used to identify indicative funding allocations.

- **`IndicativeProviderStatusList`**  
  Provider statuses that should be treated as indicative during processing.

- **`Logging:ApplicationInsights:LogLevel:Default`**  
  The default logging level when writing telemetry to Application Insights.

- **`Logging:ApplicationInsights:LogLevel:Microsoft`**  
  The logging level used for Microsoft framework components when writing telemetry to Application Insights.

- **`Logging:LogLevel:Default`**  
  The default logging level for the application.
  
- **`NonRelationalDb_CosmosDbConfiguration_DatabaseName`**  
  The database used to store funding-related data.

- **`NonRelationalDb_CosmosDbConfiguration_FundingCollection`**  
  The collection containing funding information.

- **`NonRelationalDb_CosmosDbConfiguration_LayoutCollection`**  
  The collection containing layout mappings and metadata.

- **`NonRelationalDb_CosmosDbConfiguration_ProviderFundingCollection`**  
  The collection containing provider funding records.

- **`NonRelationalDb_CosmosDbConfiguration_ConnectionString`**  
  The Cosmos DB connection string used to access funding data.

- **`PdsApplicationInsights:InstrumentationKey`**  
  The instrumentation key used by the application logging framework for sending telemetry and diagnostics to Application Insights.

- **`Parallel_Run_Batch_Size`**  
  The maximum number of items processed concurrently.

- **`Processing_Batch_Size`**  
  The number of funding records processed in a batch.

- **`Processing_Run_Size_Comparison`**  
  The number of comparisons processed during comparison operations.

- **`ServiceBusConnectionString`**  
  The Azure Service Bus connection string used for document generation messaging.

- **`QueueName`**  
  The queue monitoned for document generation request.

### Layout and Document Configuration

The application contains a large number of configuration entries used to determine:

- Funding stream layouts
- Academic year mappings
- File naming conventions
- Output file types
- Layout identifiers

Examples include:

- `LayoutID_`
- `FileName_LayoutID_`
- `FileType_LayoutID_`

These settings determine which layout should be used, how generated documents should be named, and which output format should be produced for a given funding stream and academic year combination.

## Text Execution

In order to test the application locally a valid `appsettings.json` file will need to be created in the `PDS.ViewYourFunding.DocumentGenerator.Repositories.Tests` project. `appsettings.example.json`, in `PDS.ViewYourFunding.DocumentGenerator.Repositories.Tests` can be used as a base and populated with appropriate values which can be found in Azure Portal.

## Test Application Settings (`appsettings.json`)

```json
{
  "CosmosDB_ContainerName": "pdfGeneratorIntegrationTests",
  "CosmosDB_ConnectionMode": "Gateway",
  "CosmosDB_ConnectionString": "",
  "CosmosDB_DbName": "funding"
}
```

### Setting Details

- **`CosmosDB_ContainerName`**  
  The cosmos db container used for document generator integration tests.

- **`CosmosDB_ConnectionMode`**  
  The cosmos db connection mode required for local running of tests.

- **`CosmosDB_ConnectionString`**  
  The connection string for the document generator cosmos db resource. (Use `pds-dev-shared-cdb`)

- **`CosmosDB_DbName`**  
  The cosmos db database used for document generator integration tests.

## Build and Test

To build and test locally, you can either use Visual Studio, Visual Studio Code or simply use dotnet CLI `dotnet build` and `dotnet test` more information in dotnet CLI can be found at <https://docs.microsoft.com/en-us/dotnet/core/tools/>.

## Contribute

To contribute,

- If you are part of the team then create a branch for changes and then submit your changes for review by creating a pull request.
- If you are external to the organisation then fork this repository and make necessary changes and then submit your changes for review by creating a pull request.