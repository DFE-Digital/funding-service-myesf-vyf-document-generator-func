using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDS.ViewYourFunding.DocumentGenerator.Services.Helpers;
using PDS.ViewYourFunding.DocumentGenerator.Services.Models;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Tests
{
    /// <summary>
    /// The FundingPeriodHelperTests class.
    /// </summary>
    [TestClass]
    public class FundingVersionHelperTests
    {
        /// <summary>
        /// Gets the years from code valid funding period code should evaluate correct.
        /// </summary>
        /// <param name="statementVersionNumber">The channel version string array.</param>
        /// <param name="expectedStatementValue">The expected value from statment channel.</param>
        [TestMethod, TestCategory("Unit")]
        [DataRow(0, 0)]
        [DataRow(12, 12)]
        [DataRow(3, 3)]
        [DataRow(null, null)]
        public void GetStatementChannelValue_ExpectedResult(int? statementVersionNumber, int? expectedStatementValue)
        {
            ChannelVersion[] channelVersions = null;


            if (statementVersionNumber.HasValue)
            {
                channelVersions = new ChannelVersion[]
                {
                    new ChannelVersion { Type = "Payment", Value = 0 },
                    new ChannelVersion { Type = "Statement", Value = statementVersionNumber.Value },
                    new ChannelVersion { Type = "Contract", Value = 0 }
                };
            }

            // Act
            var actual = FundingVersionHelper.GetStatementChannelVersion(channelVersions);

            // Assert
            actual.Should().Be(expectedStatementValue);
        }

        /// <summary>
        /// The string helper method to check if the funding is first version.
        /// </summary>
        /// <param name="fundingID">The funding Id to check.</param>
        /// <param name="statementChannelVersion">The statement channel version of funding ID to check.</param>
        /// <param name="expectedResult">The expected result.</param>
        [DataRow("GAG-AY2021-12345678-2_0", 1, true)]
        [DataRow("GAG-AY2021-12345678-3_0", 2, false)]
        [DataRow("GAG-AY2021-12345678-1_0", 0, true)]
        [DataRow("GAG-AY2021-12345678-3_0", 0, false)]
        [DataRow("GAG-AY2021-12345678-1_0", null, true)]
        [DataRow("GAG-AY2021-12345678-3_0", null, false)]
        [DataRow("GAG-AY2021-12345678-1_0", -1, true)]
        [DataRow("GAG-AY2021-12345678-3_0", -1, false)]
        [DataRow("", 0, false)]
        [DataRow("", null, false)]
        [DataRow("", -1, false)]
        [DataRow("", 1, true)]
        [TestMethod, TestCategory("Unit")]
        public void IsFirstVersionOfFunding_ChecksVersion(string fundingID, int? statementChannelVersion, bool expectedResult)
        {
            // Act
            var result = FundingVersionHelper.IsFirstVersionOfFunding(fundingID, statementChannelVersion);

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}