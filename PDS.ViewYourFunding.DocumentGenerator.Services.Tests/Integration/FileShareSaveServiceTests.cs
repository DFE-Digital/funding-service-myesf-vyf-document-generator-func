using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDS.ViewYourFunding.DocumentGenerator.Services.Models;
using PDS.ViewYourFunding.DocumentGenerator.Services.Tests.Config;
using System;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Tests
{
    /// <summary>
    /// Tests for the file share save service.
    /// </summary>
    [TestClass]
    [Ignore] // Ignored for now while we set up pipelines and config
    public class FileShareSaveServiceTests
    {
        private readonly FileShareSaveService fileShareSaveService;

        public FileShareSaveServiceTests()
        {
            var configuration = ConfigHelper.GetServiceConfiguration();
            fileShareSaveService = new FileShareSaveService(configuration.FileRepoStorage_ConnectionString, configuration.FileRepoStorageName_Internal, configuration.FileRepoStorageName_Business);
        }

        [TestMethod, TestCategory("Integration"), TestCategory("CoreIntegration")]
        [DataRow("GAG", "AY-2122", "GAG-FY-2021-12345678-1_0", "", true, Constants.FolderNames.IndicativeStatements)]
        [DataRow("GAG", "AY-2122", "GAG-FY-2021-12345678-2_0", "", false, Constants.FolderNames.UpdatedStatements)]
        [DataRow("GAG", "AY-2122", "GAG-FY-2021-12345678-1_0", "", false, Constants.FolderNames.NewStatements)]
        public async Task Save_FullyMocked_Id_RunsWithoutFault(
            string fundingSteamCode,
            string fundingPeriodCode,
            string providerFundingId,
            string fundingId,
            bool indicative,
            string folderName)
        {
            // Arrange
            var datePartToday = DateTime.Now.ToString("yyyy-MM-dd");
            var expected = $"{fundingSteamCode}/{fundingPeriodCode}/{datePartToday}/{folderName}/12345678";

            var fundingDetails = new FundingDetails
            {
                FundingStreamCode = fundingSteamCode,
                FundingPeriodCode = fundingPeriodCode,
                ProviderFundingId = providerFundingId,
                FundingId = fundingId,
                Indicative = indicative,
                FolderName = datePartToday
            };

            await fileShareSaveService.CreateFileDirectories(fundingSteamCode, fundingPeriodCode, datePartToday);

            // Act
            var actual = await fileShareSaveService.Save(fundingDetails, "12345678", new byte[] { });

            // Assert
            actual.Should().Be(expected);
        }

        [TestMethod, TestCategory("Integration"), TestCategory("CoreIntegration")]
        [DataRow("GAG", "AY-2122", "GAG-FY-2021-12345678-1_0", "", true, 1, Constants.FolderNames.IndicativeStatements)]
        [DataRow("GAG", "AY-2122", "GAG-FY-2021-12345678-2_0", "", false, 2, Constants.FolderNames.UpdatedStatements)]
        [DataRow("GAG", "AY-2122", "GAG-FY-2021-12345678-1_0", "", false, 1, Constants.FolderNames.NewStatements)]
        [DataRow("GAG", "AY-2122", "GAG-FY-2021-12345678-1_0", "", false, null, Constants.FolderNames.NewStatements)]
        public async Task Save_FullyMocked_StatementChannelVersion_RunsWithoutFault(
            string fundingSteamCode,
            string fundingPeriodCode,
            string providerFundingId,
            string fundingId,
            bool indicative,
            int? statementChannelVersion,
            string folderName)
        {
            // Arrange
            var datePartToday = DateTime.Now.ToString("yyyy-MM-dd");
            var expected = $"{fundingSteamCode}/{fundingPeriodCode}/{datePartToday}/{folderName}/12345678";

            var fundingDetails = new FundingDetails
            {
                FundingStreamCode = fundingSteamCode,
                FundingPeriodCode = fundingPeriodCode,
                ProviderFundingId = providerFundingId,
                FundingId = fundingId,
                Indicative = indicative,
                ChannelVersions = new ChannelVersion[] { },
                StatementChannelVersion = statementChannelVersion,
                FolderName = datePartToday
            };

            await fileShareSaveService.CreateFileDirectories(fundingSteamCode, fundingPeriodCode, datePartToday);

            // Act
            var actual = await fileShareSaveService.Save(fundingDetails, "12345678", new byte[] { });

            // Assert
            actual.Should().Be(expected);
        }
    }
}