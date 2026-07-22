using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Core.Utils;
using PDS.ViewYourFunding.DocumentGenerator.Services.Implementations;
using System;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Tests.Unit
{
    /// <summary>
    /// Tests for DateTimeService.
    /// </summary>
    [TestClass]
    public class DateTimeServiceTests
    {
        /// <summary>
        /// Test GetDateTimePathComponent.
        /// </summary>
        /// <param name="hour">The hour.</param>
        /// <param name="min">The min.</param>
        /// <param name="expectedValue">The expected result.</param>
        [TestMethod, TestCategory("Unit")]
        [DataRow(13, 44, "2020-12-11 13h44m")]
        [DataRow(9, 4, "2020-12-11 09h04m")]
        [DataRow(6, 45, "2020-12-11 06h45m")]
        [DataRow(12, 12, "2020-12-11 12h12m")]
        public void GetDateTimePathComponent(int hour, int min, string expectedValue)
        {
            // Arrange
            var input = new DateTime(2020, 12, 11, hour, min, 34);

            var mockSystemProvider = new Mock<ISystemProvider>();
            mockSystemProvider.Setup(s => s.DateTime.Now()).Returns(input);
            mockSystemProvider.Setup(s => s.DateTime.ConvertToUKTime(input)).Returns(input);

            // Act
            var result = new DateTimeService(mockSystemProvider.Object).GetDateTimePathComponent();

            // Assert
            result.Should().Be(expectedValue);
        }

        /// <summary>
        /// Test GetDateTimePathComponentCustom.
        /// </summary>
        /// <param name="hour">The hour.</param>
        /// <param name="min">The min.</param>
        /// <param name="expectedValue">The expected result.</param>
        [TestMethod, TestCategory("Unit")]
        [DataRow(13, 44, "2020-12-11 13h44m")]
        [DataRow(9, 4, "2020-12-11 09h04m")]
        [DataRow(6, 45, "2020-12-11 06h45m")]
        [DataRow(12, 12, "2020-12-11 12h12m")]
        public void GetDateTimePathComponentCustom(int hour, int min, string expectedValue)
        {
            // Arrange
            var input = new DateTime(2020, 12, 11, hour, min, 34);

            var mockSystemProvider = new Mock<ISystemProvider>();
            mockSystemProvider.Setup(s => s.DateTime.Now()).Returns(input);
            mockSystemProvider.Setup(s => s.DateTime.ConvertToUKTime(input)).Returns(input);

            // Act
            var result = new DateTimeService(mockSystemProvider.Object).GetDateTimePathComponent(input);

            // Assert
            result.Should().Be(expectedValue);
        }
    }
}
