using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDS.ViewYourFunding.DocumentGenerator.Services.Helpers;
using PDS.ViewYourFunding.DocumentGenerator.Services.Models;
using System;
using System.Collections.Generic;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Tests.Unit
{
    /// <summary>
    /// The FundingValueHelperTests class.
    /// </summary>
    [TestClass, TestCategory("Unit")]
    public class FundingValueHelperTests
    {
        [TestMethod]
        [DynamicData(nameof(ExtractCalculationValueSource), DynamicDataSourceType.Method)]
        public void ExtractCalculationValue_ExpectedResult(CalculationAndFundingLinesCollection input, int templateCalculationId, int expectedResult)
        {
            // Arrange
            // Act
            var actual = input.ExtractCalculationValue(templateCalculationId);

            // Assert
            actual.Should().Be(expectedResult);
        }

        [TestMethod]
        [DynamicData(nameof(UpdateInYearOpenerStatusSource), DynamicDataSourceType.Method)]
        public void UpdateInYearOpenerStatus_ExpectedResult(
            FundingMetaDetails input,
            FundingDetails fundingDetails,
            int yearFrom,
            CalculationAndFundingLinesCollection calculationAndFundingLinesCollection,
            FundingMetaDetails expectedResult)
        {
            // Arrange
            // Act
            input.UpdateInYearOpenerStatus(fundingDetails, yearFrom, calculationAndFundingLinesCollection);

            // Assert
            input.Should().BeEquivalentTo(expectedResult);
        }

        private static CalculationAndFundingLinesCollection GetCalculationAndFundingLinesCollection(int openDays)
        {
            return new CalculationAndFundingLinesCollection
            {
                Calculations = new Dictionary<int, CalculationNoNesting>
                {
                    {
                        733, new Calculation
                        {
                            TemplateCalculationId = 733,
                            Value = 365
                        }
                    },
                    {
                        567, new Calculation
                        {
                            TemplateCalculationId = 567,
                            Value = openDays
                        }
                    },
                    {
                        19, new Calculation
                        {
                            TemplateCalculationId = 19,
                            Value = 340
                        }
                    }
                }
            };
        }

        private static IEnumerable<object[]> UpdateInYearOpenerStatusSource() =>
            new List<object[]>
            {
                new object[]
                {
                    new FundingMetaDetails(),
                    new FundingDetails(),
                    2020,
                    GetCalculationAndFundingLinesCollection(365),
                    new FundingMetaDetails { IsSeptemberToMarchOpener = true }
                },
                new object[]
                {
                    new FundingMetaDetails(),
                    new FundingDetails { DateOpenedRaw = new DateTime(2020, 7, 1).ToString() },
                    2020,
                    GetCalculationAndFundingLinesCollection(365),
                    new FundingMetaDetails { IsInYearOpener = true, IsSecondYearInYearOpener = true, IsAprilToAugustOpener = true }
                },
                new object[]
                {
                    new FundingMetaDetails(),
                    new FundingDetails { DateOpenedRaw = new DateTime(2021, 5, 1).ToString() },
                    2020,
                    GetCalculationAndFundingLinesCollection(340),
                    new FundingMetaDetails { IsInYearOpener = true, IsAprilToAugustOpener = true }
                }
            };

        private static IEnumerable<object[]> ExtractCalculationValueSource() =>
            new List<object[]>
            {
                new object[]
                {
                    GetCalculationAndFundingLinesCollection(365),
                    19,
                    340
                },
                new object[]
                {
                    GetCalculationAndFundingLinesCollection(365),
                    15,
                    0
                }
            };
    }
}