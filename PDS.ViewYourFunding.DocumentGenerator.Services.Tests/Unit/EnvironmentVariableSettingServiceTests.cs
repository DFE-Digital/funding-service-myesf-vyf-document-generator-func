using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Tests
{
    /// <summary>
    /// Tests for the environmental settings service.
    /// </summary>
    [TestClass, TestCategory("Unit")]
    public class EnvironmentVariableSettingServiceTests
    {
        [TestMethod]
        public void GetSetting_FullyMocked_RunsWithoutFault()
        {
            // Arrange
            var service = new EnvironmentVariableSettingService();

            // Act
            var actual = service.GetSetting("PATH");

            // Assert
            actual.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public void GetIndicativeConfiguration_FullyMocked_RunsWithoutFault()
        {
            // Arrange
            var service = new EnvironmentVariableSettingService();

            // Act
            var actual = service.GetIndicativeConfiguration();

            // Assert
            actual.Should().NotBeNull();
        }
    }
}