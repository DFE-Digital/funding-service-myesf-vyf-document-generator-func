using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PDS.ViewYourFunding.DocumentGenerator.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Tests
{
    /// <summary>
    /// Tests for the non-releational db audit log service.
    /// </summary>
    [TestClass]
    public class NonRelationalDbAuditLogServiceTests
    {
        /// <summary>
        /// Test that a fully mocked test runs without fault.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task Log_FullyMocked_RunsWithoutFault()
        {
            // Arrange
            var service = new NonRelationalDbAuditLogService(GetNonRelationalDb().Object);

            // Act
            await service.Log("ABC");
        }

        /// <summary>
        /// Tests that fully mocked test runs without fault.
        /// </summary>
        /// <param name="lastRunTime">Mock value to be returned from last run time query.</param>
        /// <param name="auditReturn">Mock value to be returned from audit query.</param>
        /// <param name="expectedValue">Expected return of testable method.</param>
        /// <returns>An awaitable task.</returns>
        [TestMethod, TestCategory("Unit")]
        [DataRow(null, null, false)]
        [DataRow("lastRun", "0", true)]
        [DataRow("lastRun", "1", false)]
        [DataRow("lastRun", "10", false)]
        public async Task NoRunningInstanceOfFeedReader_FullyMocked_RunsWithoutFault(string lastRunTime, string auditReturn, bool expectedValue)
        {
            // Arrange
            var nonRelationalDb = new Mock<INonRelationalDb>(MockBehavior.Strict);
            if (lastRunTime != null)
            {
                nonRelationalDb.Setup(s => s.GetDocumentsForSqlQuery<string>("SELECT value c.endDateTime FROM c where c.status =  'Successful' and c.action ='Import' order by c.endDateTime desc OFFSET 0 LIMIT 1")).ReturnsAsync(new List<string> { lastRunTime });
            }
            else
            {
                nonRelationalDb.Setup(s => s.GetDocumentsForSqlQuery<string>("SELECT value c.endDateTime FROM c where c.status =  'Successful' and c.action ='Import' order by c.endDateTime desc OFFSET 0 LIMIT 1")).ReturnsAsync(new List<string>());
            }

            nonRelationalDb.Setup(s => s.GetDocumentsForSqlQuery<string>($"SELECT value count(1) FROM c WHERE c.status =  'Started' AND c.action ='Import' AND c.startDateTime > '{lastRunTime}'")).ReturnsAsync(new List<string> { auditReturn });

            var service = new NonRelationalDbAuditLogService(nonRelationalDb.Object);

            // Act
            var result = await service.CheckNoRunningInstanceOfFeedReader();

            // Assert
            result.Should().Be(expectedValue);
        }

        /// <summary>
        /// Test that a fully mocked test runs without fault.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task GetLastSuccessfulRunTime_FullyMocked_RunsWithoutFault()
        {
            // Arrange
            var nonRelationalDb = new Mock<INonRelationalDb>(MockBehavior.Strict);
            nonRelationalDb.Setup(s => s.GetDocumentsForSqlQuery<string>(It.IsAny<string>())).ReturnsAsync(new List<string> { "2030-12-12" });

            var service = new NonRelationalDbAuditLogService(nonRelationalDb.Object);

            // Act
            var result = await service.GetLastSuccessfulRunTime();

            // Assert
            result.Should().BeEquivalentTo("2030-12-12");
        }

        private Mock<INonRelationalDb> GetNonRelationalDb()
        {
            var service = new Mock<INonRelationalDb>(MockBehavior.Strict);
            service.Setup(s => s.SaveDocument(It.IsAny<Dictionary<string, string>>())).Returns(Task.FromResult<object>(null));

            return service;
        }
    }
}