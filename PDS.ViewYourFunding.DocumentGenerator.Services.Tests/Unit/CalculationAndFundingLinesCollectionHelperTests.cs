using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDS.ViewYourFunding.DocumentGenerator.Services.Helpers;
using PDS.ViewYourFunding.DocumentGenerator.Services.Models;
using System.Collections.Generic;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Tests
{
    /// <summary>
    /// The CalculationAndFundingLinesCollectionHelperTests class.
    /// </summary>
    [TestClass, TestCategory("Unit")]
    public class CalculationAndFundingLinesCollectionHelperTests
    {
        [TestMethod]
        [DynamicData(nameof(GetSchemaBasedCalculationAndFundingLinesCollectionSource), DynamicDataSourceType.Method)]
        public void GetSchemaBasedCalculationAndFundingLinesCollection_ExpectedResult(
            string input,
            string schemaVersion,
            CalculationAndFundingLinesCollection expectedResult)
        {
            // Arrange
            // Act
            var actual = input.GetSchemaBasedCalculationAndFundingLinesCollection(schemaVersion);

            // Assert
            actual.Should().BeEquivalentTo(expectedResult);
        }

        private static IEnumerable<object[]> GetSchemaBasedCalculationAndFundingLinesCollectionSource() =>
            new List<object[]>
            {
                new object[]
                {
                    ValidSchema1Point1Data,
                    "1.1",
                    GetCalculationAndFundingLinesCollection()
                },
                new object[]
                {
                    ValidSchema1Point2Data,
                    "1.1",
                    GetCalculationAndFundingLinesCollection()
                },
                new object[]
                {
                    ValidSchema1Point1Data,
                    "1.2",
                    GetCalculationAndFundingLinesCollection()
                },
                new object[]
                {
                    ValidSchema1Point2Data,
                    "1.2",
                    GetCalculationAndFundingLinesCollection()
                },
                new object[]
                {
                    ValidSchema1Point0Data,
                    "1.0",
                    GetCalculationAndFundingLinesCollection()
                },
                new object[]
                {
                    ValidSchema1Point1Data,
                    "1.0",
                    GetCalculationAndFundingLinesCollection()
                },
                new object[]
                {
                    ValidSchema1Point1Data,
                    "9",
                    new CalculationAndFundingLinesCollection
                    {
                        Calculations = new Dictionary<int, CalculationNoNesting>(),
                        FundingLines = new Dictionary<int, FundingLineNoNesting>()
                    }
                }
            };

        private static CalculationAndFundingLinesCollection GetCalculationAndFundingLinesCollection()
        {
            return new CalculationAndFundingLinesCollection
            {
                FundingLines = new Dictionary<int, FundingLineNoNesting>
                {
                    {
                        1, new FundingLineNoNesting
                        {
                            TemplateLineId = 1,
                            Value = 10000
                        }
                    }
                },
                Calculations = new Dictionary<int, CalculationNoNesting>
                {
                    {
                        1, new CalculationNoNesting
                        {
                            TemplateCalculationId = 1,
                            Value = 10000
                        }
                    }
                }
            };
        }

        private const string ValidSchema1Point2Data =
            @"{
	    ""fundingLines"": [
		    {
			""name"": ""School Allocation Block with Notional SEN and DeDelegation"",
			""fundingLineCode"": null,
			""value"": 10000,
			""templateLineId"": 1,
			""type"": ""Information"",
			""distributionPeriods"": null
		}],
  ""calculations"": [
		    {
			""name"": ""School Allocation Block with Notional SEN and DeDelegation"",
			""fundingLineCode"": null,
			""value"": 10000,
			""templateCalculationId"": 1,
			""type"": ""Information"",
			""distributionPeriods"": null
		}]}";

        private const string ValidSchema1Point1Data =
            @"{
            ""fundingLines"": {
            ""1"": {
                ""name"": ""Total Allocation"",
                ""type"": ""Cash"",
                ""aggregationType"": ""None"",
                ""formulaText"": ""Something * something"",
                ""templateLineId"": 1,
                ""value"": 10000,
                ""valueFormat"": ""Currency""
            }},
""calculations"": {
            ""1"": {
                ""name"": ""Total Allocation"",
                ""type"": ""Cash"",
                ""aggregationType"": ""None"",
                ""formulaText"": ""Something * something"",
                ""templateCalculationId"": 1,
                ""value"": 10000,
                ""valueFormat"": ""Currency""
            }}}";

        private const string ValidSchema1Point0Data =
            @"{
	""fundingLines"": [
		{
			""name"": ""Total Allocation"",
			""fundingLineCode"": ""TotalAllocation"",
			""value"": 10000,
			""templateLineId"": 1,
			""type"": ""Payment"",
			""calculations"": [
				{
					""name"": ""School Allocation Block with Notional SEN and DeDelegation"",
					""fundingLineCode"": null,
					""value"": 10000,
					""templateCalculationId"": 1,
					""type"": ""Information"",
					""calculations"": [
						{
							""name"": ""School Allocation Block with Notional SEN and DeDelegation"",
							""fundingLineCode"": null,
							""value"": 10000,
							""templateCalculationId"": 1,
							""type"": ""Information"",
							""distributionPeriods"": null
						}
					]
				}
			],
			""fundingLines"": [
				{
					""name"": ""Total Allocation"",
					""fundingLineCode"": ""TotalAllocation"",
					""value"": 10000,
					""templateLineId"": 1,
					""type"": ""Payment"",
					""calculations"": [
						{
							""name"": ""School Allocation Block with Notional SEN and DeDelegation"",
							""fundingLineCode"": null,
							""value"": 10000,
							""templateCalculationId"": 1,
							""type"": ""Information"",
							""distributionPeriods"": null
						}
					]
				}
			]
		}
	]
}";
    }
}