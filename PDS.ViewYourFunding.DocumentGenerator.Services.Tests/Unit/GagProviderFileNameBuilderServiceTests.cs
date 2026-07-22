using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PDS.ViewYourFunding.DocumentGenerator.Services.Models;
using PDS.ViewYourFunding.DocumentGenerator.Services.Strategies;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Tests.Unit
{
    /// <summary>
    /// The GagProviderFileNameBuilderServiceTests class.
    /// </summary>
    [TestClass, TestCategory("Unit")]
    public class GagProviderFileNameBuilderServiceTests
    {
        private readonly Mock<IPopulateFundingMetaData> _mockPopulateFundingMetaData = new Mock<IPopulateFundingMetaData>(MockBehavior.Strict);

        [TestMethod]
        [DataRow(true,  true, false, false, true, "12345678_10055_202122.pdf")]
        [DataRow(true,  true, false, true, true, "12345678_10054_202122.pdf")]
        [DataRow(true,  true, false, true, false, "12345678_10054_202122.pdf")]
        [DataRow(false,  false, true, true, true, "12345678_10054_202122.pdf")]
        [DataRow(false,  false, true, false, true, "12345678_10008_202122.pdf")]
        [DataRow(false,  false, true, false, false, "12345678_10008_202122.pdf")]
        [DataRow(false, false, true, true, false, "12345678_10056_202122.pdf")]
        [DataRow(true, false, false, true, false, "12345678_10057_202122.pdf")]
        [DataRow(true, true, false, false, false, "12345678_10055_202122.pdf")]
        public void BuildFileName_ExpectedResult(
            bool inYearOpener,
            bool isAprilToAugust,
            bool isSeptemberMarch,
            bool indicative,
            bool isAcademy,
            string expected)
        {
            // Arrange
            var service = GetGagProviderFileNameBuilderService();

            var fundingMetaDetails = new FundingMetaDetails
            {
                IsInYearOpener = inYearOpener,
                IsAprilToAugustOpener = isAprilToAugust,
                IsSeptemberToMarchOpener = isSeptemberMarch,
                Indicative = indicative,
                IsAcademy = isAcademy,
                IsFreeSchool = !isAcademy
            };

            _mockPopulateFundingMetaData
                .Setup(populater => populater.GetFundingMetaDetails(It.IsAny<FundingDetails>(), It.IsAny<int>()))
                .Returns(fundingMetaDetails);

            var fundingDetails = new FundingDetails { Ukprn = "12345678" };
            const string tokenisedFileName = "UKPRN_CODE_202122";
            const int yearFrom = 2020;

            // Act
            var actual = service.BuildFileName(fundingDetails, tokenisedFileName, yearFrom);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        [DataRow("psg", true, false)]
        [DataRow("gag", true, true)]
        public void AppliesTo_ExpectedResult(string fundingStreamCode, bool isProviderFunding, bool expected)
        {
            // Arrange
            var service = GetGagProviderFileNameBuilderService();

            // Act
            var actual = service.AppliesTo(fundingStreamCode, isProviderFunding);

            // Assert
            actual.Should().Be(expected);
        }

        [TestMethod]
        public void IsProviderFunding_ExpectedResult()
        {
            // Arrange
            var service = GetGagProviderFileNameBuilderService();

            // Act
            var actual = service.IsProviderFunding;

            // Assert
            actual.Should().Be(true);
        }

        private GagProviderFileNameBuilderService GetGagProviderFileNameBuilderService()
        {
            return new GagProviderFileNameBuilderService(_mockPopulateFundingMetaData.Object);
        }
    }
}