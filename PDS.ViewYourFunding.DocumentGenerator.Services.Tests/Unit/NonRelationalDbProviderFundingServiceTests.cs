using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PDS.ViewYourFunding.DocumentGenerator.Repositories;
using PDS.ViewYourFunding.DocumentGenerator.Services.Config;
using PDS.ViewYourFunding.DocumentGenerator.Services.Interfaces;
using PDS.ViewYourFunding.DocumentGenerator.Services.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Tests
{
    /// <summary>
    /// Tests for the non-relational db provider funding service.
    /// </summary>
    [TestClass, TestCategory("Unit")]
    public class NonRelationalDbProviderFundingServiceTests
    {
        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task GetProviderFundingDetailsForDocuments_FullyMocked_RunsWithoutFault(bool indicative)
        {
            // Arrange
            var dbService = GetNonRelationalDb();
            dbService.Setup(s => s.GetDocumentsForSqlQuery<FundingDetails>(It.IsAny<string>())).ReturnsAsync(new List<FundingDetails>());
            dbService.Setup(s => s.PatchDocuments(It.IsAny<List<string>>(), It.IsAny<List<PatchOperation>>())).Returns(Task.CompletedTask);

            var service = new NonRelationalDbProviderFundingService(GetSettingService().Object, GetAuditLogService().Object, dbService.Object);
            var expected = new List<FundingDetails>();

            // Act
            var actual = await service.GetProviderFundingDetailsForDocuments(indicative);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task GetProviderFundingDetails_FullyMocked_RunsWithoutFault(bool forLAName)
        {
            // Arrange
            var expectedFundingDetails = new FundingDetails
            {
                ProviderFundingId = nameof(FundingDetails.ProviderFundingId),
                LACode = nameof(FundingDetails.LACode),
                LAName = nameof(FundingDetails.LAName)
            };

            var dbService = GetNonRelationalDb();
            dbService.Setup(s => s.GetDocumentsForSqlQuery<FundingDetails>(It.IsAny<string>())).ReturnsAsync(
                new List<FundingDetails>
                {
                    expectedFundingDetails
                });

            var service = new NonRelationalDbProviderFundingService(GetSettingService().Object, GetAuditLogService().Object, dbService.Object);

            // Act
            var actual = await service.GetProviderFundingDetails("id", forLAName);

            // Assert
            actual.Should().BeEquivalentTo(expectedFundingDetails);
        }

        /// <summary>
        /// Test that a fully mocked test runs without fault.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [TestMethod]
        public async Task AddDocumentGeneratedAttribute_FullyMocked_RunsWithoutFault()
        {
            // Arrange
            var dbService = GetNonRelationalDb();
            var settingService = GetSettingService();
            dbService.Setup(s => s.PatchDocument(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<PatchOperation>>())).Returns(Task.CompletedTask);

            var service = new NonRelationalDbProviderFundingService(settingService.Object, GetAuditLogService().Object, dbService.Object);

            // Act
            await service.AddDocumentGeneratedAttribute("id", "partitionKey", "true");

            //Assert
            dbService.Verify(s => s.PatchDocument(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<PatchOperation>>()), Times.Exactly(1));
        }

        /// <summary>
        /// Test that a fully mocked test runs without fault.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task AddDocumentGeneratedAttributeBatch_FullyMocked_RunsWithoutFault()
        {
            // Arrange
            var dbService = GetNonRelationalDb();
            var settingService = GetSettingService();
            dbService.Setup(s => s.PatchDocuments(It.IsAny<List<string>>(), It.IsAny<List<PatchOperation>>())).Returns(Task.CompletedTask);

            var service = new NonRelationalDbFundingService(settingService.Object, GetAuditLogService().Object, dbService.Object);

            // Act
            await service.AddDocumentGeneratedAttributeBatch(new List<string> { "id1:partitionKey1", "id2:partitionKey2" }, "true");

            //Assert
            dbService.Verify(s => s.PatchDocuments(It.IsAny<List<string>>(), It.IsAny<List<PatchOperation>>()), Times.Exactly(1));
        }

        /// <summary>
        /// Test that a fully mocked test runs without fault.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task AddRerunDateAttribute_FullyMocked_RunsWithoutFault()
        {
            // Arrange
            var dbService = GetNonRelationalDb();
            dbService.Setup(s => s.PatchDocuments(It.IsAny<string>(), It.IsAny<List<PatchOperation>>())).Returns(Task.CompletedTask);

            var service = new NonRelationalDbProviderFundingService(null, null, dbService.Object);

            // Act
            await service.AddRerunDateAttribute("created date time", "fundingStreamCode", string.Empty, DateTimeOffset.Now);

            //Assert
            dbService.Verify(s => s.PatchDocuments(It.IsAny<string>(), It.IsAny<List<PatchOperation>>()), Times.Exactly(2));
        }

        private Mock<INonRelationalDb> GetNonRelationalDb()
        {
            var service = new Mock<INonRelationalDb>(MockBehavior.Strict);

            return service;
        }

        private Mock<ISettingService> GetSettingService()
        {
            var service = new Mock<ISettingService>(MockBehavior.Strict);
            service.Setup(s => s.GetSetting("FilteredFundingStreams_ProviderFundings")).Returns("SETTING");
            service.Setup(s => s.GetSetting("FilteredVariations_ProviderFundings")).Returns("SETTING");
            service.Setup(s => s.GetIndicativeConfiguration()).Returns(new IndicativeConfiguration
            {
                IndicativeProviderStatusList = "status1,status2",
                IndicativeGroupingReason = nameof(IndicativeConfiguration.IndicativeGroupingReason)
            });

            return service;
        }

        private Mock<IAuditLogService> GetAuditLogService()
        {
            var service = new Mock<IAuditLogService>(MockBehavior.Strict);
            service.Setup(s => s.GetLastSuccessfulRunTime()).ReturnsAsync("2030-12-12");

            return service;
        }
    }
}