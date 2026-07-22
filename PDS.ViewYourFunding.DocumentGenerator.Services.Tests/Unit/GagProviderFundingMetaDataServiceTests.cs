using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDS.ViewYourFunding.DocumentGenerator.Services.Constants;
using PDS.ViewYourFunding.DocumentGenerator.Services.Models;
using PDS.ViewYourFunding.DocumentGenerator.Services.Strategies;
using System;
using System.Collections.Generic;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Tests.Unit
{
    /// <summary>
    /// The GagProviderFundingMetaDataServiceTests class.
    /// </summary>
    [TestClass, TestCategory("Unit")]
    public class GagProviderFundingMetaDataServiceTests
    {
        [TestMethod]
        [DynamicData(nameof(GetFundingMetaDetailsSource), DynamicDataSourceType.Method)]
        public void GetFundingMetaDetails_ExpectedResult(
            FundingDetails inputFundingDetails,
            int yearFrom,
            FundingMetaDetails expected)
        {
            // Arrange
            var service = GetGagProviderFundingMetaDataService();

            // Act
            var actual = service.GetFundingMetaDetails(inputFundingDetails, yearFrom);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        [DataRow("psg", true, false)]
        [DataRow("gag", true, true)]
        public void AppliesTo_ExpectedResult(string fundingStreamCode, bool isProviderFunding, bool expected)
        {
            // Arrange
            var service = GetGagProviderFundingMetaDataService();

            // Act
            var actual = service.AppliesTo(fundingStreamCode, isProviderFunding);

            // Assert
            actual.Should().Be(expected);
        }

        [TestMethod]
        public void IsProviderFunding_ExpectedResult()
        {
            // Arrange
            var service = GetGagProviderFundingMetaDataService();

            // Act
            var actual = service.IsProviderFunding;

            // Assert
            actual.Should().Be(true);
        }

        private static GagProviderFundingMetaDataService GetGagProviderFundingMetaDataService()
        {
            return new GagProviderFundingMetaDataService();
        }

        private static IEnumerable<object[]> GetFundingMetaDetailsSource() =>
            new List<object[]>
            {
                new object[]
                {
                    new FundingDetails
                    {
                        SchemaVersion = "1.2",
                        FundingValue = ValidSchema1Point2Data.Replace("{openDays}", "365"),
                        DateOpenedRaw = new DateTime(2018, 4, 1).ToString(),
                        OriginalProviderType = FundingMetaDataGagConstants.FreeSchoolProviderType
                    },
                    2020,
                    new FundingMetaDetails
                    {
                        IsAprilToAugustOpener = true,
                        IsFreeSchool = true
                    }
                },
                new object[]
                {
                    new FundingDetails
                    {
                        SchemaVersion = "1.2",
                        FundingValue = ValidSchema1Point2Data.Replace("{openDays}", "365"),
                        DateOpenedRaw = new DateTime(2018, 4, 1).ToString(),
                        OriginalProviderType = FundingMetaDataGagConstants.AcademyProviderType
                    },
                    2020,
                    new FundingMetaDetails
                    {
                        IsAprilToAugustOpener = true,
                        IsAcademy = true
                    }
                },
                new object[]
                {
                    new FundingDetails
                    {
                        SchemaVersion = "1.2",
                        FundingValue = ValidSchema1Point2Data.Replace("{openDays}", "300"),
                        DateOpenedRaw = new DateTime(2020, 10, 1).ToString(),
                        OriginalProviderType = FundingMetaDataGagConstants.AcademyProviderType
                    },
                    2020,
                    new FundingMetaDetails
                    {
                        IsSeptemberToMarchOpener = true,
                        IsAcademy = true,
                        IsInYearOpener = true
                    }
                },
                new object[]
                {
                    new FundingDetails
                    {
                        SchemaVersion = "1.2",
                        FundingValue = ValidSchema1Point2Data.Replace("{openDays}", "365"),
                        DateOpenedRaw = new DateTime(2020, 5, 1).ToString(),
                        OriginalProviderType = FundingMetaDataGagConstants.AcademyProviderType
                    },
                    2020,
                    new FundingMetaDetails
                    {
                        IsAprilToAugustOpener = true,
                        IsAcademy = true,
                        IsInYearOpener = true,
                        IsSecondYearInYearOpener = true
                    }
                }
            };


        private const string ValidSchema1Point2Data =
            @"{
	""calculations"": [
		{
			""name"": ""Full year days"",
			""fundingLineCode"": null,
			""value"": 365,
			""templateCalculationId"": 733,
			""type"": ""Information"",
			""distributionPeriods"": null
		},
		{
			""name"": ""Open days"",
			""fundingLineCode"": null,
			""value"": {openDays},
			""templateCalculationId"": 567,
			""type"": ""Information"",
			""distributionPeriods"": null
		}]}";
    }
}