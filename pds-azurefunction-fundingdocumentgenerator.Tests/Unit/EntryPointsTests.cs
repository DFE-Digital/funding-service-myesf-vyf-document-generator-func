using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Core.Logging;
using PDS.ViewYourFunding.DocumentGenerator.Services;
using PDS.ViewYourFunding.DocumentGenerator.Services.Interfaces;
using PDS.ViewYourFunding.DocumentGenerator.Services.Messages;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static Moq.It;

namespace PDS.ViewYourFunding.DocumentGenerator.FunctionApp.Tests
{
    /// <summary>
    /// Tests for the entry points.
    /// </summary>
    [TestClass]
    public class EntryPointsTests
    {
        /// <summary>
        /// Test the servce bug entry point.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task Run_Timer_VerifyCounts()
        {
            // Arrange
            var logicService = GetLogicServiceForCosmosCall();
            var service = new EntryPoints(logicService.Object, GetAuditLogService().Object, GetTimerTriggerControlService().Object, GetMockLoggerAdapter().Object);

            // Act
            await service.Run_Timer_DocumentGenerator(null, default);

            // Assert
            logicService.Verify(
                s => s.RunDocumentGeneratorTimer(default),
                Times.Once());
        }

        /// <summary>
        /// Test the service bug entry point with exception.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void Run_WhenExceptionThrown_Logs()
        {
            // Arrange
            var logicService = GetLogicServiceWithException();
            var logService = GetAuditLogService();
            var service = new EntryPoints(logicService.Object, logService.Object, GetTimerTriggerControlService().Object, GetMockLoggerAdapter().Object);

            // Act
            Func<Task> method = async () => await service.Run_Timer_DocumentGenerator(null, default);

            // Assert
            method.Should().Throw<Exception>();

            logicService.Verify(
                s => s.RunDocumentGeneratorTimer(default),
                Times.Once());

            logService.Verify(s => s.Log(It.IsAny<string>()), Times.Once());
        }

        /// <summary>
        /// Test the run funding http entry point.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task FundingReports_Http_Verify()
        {
            // Arrange
            var logicService = GetLogicService();

            var service = new EntryPoints(logicService.Object, null, GetTimerTriggerControlService().Object, GetMockLoggerAdapter().Object);

            // Act
            var result = await service.Run_Http_GenerateFundingReports(GetHttpRequest_For_RunFunding().Object);

            // Assert
            result.Should().BeOfType(typeof(OkObjectResult));

            logicService.Verify(
                s => s.RunGenerateFundingReports(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Once());
        }

        /// <summary>
        /// Test the http entry point.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task Run_Http_GeneralData_VerifyCounts()
        {
            // Arrange
            var logicService = GetLogicService();
            var service = new EntryPoints(logicService.Object, GetAuditLogService().Object, GetTimerTriggerControlService().Object, GetMockLoggerAdapter().Object);

            // Act
            await service.Run_Http_GenerateSingleDocument(GetHttpRequest().Object);

            // Assert
            logicService.Verify(
                s => s.RunGenerateSingleDocument(null, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Once());
        }

        /// <summary>
        /// Test the rerun http entry point.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task Rerun_Http_Verify()
        {
            // Arrange
            var logicService = GetLogicService();

            var service = new EntryPoints(logicService.Object, null, GetTimerTriggerControlService().Object, GetMockLoggerAdapter().Object);

            // Act
            var result = await service.Run_Http_RerunDocumentGeneration(GetHttpRequest().Object);

            // Assert
            result.Should().BeOfType(typeof(OkObjectResult));

            logicService.Verify(
                s => s.RunRerunDocumentGeneration(It.IsAny<ResetAttributeRequest>()),
                Times.Once());
        }

        /// <summary>
        /// Test the rerun http entry point.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task Rerun_Http_WhenBadRequest_Verify()
        {
            // Arrange
            var logicService = GetLogicService();

            var service = new EntryPoints(logicService.Object, null, GetTimerTriggerControlService().Object, GetMockLoggerAdapter().Object);

            // Act
            var result = await service.Run_Http_RerunDocumentGeneration(GetHttpRequest_NotSet().Object);

            // Assert
            result.Should().BeOfType(typeof(BadRequestObjectResult));

            logicService.Verify(
                s => s.RunRerunDocumentGeneration(It.IsAny<ResetAttributeRequest>()),
                Times.Never());
        }

        /// <summary>
        /// Test the ComparePdfs http entry point.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task ComparePdfs_Http_Verify()
        {
            // Arrange
            var logicService = GetLogicService();

            var service = new EntryPoints(logicService.Object, null, GetTimerTriggerControlService().Object, GetMockLoggerAdapter().Object);

            // Act
            var result = await service.Run_Http_PdfComparison(GetHttpRequest_For_ComparePdfs().Object);

            // Assert
            result.Should().BeOfType(typeof(OkObjectResult));

            logicService.Verify(
                s => s.RunPdfComparison(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Once());
        }

        /// <summary>
        /// Test the ComparePdfs http entry point.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task ComparePdfs_Http_WhenBadRequest_Verify()
        {
            // Arrange
            var logicService = GetLogicService();

            var service = new EntryPoints(logicService.Object, null, GetTimerTriggerControlService().Object, GetMockLoggerAdapter().Object);

            // Act
            var result = await service.Run_Http_PdfComparison(GetHttpRequest_NotSet().Object);

            // Assert
            result.Should().BeOfType(typeof(BadRequestObjectResult));

            logicService.Verify(
                s => s.RunPdfComparison(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never());
        }

        private Mock<IAuditLogService> GetAuditLogService()
        {
            var service = new Mock<IAuditLogService>(MockBehavior.Strict);
            service.Setup(s => s.Log(It.IsAny<string>())).Returns(Task.FromResult<IsAnyType>(null));
            return service;
        }

        private Mock<ITimerTriggerControlService> GetTimerTriggerControlService()
        {
            var service = new Mock<ITimerTriggerControlService>(MockBehavior.Strict);

            service.SetupAllProperties();
            service.Setup(s => s.IsFundingReportHttpTriggerFunctionInProgress).Returns(false);
            service.Setup(s => s.IsRerunHttpTriggerFunctionInProgress).Returns(false);
            service.Setup(s => s.IsTimerTriggerFunctionInProgress).Returns(false);
            service.Setup(s => s.WaitTillTimerOrFundingReportToFinish(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            service.Setup(s => s.WaitTillTimerOrRerunToFinish(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            return service;
        }

        private Mock<ILogicService> GetLogicService()
        {
            var service = new Mock<ILogicService>(MockBehavior.Strict);

            service
                .Setup(s => s.RunGenerateFundingReports(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            service
                .Setup(s => s.RunGenerateSingleDocument(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<string> { "OK" });

            service
                .Setup(s => s.RunRerunDocumentGeneration(It.IsAny<ResetAttributeRequest>()))
                .Returns(Task.CompletedTask);

            service
                .Setup(s => s.RunPdfComparison(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            return service;
        }

        private Mock<ILogicService> GetLogicServiceForCosmosCall()
        {
            var service = new Mock<ILogicService>(MockBehavior.Strict);
            service
                .Setup(s => s.RunDocumentGeneratorTimer(default))
                .Returns(Task.CompletedTask);

            return service;
        }

        private Mock<ILogicService> GetLogicServiceWithException()
        {
            var service = new Mock<ILogicService>(MockBehavior.Strict);
            service
                .Setup(s => s.RunDocumentGeneratorTimer(default))
                .Throws(new Exception());

            return service;
        }

        private Mock<HttpRequest> GetHttpRequest()
        {
            var query = new Mock<IQueryCollection>(MockBehavior.Strict);
            query.Setup(s => s["fundingId"]).Returns((string)null);
            query.Setup(s => s["providerFundingId"]).Returns("I");
            query.Setup(s => s["fundingStreamCode"]).Returns("A");
            query.Setup(s => s["ukprn"]).Returns("B");
            query.Setup(s => s["fundingPeriodCode"]).Returns("C");
            query.Setup(s => s["cutoffDate"]).Returns("D");
            query.Setup(s => s["providerType"]).Returns("E");
            query.Setup(s => s["providerSubType"]).Returns("F");
            query.Setup(s => s["laName"]).Returns("G");
            query.Setup(s => s["laCode"]).Returns("H");
            query.Setup(s => s["createdDate"]).Returns("I");

            query.Setup(s => s.GetEnumerator()).Returns(new List<KeyValuePair<string, StringValues>>
            {
                new KeyValuePair<string, StringValues>("fundingStreamCode", new StringValues("NMSS"))
            }.GetEnumerator());

            var service = new Mock<HttpRequest>(MockBehavior.Strict);
            service.Setup(s => s.Query).Returns(query.Object);

            return service;
        }

        private Mock<HttpRequest> GetHttpRequest_For_ComparePdfs()
        {
            var query = new Mock<IQueryCollection>(MockBehavior.Strict);
            query.Setup(s => s["fundingStreamCode"]).Returns("A");
            query.Setup(s => s["fundingPeriodCode"]).Returns("B");
            query.Setup(s => s["folderSource"]).Returns("C");
            query.Setup(s => s["folderDestination"]).Returns("D");

            var service = new Mock<HttpRequest>(MockBehavior.Strict);
            service.Setup(s => s.Query).Returns(query.Object);

            return service;
        }

        private Mock<HttpRequest> GetHttpRequest_For_RunFunding()
        {
            var query = new Mock<IQueryCollection>(MockBehavior.Strict);
            query.Setup(s => s["groupTypeCode"]).Returns("A");
            query.Setup(s => s["excludedGroupTypeCode"]).Returns("B");
            query.Setup(s => s["groupTypeReason"]).Returns("C");
            query.Setup(s => s["fundingPeriodId"]).Returns("D");

            var service = new Mock<HttpRequest>(MockBehavior.Strict);
            service.Setup(s => s.Query).Returns(query.Object);

            return service;
        }

        private Mock<HttpRequest> GetHttpRequest_NotSet()
        {
            var query = new Mock<IQueryCollection>(MockBehavior.Strict);

            var service = new Mock<HttpRequest>(MockBehavior.Strict);
            service.Setup(s => s.Query).Returns(query.Object);

            return service;
        }

        private Mock<ILoggerAdapter<EntryPoints>> GetMockLoggerAdapter()
        {
            Mock<ILoggerAdapter<EntryPoints>> mockLogger = new Mock<ILoggerAdapter<EntryPoints>>(MockBehavior.Strict);

            mockLogger.Setup(l => l.LogInformation(It.IsAny<string>()));
            mockLogger.Setup(l => l.LogError(It.IsAny<Exception>(), It.IsAny<string>()));

            return mockLogger;
        }
    }
}