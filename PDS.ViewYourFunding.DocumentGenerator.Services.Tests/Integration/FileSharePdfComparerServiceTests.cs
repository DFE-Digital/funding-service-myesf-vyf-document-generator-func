using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Core.Logging;
using PDS.ViewYourFunding.DocumentGenerator.Services.Tests.Config;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Tests
{
    /// <summary>
    /// Tests for the file share save service.
    /// </summary>
    [TestClass]
    [Ignore] // Ignored for now while we set up pipelines and config
    public class FileSharePdfComparerServiceTests
    {
        /// <summary>
        /// Test that a fully mocked test runs without fault.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [TestMethod, TestCategory("Integration"), TestCategory("CoreIntegration")]
        public async Task ComparePdfs_FullyMocked_RunsWithoutFault()
        {
            // Arrange
            var configuration = ConfigHelper.GetServiceConfiguration();
            var service = new FileSharePdfComparerService(null, configuration.FileRepoStorage_ConnectionString, "ConnectionString", configuration.FileRepoStorageName_Internal, GetMockLoggerAdapter().Object);

            // Act
            await service.ComparePdfs("GAG", "AY-2020", "12-12-2020", "15-12-2020", 10);
        }

        private Mock<LoggerAdapter<FileSharePdfComparerService>> GetMockLoggerAdapter()
        {
            return new Mock<LoggerAdapter<FileSharePdfComparerService>>(MockBehavior.Strict);
        }
    }
}