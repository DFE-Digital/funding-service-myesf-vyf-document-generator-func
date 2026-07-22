using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDS.ViewYourFunding.DocumentGenerator.Services.Helpers;
using System;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Tests
{
    /// <summary>
    /// The FundingPeriodHelperTests class.
    /// </summary>
    [TestClass]
    public class FundingPeriodHelperTests
    {
        /// <summary>
        /// Gets the years from code valid funding period code should evaluate correct.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void GetYearsFromCode_ValidFundingPeriodCode_ShouldEvaluateCorrect()
        {
            // Arrange
            var expectedYear1 = 2019;
            var expectedYear2 = 2020;

            // Act
            var actual = FundingPeriodHelper.GetYearsFromCode("FY-1920");

            // Assert
            actual.Should().BeEquivalentTo((expectedYear1, expectedYear2));
        }

        /// <summary>
        /// Gets the years from code invalid funding period code should throw error.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void GetYearsFromCode_InvalidFundingPeriodCode_ShouldThrowError()
        {
            // Act
            Action act = () => FundingPeriodHelper.GetYearsFromCode("FY-A920");

            // Assert
            act.Should().Throw<Exception>();
        }
    }
}