using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Tests
{
    /// <summary>
    /// Tests for the http service.
    /// </summary>
    [TestClass]
    public class HttpServiceTests
    {
        /// <summary>
        /// Test that a fully mocked test runs without fault.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task ReadAsStringAsync_FullyMocked_RunsWithoutFault()
        {
            // Arrange
            var service = new HttpService("123", "https://www.example.org");

            // Act
            var actual = await service.ReadAsStringAsync("/");

            // Assert
            actual.Should().NotBeNullOrEmpty();
        }
    }
}