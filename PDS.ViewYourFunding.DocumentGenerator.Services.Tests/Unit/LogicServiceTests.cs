using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using Moq;
using Pds.Core.Logging;
using PDS.ViewYourFunding.DocumentGenerator.Services.Interfaces;
using PDS.ViewYourFunding.DocumentGenerator.Services.Messages;
using PDS.ViewYourFunding.DocumentGenerator.Services.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Tests
{
    /// <summary>
    /// Tests for the logic service.
    /// </summary>
    [TestClass]
    public class LogicServiceTests
    {
        /// <summary>
        /// Test that a fully mocked test runs without fault.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task Run_FullyMocked_RunsWithoutFault()
        {
            var pdfService = GetPDFConverterService();
            var httpService = GetHttpService();
            var datetimeService = GetDateTimeService();

            // Arrange
            var mainLogicService = new LogicService(
                pdfService.Object,
                null,
                httpService.Object,
                GetLayoutService().Object,
                null,
                GetProviderFundingService().Object,
                GetSaveService().Object,
                null,
                GetSettingService().Object,
                null,
                datetimeService.Object,
                GetMockLoggerAdapter().Object);

            var expected = new List<string> { "GAG_12345678_ForLayoutA", "GAG_12345678_ForLayoutB" };

            // Act
            var actual = await mainLogicService.RunGenerateSingleDocument(null, "GAG-FY-2021-12345678-1_0", "GAG", "12345678", "AY-2021", "General", "General", "LAName", "LACode", "2030-01-01");

            // Assert
            pdfService.Verify(
                s => s.CreatePdfFromHtml(
                    It.IsAny<string>(),
                    It.IsAny<double?>(),
                    It.IsAny<double?>(),
                    It.IsAny<double?>(),
                    It.IsAny<double?>(),
                    It.IsAny<double?>(),
                    It.IsAny<double?>(),
                    It.IsAny<IEnumerable<Bookmark>>(),
                    It.IsAny<string>()),
                Times.Exactly(2));

            httpService.Verify(
                s => s.ReadAsStringAsync("view-latest-funding/api/external/render?fundingId=&providerFundingId=GAG-FY-2021-12345678-1_0&fundingStreamCode=GAG&fundingPeriodCode=AY-2021&cutoffDate=2031-01-01&ukprn=12345678&layoutId=A_LAYOUT_ID"),
                Times.Once);

            actual.Should().BeEquivalentTo(expected);
        }

        /// <summary>
        /// Test that a fully mocked test runs without fault.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task Run_FullyMocked_WhenFileTypeOds_RunsWithoutFault()
        {
            var httpService = GetHttpService();
            var datetimeService = GetDateTimeService();

            // Arrange
            var mainLogicService = new LogicService(
                null,
                null,
                httpService.Object,
                GetLayoutService("ods").Object,
                null,
                GetProviderFundingService().Object,
                GetSaveService().Object,
                null,
                GetSettingService().Object,
                null,
                datetimeService.Object,
                GetMockLoggerAdapter().Object);

            var expected = new List<string> { "GAG_12345678_ForLayoutA", "GAG_12345678_ForLayoutB" };

            // Act
            var actual = await mainLogicService.RunGenerateSingleDocument(null, "GAG-FY-2021-12345678-1_0", "GAG", "12345678", "AY-2021", "General", "General", "LAName", "LACode", "2030-01-01");

            // Assert
            httpService.Verify(
                s => s.ReadAsByteArrayAsync("view-latest-funding/api/external/getFile?fundingId=&providerFundingId=GAG-FY-2021-12345678-1_0&fundingStreamCode=GAG&fundingPeriodCode=AY-2021&publicationDate=2031-01-01&cutoffDate=2031-01-01&ukprn=12345678&layoutId=A_LAYOUT_ID"),
                Times.Once);

            actual.Should().BeEquivalentTo(expected);
        }

        /// <summary>
        /// Test for run from cosmos data that a fully mocked test runs without fault.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task Run_FromCosmosData_FullyMocked_RunsWithoutFault()
        {
            var pdfService = GetPDFConverterService();
            var httpService = GetHttpService();
            var saveService = GetSaveService();
            var dateTimeservice = GetDateTimeService();

            // Arrange
            var mainLogicService = new LogicService(
                pdfService.Object,
                null,
                httpService.Object,
                GetLayoutService().Object,
                null,
                GetProviderFundingService().Object,
                saveService.Object,
                GetAuditLogService().Object,
                GetSettingService().Object,
                null,
                dateTimeservice.Object,
                GetMockLoggerAdapter().Object);

            // Act
            await mainLogicService.RunDocumentGeneratorTimer();

            // Assert
            AssertPDFServiceCall(pdfService, Times.Exactly(4));

            AssertCallToApi(httpService, Times.Once(), string.Empty, "GAG-FY-2021-12345678-1_0", "GAG", "12345678", "A_LAYOUT_ID");
            AssertCallToApi(httpService, Times.Once(), string.Empty, "GAG-FY-2021-12345678-1_0", "GAG", "12345678", "B_LAYOUT_ID");
            AssertCallToApi(httpService, Times.Once(), string.Empty, "GAG-FY-2021-78901234-2_0", "GAG", "78901234", "A_LAYOUT_ID");
            AssertCallToApi(httpService, Times.Once(), string.Empty, "GAG-FY-2021-78901234-2_0", "GAG", "78901234", "B_LAYOUT_ID");

            AssertCallToSaveService(saveService, Times.Once(), "GAG", "GAG_12345678_ForLayoutA.pdf");
            AssertCallToSaveService(saveService, Times.Once(), "GAG", "GAG_12345678_ForLayoutB.pdf");
            AssertCallToSaveService(saveService, Times.Once(), "GAG", "GAG_78901234_ForLayoutA.pdf");
            AssertCallToSaveService(saveService, Times.Once(), "GAG", "GAG_78901234_ForLayoutB.pdf");
        }

        /// <summary>
        /// Test for run from cosmos data that a fully mocked test runs without fault.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task RunFunding_FromCosmosData_FullyMocked_RunsWithoutFault()
        {
            var pdfService = GetPDFConverterService();
            var httpService = GetHttpService();
            var saveService = GetSaveService();
            var dateTimeservice = GetDateTimeService();

            // Arrange
            var mainLogicService = new LogicService(
                pdfService.Object,
                null,
                httpService.Object,
                GetLayoutService().Object,
                GetFundingService().Object,
                null,
                saveService.Object,
                GetAuditLogService().Object,
                GetSettingService().Object,
                null,
                dateTimeservice.Object,
                GetMockLoggerAdapter().Object);

            // Act
            await mainLogicService.RunGenerateFundingReports("A", "B", "C", "D");

            // Assert
            AssertPDFServiceCall(pdfService, Times.Exactly(4));

            AssertCallToApi(httpService, Times.Once(), "1619-FY-2021-12345678-1_0", string.Empty, "1619", "12345678", "C_LAYOUT_ID");
            AssertCallToApi(httpService, Times.Once(), "1619-FY-2021-12345678-1_0", string.Empty, "1619", "12345678", "D_LAYOUT_ID");
            AssertCallToApi(httpService, Times.Once(), "1619-FY-2021-78901234-2_0", string.Empty, "1619", "78901234", "C_LAYOUT_ID");
            AssertCallToApi(httpService, Times.Once(), "1619-FY-2021-78901234-2_0", string.Empty, "1619", "78901234", "D_LAYOUT_ID");

            AssertCallToSaveService(saveService, Times.Once(), "1619", "1619_12345678_ForLayoutC.pdf");
            AssertCallToSaveService(saveService, Times.Once(), "1619", "1619_12345678_ForLayoutD.pdf");
            AssertCallToSaveService(saveService, Times.Once(), "1619", "1619_78901234_ForLayoutC.pdf");
            AssertCallToSaveService(saveService, Times.Once(), "1619", "1619_78901234_ForLayoutD.pdf");
        }

        /// <summary>
        /// Test for run from cosmos data that a fully mocked test runs without fault.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task RunFunding_FromCosmosData_FullyMocked_AddsLADetails_RunsWithoutFault()
        {
            var pdfService = GetPDFConverterService();
            var httpService = GetHttpService();
            var saveService = GetSaveService();
            var dateTimeservice = GetDateTimeService();

            // Arrange
            var mainLogicService = new LogicService(
                pdfService.Object,
                null,
                httpService.Object,
                GetLayoutServiceForLAReport().Object,
                GetFundingWithProviderFungingsService().Object,
                GetProviderFundingService().Object,
                saveService.Object,
                GetAuditLogService().Object,
                GetSettingService().Object,
                null,
                dateTimeservice.Object,
                GetMockLoggerAdapter().Object);

            // Act
            await mainLogicService.RunGenerateFundingReports("A", "B", "C", "D");

            // Assert
            AssertPDFServiceCall(pdfService, Times.Exactly(1));

            AssertCallToApi(httpService, Times.Once(), "1619-FY-2021-12345678-1_0", string.Empty, "1619", "12345678", "C_LAYOUT_ID");

            AssertCallToSaveService(saveService, Times.Once(), "1619", "301_1619_12345678_For_Camden.pdf");
        }

        /// <summary>
        /// Test for run from cosmos data when exception thrown creates non errorneous pdfs.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [TestMethod, TestCategory("Unit")]

        public async Task RunFunding_FromCosmosData_WhenFundingException_RunsOtherPdfs()
        {
            var pdfService = GetPDFConverterService();
            var httpService = GetHttpService();
            var exception = new Exception();
            var saveService = GetSaveService();
            var dateTimeservice = GetDateTimeService();

            // Arrange
            var mainLogicService = new LogicService(
                pdfService.Object,
                null,
                httpService.Object,
                GetLayoutServiceWithExceptionForFunding(exception).Object,
                GetFundingService().Object,
                null,
                saveService.Object,
                GetAuditLogService().Object,
                GetSettingService().Object,
                null,
                dateTimeservice.Object,
                GetMockLoggerAdapter().Object);

            // Act
            await mainLogicService.RunGenerateFundingReports("A", "B", "C", "D");

            // Assert
            AssertPDFServiceCall(pdfService, Times.Exactly(2));

            AssertCallToApi(httpService, Times.Once(), "1619-FY-2021-12345678-1_0", string.Empty, "1619", "12345678", "C_LAYOUT_ID");
            AssertCallToApi(httpService, Times.Never(), "1619-FY-2021-12345678-1_0", string.Empty, "1619", "12345678", "D_LAYOUT_ID");
            AssertCallToApi(httpService, Times.Once(), "1619-FY-2021-78901234-2_0", string.Empty, "1619", "78901234", "C_LAYOUT_ID");
            AssertCallToApi(httpService, Times.Never(), "1619-FY-2021-78901234-2_0", string.Empty, "1619", "78901234", "D_LAYOUT_ID");

            AssertCallToSaveService(saveService, Times.Once(), "1619", "1619_12345678_ForLayoutC.pdf");
            AssertCallToSaveService(saveService, Times.Never(), "1619", "1619_12345678_ForLayoutD.pdf");
            AssertCallToSaveService(saveService, Times.Once(), "1619", "1619_78901234_ForLayoutC.pdf");
            AssertCallToSaveService(saveService, Times.Never(), "1619", "1619_78901234_ForLayoutD.pdf");
        }

        /// <summary>
        /// Test for run from cosmos data when exception thrown creates non errorneous pdfs.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task Run_FromCosmosData_WhenProviderFundingException_RunsOtherPdfs()
        {
            var pdfService = GetPDFConverterService();
            var httpService = GetHttpService();
            var exception = new Exception();
            var saveService = GetSaveService();
            var dateTimeservice = GetDateTimeService();

            // Arrange
            var mainLogicService = new LogicService(
                pdfService.Object,
                null,
                httpService.Object,
                GetLayoutServiceWithExceptionForProviderFunding(exception).Object,
                null,
                GetProviderFundingService().Object,
                saveService.Object,
                GetAuditLogService().Object,
                GetSettingService().Object,
                null,
                dateTimeservice.Object,
                GetMockLoggerAdapter().Object);

            var expected = new List<string>
            {
                "1619_12345678_ForLayoutC",
                "1619_12345678_ForLayoutD",
                "1619_78901234_ForLayoutC",
                "1619_78901234_ForLayoutD",
                "GAG_12345678_ForLayoutA",
                "GAG_78901234_ForLayoutA"
            };

            // Act
            await mainLogicService.RunDocumentGeneratorTimer();

            // Assert
            AssertPDFServiceCall(pdfService, Times.Exactly(2));

            AssertCallToApi(httpService, Times.Once(), string.Empty, "GAG-FY-2021-12345678-1_0", "GAG", "12345678", "A_LAYOUT_ID");
            AssertCallToApi(httpService, Times.Never(), string.Empty, "GAG-FY-2021-12345678-1_0", "GAG", "12345678", "B_LAYOUT_ID");
            AssertCallToApi(httpService, Times.Once(), string.Empty, "GAG-FY-2021-78901234-2_0", "GAG", "78901234", "A_LAYOUT_ID");
            AssertCallToApi(httpService, Times.Never(), string.Empty, "GAG-FY-2021-78901234-2_0", "GAG", "78901234", "B_LAYOUT_ID");

            AssertCallToSaveService(saveService, Times.Once(), "GAG", "GAG_12345678_ForLayoutA.pdf");
            AssertCallToSaveService(saveService, Times.Never(), "GAG", "GAG_12345678_ForLayoutB.pdf");
            AssertCallToSaveService(saveService, Times.Once(), "GAG", "GAG_78901234_ForLayoutA.pdf");
            AssertCallToSaveService(saveService, Times.Never(), "GAG", "GAG_78901234_ForLayoutB.pdf");
        }

        /// <summary>
        /// Test for run from cosmos data when storage exception thrown stops execution after that.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void RunFunding_FromCosmosData_WhenFundingStorageException_StopsExecution()
        {
            var pdfService = GetPDFConverterService();
            var httpService = GetHttpService();
            var exception = new StorageException();
            var saveService = GetSaveServiceWithStorageExceptionForFunding(exception);
            var dateTimeservice = GetDateTimeService();

            // Arrange
            var mainLogicService = new LogicService(
                pdfService.Object,
                null,
                httpService.Object,
                GetLayoutService().Object,
                GetFundingService().Object,
                null,
                saveService.Object,
                GetAuditLogService().Object,
                GetSettingService().Object,
                null,
                dateTimeservice.Object,
                GetMockLoggerAdapter().Object);

            // Act
            Func<Task> method = async () => await mainLogicService.RunGenerateFundingReports("A", "B", "C", "D");

            // Assert
            method.Should().ThrowAsync<StorageException>();
        }

        /// <summary>
        /// Test for run from cosmos data when storage exception thrown stops execution after that.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void Run_FromCosmosData_WhenProviderFundingStorageException_StopsExecution()
        {
            var pdfService = GetPDFConverterService();
            var httpService = GetHttpService();
            var exception = new StorageException();
            var dateTimeservice = GetDateTimeService();

            // Arrange
            var mainLogicService = new LogicService(
                pdfService.Object,
                null,
                httpService.Object,
                GetLayoutService().Object,
                null,
                GetProviderFundingService().Object,
                GetSaveServiceWithStorageExceptionForProviderFunding(exception).Object,
                GetAuditLogService().Object,
                GetSettingService().Object,
                null,
                dateTimeservice.Object,
                GetMockLoggerAdapter().Object);

            // Act
            Func<Task> method = async () => await mainLogicService.RunDocumentGeneratorTimer();

            // Assert
            method.Should().ThrowAsync<StorageException>();
        }

        /// <summary>
        /// Test for run from cosmos data when there is a running instance of feed reader doesn't proceed.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task Run_FromCosmosData_WhenRunningFeedReader_DoesNotProceed()
        {
            var pdfService = GetPDFConverterService();
            var httpService = GetHttpService();
            var dateTimeservice = GetDateTimeService();

            // Arrange
            var mainLogicService = new LogicService(
                pdfService.Object,
                null,
                httpService.Object,
                GetLayoutService().Object,
                GetFundingService().Object,
                GetProviderFundingService().Object,
                GetSaveService().Object,
                GetAuditLogServiceReturningRunningFeedInstance().Object,
                GetSettingService().Object,
                null,
                dateTimeservice.Object,
                GetMockLoggerAdapter().Object);

            // Act
            await mainLogicService.RunDocumentGeneratorTimer();

            // Assert
            AssertPDFServiceCall(pdfService, Times.Never());

            httpService.Verify(
                s => s.ReadAsStringAsync(It.IsAny<string>()),
                Times.Never);
        }

        /// <summary>
        /// Test that a fully mocked test runs without fault.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task ResetPdfAttribute_FullyMocked_RunsWithoutFault()
        {
            // Arrange
            const string createdSinceDate = "Created Since Datetime";
            const string fundingStreamCode = "fundingStreamCode";
            var request = new ResetAttributeRequest
            {
                SinceCreatedDate = createdSinceDate,
                FundingStreamCode = fundingStreamCode
            };

            var fundingService = new Mock<IFundingService>(MockBehavior.Strict);
            fundingService.Setup(s => s.AddRerunDateAttribute(createdSinceDate, fundingStreamCode, It.IsAny<string>(), It.IsAny<DateTimeOffset>())).Returns(Task.CompletedTask);

            var providerFundingService = new Mock<IProviderFundingService>(MockBehavior.Strict);
            providerFundingService.Setup(s => s.AddRerunDateAttribute(createdSinceDate, fundingStreamCode, It.IsAny<string>(), It.IsAny<DateTimeOffset>())).Returns(Task.CompletedTask);

            var dateTimeservice = GetDateTimeService();

            var mainLogicService = new LogicService(
                null,
                null,
                null,
                null,
                fundingService.Object,
                providerFundingService.Object,
                null,
                null,
                null,
                null,
                dateTimeservice.Object,
                GetMockLoggerAdapter().Object);

            // Act
            await mainLogicService.RunRerunDocumentGeneration(request);

            // Assert
            fundingService.Verify(
                s => s.AddRerunDateAttribute(createdSinceDate, fundingStreamCode, It.IsAny<string>(), It.IsAny<DateTimeOffset>()),
                Times.Once());

            providerFundingService.Verify(
                s => s.AddRerunDateAttribute(createdSinceDate, fundingStreamCode, It.IsAny<string>(), It.IsAny<DateTimeOffset>()),
                Times.Once());
        }

        /// <summary>
        /// Test that a fully mocked test runs without fault.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        /// <param name="resetFunding">Simulate reset funding data.</param>
        /// <param name="resetProviderFunding">Simulate provider funding data.</param>
        [TestMethod, TestCategory("Unit")]
        [DataRow(true, true)]
        [DataRow(false, true)]
        [DataRow(true, false)]
        public async Task ResetPdfAttribute_FullyMocked_ResetFlags_RunsWithoutFault(
            bool resetFunding,
            bool resetProviderFunding)
        {
            // Arrange
            const string createdSinceDate = "Created Since Datetime";
            const string fundingStreamCode = "fundingStreamCode";
            var request = new ResetAttributeRequest
            {
                SinceCreatedDate = createdSinceDate,
                FundingStreamCode = fundingStreamCode,
                ResetProviderFunding = resetProviderFunding,
                ResetFunding = resetFunding
            };

            var fundingService = new Mock<IFundingService>(MockBehavior.Strict);
            fundingService.Setup(s => s.AddRerunDateAttribute(createdSinceDate, fundingStreamCode, It.IsAny<string>(), It.IsAny<DateTimeOffset>())).Returns(Task.CompletedTask);

            var providerFundingService = new Mock<IProviderFundingService>(MockBehavior.Strict);
            providerFundingService.Setup(s => s.AddRerunDateAttribute(createdSinceDate, fundingStreamCode, It.IsAny<string>(), It.IsAny<DateTimeOffset>())).Returns(Task.CompletedTask);

            var dateTimeservice = GetDateTimeService();

            var mainLogicService = new LogicService(
                null,
                null,
                null,
                null,
                fundingService.Object,
                providerFundingService.Object,
                null,
                null,
                null,
                null,
                dateTimeservice.Object,
                GetMockLoggerAdapter().Object);

            // Act
            await mainLogicService.RunRerunDocumentGeneration(request);

            // Assert
            fundingService.Verify(
                s => s.AddRerunDateAttribute(createdSinceDate, fundingStreamCode, It.IsAny<string>(), It.IsAny<DateTimeOffset>()),
                resetFunding ? Times.Once() : Times.Never());

            providerFundingService.Verify(
                s => s.AddRerunDateAttribute(createdSinceDate, fundingStreamCode, It.IsAny<string>(), It.IsAny<DateTimeOffset>()),
                resetProviderFunding ? Times.Once() : Times.Never());
        }

        /// <summary>
        /// Test that a fully mocked test runs without fault.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task ComparePdfs_FullyMocked_RunsWithoutFault()
        {
            // Arrange
            var fundingStreamCode = "Funding Stream Code";
            var fundingPeriodCode = "Funding Period Code";
            var sourceFolder = "Source Folder";
            var destinationFolder = "Destination Folder";
            var parallelRunSize = 10;

            var settingService = new Mock<ISettingService>(MockBehavior.Strict);
            settingService.Setup(s => s.GetSetting("Processing_Run_Size_Comparison")).Returns("10");

            var fileSharePdfComparerService = new Mock<IFileSharePdfComparerService>(MockBehavior.Strict);
            fileSharePdfComparerService.Setup(s => s.ComparePdfs(fundingStreamCode, fundingPeriodCode, sourceFolder, destinationFolder, parallelRunSize)).Returns(Task.CompletedTask);

            var mainLogicService = new LogicService(
                null,
                fileSharePdfComparerService.Object,
                null,
                null,
                null,
                null,
                null,
                null,
                settingService.Object,
                null,
                null,
                GetMockLoggerAdapter().Object);

            // Act
            await mainLogicService.RunPdfComparison(fundingStreamCode, fundingPeriodCode, sourceFolder, destinationFolder);

            // Assert
            fileSharePdfComparerService.Verify(
                s => s.ComparePdfs(fundingStreamCode, fundingPeriodCode, sourceFolder, destinationFolder, parallelRunSize),
                Times.Once());
        }


        #region Private Helpers

        private void AssertCallToApi(Mock<IHttpService> service, Times times, string fundingId, string providerFundingId, string fundingStreamCode, string ukprn, string layoutId)
        {
            service.Verify(
                s => s.ReadAsStringAsync($"view-latest-funding/api/external/render?fundingId={fundingId}&providerFundingId={providerFundingId}&fundingStreamCode={fundingStreamCode}&fundingPeriodCode=AY-2021&cutoffDate=2031-01-01&ukprn={ukprn}&layoutId={layoutId}"),
                times);
        }

        private void AssertPDFServiceCall(Mock<IPDFConverterService> service, Times times)
        {
            service.Verify(
                s => s.CreatePdfFromHtml(
                    It.IsAny<string>(),
                    It.IsAny<double?>(),
                    It.IsAny<double?>(),
                    It.IsAny<double?>(),
                    It.IsAny<double?>(),
                    It.IsAny<double?>(),
                    It.IsAny<double?>(),
                    It.IsAny<IEnumerable<Bookmark>>(),
                    It.IsAny<string>()),
                times);
        }

        private void AssertCallToSaveService(Mock<ISaveService> service, Times times, string fundingStreamCode, string fileName)
        {
            service.Verify(
                s => s.Save(It.IsAny<FundingDetails>(), fileName, It.IsAny<byte[]>()),
                times);
        }

        private Mock<IPDFConverterService> GetPDFConverterService()
        {
            var service = new Mock<IPDFConverterService>(MockBehavior.Strict);
            service
                .Setup(s => s.CreatePdfFromHtml(
                    It.IsAny<string>(),
                    It.IsAny<double?>(),
                    It.IsAny<double?>(),
                    It.IsAny<double?>(),
                    It.IsAny<double?>(),
                    It.IsAny<double?>(),
                    It.IsAny<double?>(),
                    It.IsAny<IEnumerable<Bookmark>>(),
                    It.IsAny<string>()))
                .Returns(new byte[] { });

            return service;
        }

        private Mock<IHttpService> GetHttpService()
        {
            var service = new Mock<IHttpService>(MockBehavior.Strict);
            service.Setup(s => s.ReadAsStringAsync(It.IsAny<string>())).ReturnsAsync("<html><body>ABC</body></html>");
            service.Setup(s => s.ReadAsByteArrayAsync(It.IsAny<string>())).ReturnsAsync(new byte[] { 0x00, 0x00, 0x00, 0x01 });

            return service;
        }

        private Mock<IDateTimeService> GetDateTimeService()
        {
            var service = new Mock<IDateTimeService>(MockBehavior.Strict);
            service.Setup(d => d.GetDateTimePathComponent()).Returns(DateTime.Now.ToString("yyyy-MM-dd HH-MM"));
            service.Setup(d => d.GetDateTimePathComponent(It.IsAny<DateTime>())).Returns(DateTime.Now.ToString("yyyy-MM-dd HH-MM"));
            return service;
        }

        private Mock<IFundingService> GetFundingWithProviderFungingsService()
        {
            var service = new Mock<IFundingService>(MockBehavior.Strict);
            service.Setup(s => s.GetFundingDetailsForDocuments(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new List<FundingDetails>
            {
                new FundingDetails
                {
                    FundingId = "1619-FY-2021-12345678-1_0",
                    FundingStreamCode = "1619",
                    Ukprn = "12345678",
                    FundingPeriodCode = "AY-2021",
                    ProviderType = "General",
                    ProviderSubType = "General",
                    LAName = "LAName",
                    CutoffDate = "2030-01-01",
                    ProviderFundings = new List<string> { "providerFundingId1" }
                },
            });
            service.Setup(s => s.AddDocumentGeneratedAttribute("1619-FY-2021-12345678-1_0", "12345678", "true")).Returns(Task.CompletedTask);

            return service;
        }

        private Mock<IFundingService> GetFundingService()
        {
            var service = new Mock<IFundingService>(MockBehavior.Strict);
            service.Setup(s => s.GetFundingDetailsForDocuments(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new List<FundingDetails>
            {
                new FundingDetails
            {
                FundingId = "1619-FY-2021-12345678-1_0",
                FundingStreamCode = "1619",
                Ukprn = "12345678",
                FundingPeriodCode = "AY-2021",
                ProviderType = "General",
                ProviderSubType = "General",
                LAName = "LAName",
                CutoffDate = "2030-01-01"
            },
                new FundingDetails
            {
                FundingId = "1619-FY-2021-78901234-2_0",
                FundingStreamCode = "1619",
                Ukprn = "78901234",
                FundingPeriodCode = "AY-2021",
                ProviderType = "General",
                ProviderSubType = "General",
                LAName = "LAName",
                CutoffDate = "2030-01-01"
            }
            });
            service.Setup(s => s.AddDocumentGeneratedAttribute(It.IsAny<string>(), It.IsAny<string>(), "true")).Returns(Task.CompletedTask);

            return service;
        }

        private Mock<IProviderFundingService> GetProviderFundingService()
        {
            var service = new Mock<IProviderFundingService>(MockBehavior.Strict);
            service.Setup(s => s.GetProviderFundingDetailsForDocuments(It.IsAny<bool>())).ReturnsAsync(new List<FundingDetails>
            {
                new FundingDetails
                {
                ProviderFundingId = "GAG-FY-2021-12345678-1_0",
                FundingStreamCode = "GAG",
                Ukprn = "12345678",
                FundingPeriodCode = "AY-2021",
                ProviderType = "General",
                ProviderSubType = "General",
                LAName = "LAName",
                CutoffDate = "2030-01-01"
                },
                new FundingDetails
            {
                ProviderFundingId = "GAG-FY-2021-78901234-2_0",
                FundingStreamCode = "GAG",
                Ukprn = "78901234",
                FundingPeriodCode = "AY-2021",
                ProviderType = "General",
                ProviderSubType = "General",
                LAName = "LAName",
                CutoffDate = "2030-01-01"
            }
            });
            var fundingDetails = new FundingDetails
            {
                ProviderFundingId = "GAG-FY-2021-12345678-1_0",
                FundingStreamCode = "GAG",
                Ukprn = "12345678",
                FundingPeriodCode = "AY-2021",
                ProviderType = "General",
                ProviderSubType = "General",
                LAName = "Camden",
                LACode = "301",
                CutoffDate = "2030-01-01"
            };

            service.Setup(s => s.GetProviderFundingDetails(It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(fundingDetails);
            service.Setup(s => s.AddDocumentGeneratedAttribute(It.IsAny<string>(), It.IsAny<string>(), "true")).Returns(Task.CompletedTask);

            return service;
        }

        private Mock<ILayoutService> GetLayoutService(string fileType = "pdf")
        {
            var service = new Mock<ILayoutService>(MockBehavior.Strict);
            service.Setup(s => s.LookupLayoutId("GAG", "AY-2021", "2031-01-01", "General", "General")).Returns(("LayoutLookupGAG", new List<string> { "A_LAYOUT_ID", "B_LAYOUT_ID" }));
            service.Setup(s => s.LookupLayoutId("1619", "AY-2021", "2031-01-01", "General", "General")).Returns(("LayoutLookup1619", new List<string> { "C_LAYOUT_ID", "D_LAYOUT_ID" }));

            service.Setup(s => s.LookupFileNameFormat("LayoutLookupGAG", 0)).Returns("FUNDINGSTREAMCODE_UKPRN_ForLayoutA");
            service.Setup(s => s.LookupFileNameFormat("LayoutLookupGAG", 1)).Returns("FUNDINGSTREAMCODE_UKPRN_ForLayoutB");
            service.Setup(s => s.LookupFileNameFormat("LayoutLookup1619", 0)).Returns("FUNDINGSTREAMCODE_UKPRN_ForLayoutC");
            service.Setup(s => s.LookupFileNameFormat("LayoutLookup1619", 1)).Returns("FUNDINGSTREAMCODE_UKPRN_ForLayoutD");

            service.Setup(s => s.LookupFileType("LayoutLookupGAG")).Returns(fileType);
            service.Setup(s => s.LookupFileType("LayoutLookup1619")).Returns(fileType);

            service.Setup(s => s.GetLayout("A_LAYOUT_ID")).ReturnsAsync(new Dictionary<string, object>());
            service.Setup(s => s.GetLayout("B_LAYOUT_ID")).ReturnsAsync(new Dictionary<string, object>());
            service.Setup(s => s.GetLayout("C_LAYOUT_ID")).ReturnsAsync(new Dictionary<string, object>());
            service.Setup(s => s.GetLayout("D_LAYOUT_ID")).ReturnsAsync(new Dictionary<string, object>());

            return service;
        }

        private Mock<ILayoutService> GetLayoutServiceForLAReport()
        {
            var service = new Mock<ILayoutService>(MockBehavior.Strict);
            service.Setup(s => s.LookupLayoutId("1619", "AY-2021", "2031-01-01", "General", "General"))
                .Returns(("LayoutLookup1619", new List<string> { "C_LAYOUT_ID" }));

            service.Setup(s => s.LookupFileNameFormat("LayoutLookup1619", 0))
                .Returns("LACODE_FUNDINGSTREAMCODE_UKPRN_For_LANAME");

            service.Setup(s => s.LookupFileType("LayoutLookup1619"))
                .Returns("pdf");

            service.Setup(s => s.GetLayout("C_LAYOUT_ID")).ReturnsAsync(new Dictionary<string, object>());

            return service;
        }

        private Mock<ILayoutService> GetLayoutServiceWithExceptionForFunding(Exception exception)
        {
            var service = new Mock<ILayoutService>(MockBehavior.Strict);
            service.Setup(s => s.LookupLayoutId("GAG", "AY-2021", "2031-01-01", "General", "General")).Returns(("LayoutLookupGAG", new List<string> { "A_LAYOUT_ID", "B_LAYOUT_ID" }));
            service.Setup(s => s.LookupLayoutId("1619", "AY-2021", "2031-01-01", "General", "General")).Returns(("LayoutLookup1619", new List<string> { "C_LAYOUT_ID", "D_LAYOUT_ID" }));

            service.Setup(s => s.LookupFileNameFormat("LayoutLookupGAG", 0)).Returns("FUNDINGSTREAMCODE_UKPRN_ForLayoutA");
            service.Setup(s => s.LookupFileNameFormat("LayoutLookupGAG", 1)).Returns("FUNDINGSTREAMCODE_UKPRN_ForLayoutB");
            service.Setup(s => s.LookupFileNameFormat("LayoutLookup1619", 0)).Returns("FUNDINGSTREAMCODE_UKPRN_ForLayoutC");
            service.Setup(s => s.LookupFileNameFormat("LayoutLookup1619", 1)).Throws(exception);

            service.Setup(s => s.LookupFileType("LayoutLookupGAG")).Returns("pdf");
            service.Setup(s => s.LookupFileType("LayoutLookupGAG")).Returns("pdf");
            service.Setup(s => s.LookupFileType("LayoutLookup1619")).Returns("pdf");
            service.Setup(s => s.LookupFileType("LayoutLookup1619")).Returns("pdf");

            service.Setup(s => s.GetLayout("A_LAYOUT_ID")).ReturnsAsync(new Dictionary<string, object>());
            service.Setup(s => s.GetLayout("B_LAYOUT_ID")).ReturnsAsync(new Dictionary<string, object>());
            service.Setup(s => s.GetLayout("C_LAYOUT_ID")).ReturnsAsync(new Dictionary<string, object>());
            service.Setup(s => s.GetLayout("D_LAYOUT_ID")).ReturnsAsync(new Dictionary<string, object>());

            return service;
        }

        private Mock<ILayoutService> GetLayoutServiceWithExceptionForProviderFunding(Exception exception)
        {
            var service = new Mock<ILayoutService>(MockBehavior.Strict);
            service.Setup(s => s.LookupLayoutId("GAG", "AY-2021", "2031-01-01", "General", "General")).Returns(("LayoutLookupGAG", new List<string> { "A_LAYOUT_ID", "B_LAYOUT_ID" }));
            service.Setup(s => s.LookupLayoutId("1619", "AY-2021", "2031-01-01", "General", "General")).Returns(("LayoutLookup1619", new List<string> { "C_LAYOUT_ID", "D_LAYOUT_ID" }));

            service.Setup(s => s.LookupFileNameFormat("LayoutLookupGAG", 0)).Returns("FUNDINGSTREAMCODE_UKPRN_ForLayoutA");
            service.Setup(s => s.LookupFileNameFormat("LayoutLookupGAG", 1)).Throws(exception);
            service.Setup(s => s.LookupFileNameFormat("LayoutLookup1619", 0)).Returns("FUNDINGSTREAMCODE_UKPRN_ForLayoutC");
            service.Setup(s => s.LookupFileNameFormat("LayoutLookup1619", 1)).Returns("FUNDINGSTREAMCODE_UKPRN_ForLayoutD");

            service.Setup(s => s.LookupFileType("LayoutLookupGAG")).Returns("pdf");
            service.Setup(s => s.LookupFileType("LayoutLookup1619")).Returns("pdf");

            service.Setup(s => s.GetLayout("A_LAYOUT_ID")).ReturnsAsync(new Dictionary<string, object>());
            service.Setup(s => s.GetLayout("B_LAYOUT_ID")).ReturnsAsync(new Dictionary<string, object>());
            service.Setup(s => s.GetLayout("C_LAYOUT_ID")).ReturnsAsync(new Dictionary<string, object>());
            service.Setup(s => s.GetLayout("D_LAYOUT_ID")).ReturnsAsync(new Dictionary<string, object>());

            return service;
        }

        private Mock<ISaveService> GetSaveService()
        {
            var service = new Mock<ISaveService>(MockBehavior.Strict);

            service
               .Setup(s => s.CreateFileDirectories("GAG", "AY-2021", It.IsAny<string>()))
               .Returns(Task.CompletedTask);
            service
               .Setup(s => s.CreateFileDirectories("1619", "AY-2021", It.IsAny<string>()))
               .Returns(Task.CompletedTask);

            service
                .Setup(s => s.Save(It.IsAny<FundingDetails>(), "GAG_12345678_ForLayoutA.pdf", It.IsAny<byte[]>()))
                .ReturnsAsync("GAG_12345678_ForLayoutA");
            service
                .Setup(s => s.Save(It.IsAny<FundingDetails>(), "GAG_12345678_ForLayoutA.ods", It.IsAny<byte[]>()))
                .ReturnsAsync("GAG_12345678_ForLayoutA");
            service
                .Setup(s => s.Save(It.IsAny<FundingDetails>(), "GAG_12345678_ForLayoutB.pdf", It.IsAny<byte[]>()))
                .ReturnsAsync("GAG_12345678_ForLayoutB");
            service
                .Setup(s => s.Save(It.IsAny<FundingDetails>(), "GAG_12345678_ForLayoutB.ods", It.IsAny<byte[]>()))
                .ReturnsAsync("GAG_12345678_ForLayoutB");
            service
                .Setup(s => s.Save(It.IsAny<FundingDetails>(), "GAG_78901234_ForLayoutA.pdf", It.IsAny<byte[]>()))
                .ReturnsAsync("GAG_78901234_ForLayoutA");
            service
                .Setup(s => s.Save(It.IsAny<FundingDetails>(), "GAG_78901234_ForLayoutB.pdf", It.IsAny<byte[]>()))
                .ReturnsAsync("GAG_78901234_ForLayoutB");
            service
                .Setup(s => s.Save(It.IsAny<FundingDetails>(), "1619_12345678_ForLayoutC.pdf", It.IsAny<byte[]>()))
                .ReturnsAsync("1619_12345678_ForLayoutC");
            service
                .Setup(s => s.Save(It.IsAny<FundingDetails>(), "1619_12345678_ForLayoutD.pdf", It.IsAny<byte[]>()))
                .ReturnsAsync("1619_12345678_ForLayoutD");
            service
                .Setup(s => s.Save(It.IsAny<FundingDetails>(), "1619_78901234_ForLayoutC.pdf", It.IsAny<byte[]>()))
                .ReturnsAsync("1619_78901234_ForLayoutC");
            service
                .Setup(s => s.Save(It.IsAny<FundingDetails>(), "1619_78901234_ForLayoutD.pdf", It.IsAny<byte[]>()))
                .ReturnsAsync("1619_78901234_ForLayoutD");
            service
                .Setup(s => s.Save(It.IsAny<FundingDetails>(), "301_1619_12345678_For_Camden.pdf", It.IsAny<byte[]>()))
                .ReturnsAsync("301_1619_12345678_For_Camden");

            return service;
        }

        private Mock<ISaveService> GetSaveServiceWithStorageExceptionForFunding(StorageException exception)
        {
            var service = new Mock<ISaveService>(MockBehavior.Strict);

            service
               .Setup(s => s.CreateFileDirectories("GAG", "AY-2021", It.IsAny<string>()))
               .Returns(Task.CompletedTask);
            service
               .Setup(s => s.CreateFileDirectories("1619", "AY-2021", It.IsAny<string>()))
               .Returns(Task.CompletedTask);

            service
                .Setup(s => s.Save(It.IsAny<FundingDetails>(), "1619_12345678_ForLayoutC.pdf", It.IsAny<byte[]>()))
                .Throws(exception);

            return service;
        }

        private Mock<ISaveService> GetSaveServiceWithStorageExceptionForProviderFunding(StorageException exception)
        {
            var service = new Mock<ISaveService>(MockBehavior.Strict);
            service
               .Setup(s => s.CreateFileDirectories("GAG", "AY-2021", It.IsAny<string>()))
               .Returns(Task.CompletedTask);
            service
               .Setup(s => s.CreateFileDirectories("1619", "AY-2021", It.IsAny<string>()))
               .Returns(Task.CompletedTask);

            service
                .Setup(s => s.Save(It.IsAny<FundingDetails>(), "1619_12345678_ForLayoutC.pdf", It.IsAny<byte[]>()))
                .ReturnsAsync("1619_12345678_ForLayoutC");
            service
                .Setup(s => s.Save(It.IsAny<FundingDetails>(), "1619_12345678_ForLayoutD.pdf", It.IsAny<byte[]>()))
                .ReturnsAsync("1619_12345678_ForLayoutD");
            service
                .Setup(s => s.Save(It.IsAny<FundingDetails>(), "1619_78901234_ForLayoutC.pdf", It.IsAny<byte[]>()))
                .ReturnsAsync("1619_78901234_ForLayoutC");
            service
                .Setup(s => s.Save(It.IsAny<FundingDetails>(), "1619_78901234_ForLayoutD.pdf", It.IsAny<byte[]>()))
                .ReturnsAsync("1619_78901234_ForLayoutD");
            service
                .Setup(s => s.Save(It.IsAny<FundingDetails>(), "GAG_12345678_ForLayoutA.pdf", It.IsAny<byte[]>()))
                .ReturnsAsync("GAG_12345678_ForLayoutA");
            service
                .Setup(s => s.Save(It.IsAny<FundingDetails>(), "GAG_12345678_ForLayoutA.ods", It.IsAny<byte[]>()))
                .ReturnsAsync("GAG_12345678_ForLayoutA");
            service
                .Setup(s => s.Save(It.IsAny<FundingDetails>(), "GAG_12345678_ForLayoutB.pdf", It.IsAny<byte[]>()))
                .ReturnsAsync("GAG_12345678_ForLayoutB");
            service
                .Setup(s => s.Save(It.IsAny<FundingDetails>(), "GAG_12345678_ForLayoutB.ods", It.IsAny<byte[]>()))
                .ReturnsAsync("GAG_12345678_ForLayoutB");
            service
                .Setup(s => s.Save(It.IsAny<FundingDetails>(), "GAG_78901234_ForLayoutA.pdf", It.IsAny<byte[]>()))
                .Throws(exception);

            return service;
        }

        private Mock<IAuditLogService> GetAuditLogService()
        {
            var service = new Mock<IAuditLogService>(MockBehavior.Strict);
            service.Setup(s => s.CheckNoRunningInstanceOfFeedReader()).ReturnsAsync(true);

            return service;
        }

        private Mock<IAuditLogService> GetAuditLogServiceReturningRunningFeedInstance()
        {
            var service = new Mock<IAuditLogService>(MockBehavior.Strict);
            service.Setup(s => s.CheckNoRunningInstanceOfFeedReader()).ReturnsAsync(false);

            return service;
        }

        private Mock<ISettingService> GetSettingService()
        {
            var service = new Mock<ISettingService>(MockBehavior.Strict);
            service.Setup(s => s.GetSetting("Parallel_Run_Batch_Size")).Returns("10");

            return service;
        }

        private Mock<ILoggerAdapter<LogicService>> GetMockLoggerAdapter()
        {
            Mock<ILoggerAdapter<LogicService>> mockLogger = new Mock<ILoggerAdapter<LogicService>>(MockBehavior.Strict);

            mockLogger.Setup(l => l.LogInformation(It.IsAny<string>()));
            mockLogger.Setup(l => l.LogInformation(It.IsAny<Exception>(), It.IsAny<string>()));
            mockLogger.Setup(l => l.LogError(It.IsAny<Exception>(), It.IsAny<string>()));

            return mockLogger;
        }

        #endregion
    }
}