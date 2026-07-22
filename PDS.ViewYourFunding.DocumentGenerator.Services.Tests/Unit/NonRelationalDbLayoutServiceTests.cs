using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using PDS.ViewYourFunding.DocumentGenerator.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Tests
{
    /// <summary>
    /// Tests for the non-relational db layout service.
    /// </summary>
    [TestClass, TestCategory("Unit")]
    public class NonRelationalDbLayoutServiceTests
    {
        /// <summary>
        /// Test that a fully mocked test runs without fault.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [TestMethod]
        public async Task GetLayout_FullyMocked_RunsWithoutFault()
        {
            // Arrange
            var service = new NonRelationalDbLayoutService(GetSettingService().Object, GetNonRelationalDb().Object);
            var expected = new Dictionary<string, object>();

            // Act
            var actual = await service.GetLayout("ABC");

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        /// <summary>
        /// Test that a fully mocked test runs without fault.
        /// </summary>
        [TestMethod]
        public void LookupLayoutId_FullyMocked_RunsWithoutFault()
        {
            // Arrange
            var service = new NonRelationalDbLayoutService(GetSettingService().Object, GetNonRelationalDb().Object);
            var expected = new List<string> { "ABCDEFHJKLMNOP" };

            // Act
            var actual = service.LookupLayoutId("ABC", "DEF", "2030-01-01", "12345678", "12345678");

            // Assert
            actual.layoutIds.Should().BeEquivalentTo(expected);
        }

        /// <summary>
        /// Test that a fully mocked test runs without fault.
        /// </summary>
        [TestMethod]
        public void LookupFileType_FullyMocked_RunsWithoutFault()
        {
            // Arrange
            var service = new NonRelationalDbLayoutService(GetSettingService().Object, GetNonRelationalDb().Object);
            var expected = "ABCDEFHJKLMNOP";

            // Act
            var actual = service.LookupFileType("layoutid");

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        /// <summary>
        /// Returns pdf when file type not found.
        /// </summary>
        [TestMethod]
        public void LookupFileType_WhenFileTypeNotFound_ReturnPdf()
        {
            // Arrange
            var settingService = new Mock<ISettingService>(MockBehavior.Strict);
            settingService.Setup(s => s.GetSetting(It.IsAny<string>())).Returns(null as string);

            var service = new NonRelationalDbLayoutService(settingService.Object, GetNonRelationalDb().Object);

            // Act
            var actual = service.LookupFileType("layoutid");

            // Assert
            actual.Should().BeEquivalentTo("pdf");
        }

        private Mock<INonRelationalDb> GetNonRelationalDb()
        {
            var service = new Mock<INonRelationalDb>(MockBehavior.Strict);

            var document = new Dictionary<string, object>
            {
                { "Data", new JObject() }
            };

            service.Setup(s => s.GetDocumentById(It.IsAny<string>())).ReturnsAsync(document);
            service.Setup(s => s.SaveDocument(It.IsAny<Dictionary<string, string>>())).Returns(Task.FromResult<object>(null));

            return service;
        }

        private Mock<ISettingService> GetSettingService()
        {
            var service = new Mock<ISettingService>(MockBehavior.Strict);
            service.Setup(s => s.GetSetting(It.IsAny<string>())).Returns("ABCDEFHJKLMNOP");

            return service;
        }
    }
}