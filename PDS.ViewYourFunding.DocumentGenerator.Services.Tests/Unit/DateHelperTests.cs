using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDS.ViewYourFunding.DocumentGenerator.Services.Helpers;
using System;
using System.Collections.Generic;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Tests.Unit
{
    /// <summary>
    /// The GagProviderFileNameBuilderServiceTests class.
    /// </summary>
    [TestClass, TestCategory("Unit")]
    public class DateHelperTests
    {
        [TestMethod]
        [DataRow(1, false)]
        [DataRow(4, true)]
        public void IsAprilToAugustOpener_ExpectedResult(int input, bool expectedResult)
        {
            // Arrange
            // Act
            var actual = input.IsAprilToAugustOpener();

            // Assert
            actual.Should().Be(expectedResult);
        }

        [TestMethod]
        [DynamicData(nameof(IsSecondYearInYearOpenerSource), DynamicDataSourceType.Method)]
        public void IsSecondYearInYearOpener_ExpectedResult(DateTime input, int yearFrom,  bool expectedResult)
        {
            // Arrange
            // Act
            var actual = input.IsSecondYearInYearOpener(yearFrom);

            // Assert
            actual.Should().Be(expectedResult);
        }

        [TestMethod]
        [DataRow(10, true)]
        [DataRow(4, false)]
        public void IsSeptemberToMarchOpener_ExpectedResult(int input, bool expectedResult)
        {
            // Arrange
            // Act
            var actual = input.IsSeptemberToMarchOpener();

            // Assert
            actual.Should().Be(expectedResult);
        }

        [TestMethod]
        [DataRow("2021-09-01T00:00:00+00:00", 2021, 2022, true)]
        [DataRow("2021-09-01T00:00:00+00:00", 2022, 2023, false)]
        public void IsAcademicYearInYearOpener_ReturnsExpectedResult(string inputOpenDate, int yearFrom, int yearTo, bool expectedValue)
        {
            DateTime.TryParse(inputOpenDate, out var openDate);

            // Arrange
            var result = openDate.IsAcademicYearInYearOpener(yearFrom, yearTo);

            // Assert
            result.Should().Be(expectedValue);
        }

        private static IEnumerable<object[]> IsSecondYearInYearOpenerSource() =>
         new List<object[]>
         {
                new object[]
                {
                    new DateTime(2020, 2, 1),
                    2020,
                    false
                },
                new object[]
                {
                    new DateTime(2020, 4, 12),
                    2020,
                    true
                }
         };
    }
}