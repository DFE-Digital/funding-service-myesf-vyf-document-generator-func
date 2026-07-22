using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PDS.ViewYourFunding.DocumentGenerator.Services.Constants;
using PDS.ViewYourFunding.DocumentGenerator.Services.Helpers;
using PDS.ViewYourFunding.DocumentGenerator.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Tests
{
    /// <summary>
    /// Tests for the destination save helper.
    /// </summary>
    [TestClass, TestCategory("Unit")]
    public class DestinationSaveHelperTests
    {
        private readonly Mock<IDateTimeService> _mockDateTimeservice = new Mock<IDateTimeService>(MockBehavior.Strict);

        /// <summary>
        /// Builds file name with path for new statements using provider funding id.
        /// </summary>
        [TestMethod]
        public void BuildFileNameWithPath_ForNewStatements_Id_RunsWithoutFault()
        {
            // Arrange
            var datePartToday = DateTime.Now.ToString("yyyy-MM-dd");
            var fileName = "filename";
            var expected = $"GAG\\AY-2020\\{datePartToday}\\{FolderNames.NewStatements}\\{fileName}";

            // Act
            var actual = DestinationSaveHelper.BuildFileNameWithPath("GAG", "AY-2020", "GAG-FY-2021-10068956-1_0", false, null, fileName, datePartToday);

            // Assert
            actual.Should().Be(expected);
        }

        /// <summary>
        /// Builds file name with path for indicative statements using provider funding id.
        /// </summary>
        [TestMethod]
        public void BuildFileNameWithPath_ForIndicativeStatements_Id_RunsWithoutFault()
        {
            // Arrange
            var datePartToday = DateTime.Now.ToString("yyyy-MM-dd HH-MM");
            var fileName = "filename";
            var expected = $"GAG\\AY-2020\\{datePartToday}\\{FolderNames.IndicativeStatements}\\{fileName}";

            // Act
            var actual = DestinationSaveHelper.BuildFileNameWithPath("GAG", "AY-2020", "GAG-FY-2021-10068956-1_0", true, null, fileName, datePartToday);

            // Assert
            actual.Should().Be(expected);
        }

        /// <summary>
        /// Builds file name with path for updated statements using provider funding id.
        /// </summary>
        /// <param name="id">Provider funding id.</param>
        [TestMethod]
        [DataRow("GAG-FY-2021-10068956-2_0")]
        [DataRow("GAG-FY-2021-10068956-11_0")]
        [DataRow("GAG-FY-2021-10068956-21_0")]
        public void BuildFileNameWithPath_ForUpdatedStatements_Id_RunsWithoutFault(string id)
        {
            // Arrange
            var datePartToday = DateTime.Now.ToString("yyyy-MM-dd");
            var fileName = "filename";
            var expected = $"GAG\\AY-2020\\{datePartToday}\\{FolderNames.UpdatedStatements}\\{fileName}";

            // Act
            var actual = DestinationSaveHelper.BuildFileNameWithPath("GAG", "AY-2020", id, false, null, fileName, datePartToday);

            // Assert
            actual.Should().Be(expected);
        }

        /// <summary>
        /// Builds file name with path for new statements using channel statement version.
        /// </summary>
        /// <param name="id">funding version.</param>
        /// <param name="statementChannelVersion">Channel Versions.</param>
        [TestMethod]
        [DataRow("GAG-FY-2021-10068956-1_0", null)]
        [DataRow("GAG-FY-2021-10068956-1_0", 1)]
        public void BuildFileNameWithPath_ForNewStatements_StatementChannelVersion_RunsWithoutFault(string id, int? statementChannelVersion)
        {
            // Arrange
            var datePartToday = DateTime.Now.ToString("yyyy-MM-dd");
            var fileName = "filename";
            var expected = $"GAG\\AY-2020\\{datePartToday}\\{FolderNames.NewStatements}\\{fileName}";

            // Act
            var actual = DestinationSaveHelper.BuildFileNameWithPath("GAG", "AY-2020", id, false, statementChannelVersion, fileName, datePartToday);

            // Assert
            actual.Should().Be(expected);
        }

        /// <summary>
        /// Builds file name with path for indicative statements using channel statement version.
        /// </summary>
        [TestMethod]
        public void BuildFileNameWithPath_ForIndicativeStatements_StatementChannelVersion_RunsWithoutFault()
        {
            // Arrange
            var datePartToday = DateTime.Now.ToString("yyyy-MM-dd HH-MM");
            var fileName = "filename";
            var expected = $"GAG\\AY-2020\\{datePartToday}\\{FolderNames.IndicativeStatements}\\{fileName}";

            // Act
            var actual = DestinationSaveHelper.BuildFileNameWithPath("GAG", "AY-2020", "GAG-FY-2021-10068956-1_0", true, 1, fileName, datePartToday);

            // Assert
            actual.Should().Be(expected);
        }

        /// <summary>
        /// Builds file name with path for updated statements using channel statement version.
        /// </summary>
        /// <param name="id">Provider funding id.</param>
        /// <param name="statementChannelVersion">Channel statement version.</param>
        [TestMethod]
        [DataRow("GAG-FY-2021-10068956-2_0", null)]
        [DataRow("GAG-FY-2021-10068956-2_0", 2)]
        [DataRow("GAG-FY-2021-10068956-11_0", 11)]
        public void BuildFileNameWithPath_ForUpdatedStatements_StatementChannelVersion_RunsWithoutFault(string id, int? statementChannelVersion)
        {
            // Arrange
            var datePartToday = DateTime.Now.ToString("yyyy-MM-dd");
            var fileName = "filename";
            var expected = $"GAG\\AY-2020\\{datePartToday}\\{FolderNames.UpdatedStatements}\\{fileName}";

            // Act
            var actual = DestinationSaveHelper.BuildFileNameWithPath("GAG", "AY-2020", id, false, statementChannelVersion, fileName, datePartToday);

            // Assert
            actual.Should().Be(expected);
        }

        /// <summary>
        /// Builds folder name for new statements using provider funding id.
        /// </summary>
        [TestMethod]
        public void GetStatementFolderName_ForNewStatements_Id_RunsWithoutFault()
        {
            // Act
            var actual = DestinationSaveHelper.GetStatementFolderName("GAG-FY-2021-10068956-1_0", false, null);

            // Assert
            actual.Should().Be(FolderNames.NewStatements);
        }

        /// <summary>
        /// Builds folder name for indicative statements using provider funding id.
        /// </summary>
        [TestMethod]
        public void GetStatementFolderName_ForIndicativeStatements_Id_RunsWithoutFault()
        {
            // Act
            var actual = DestinationSaveHelper.GetStatementFolderName("GAG-FY-2021-10068956-1_0", true, null);

            // Assert
            actual.Should().Be(FolderNames.IndicativeStatements);
        }

        /// <summary>
        /// Builds folder name for updated statements using provider funding id.
        /// </summary>
        /// <param name="id">Provider funding id.</param>
        [TestMethod]
        [DataRow("GAG-FY-2021-10068956-2_0")]
        [DataRow("GAG-FY-2021-10068956-11_0")]
        [DataRow("GAG-FY-2021-10068956-21_0")]
        public void GetStatementFolderName_ForUpdatedStatements_Id_RunsWithoutFault(string id)
        {
            // Act
            var actual = DestinationSaveHelper.GetStatementFolderName(id, false, null);

            // Assert
            actual.Should().Be(FolderNames.UpdatedStatements);
        }

        /// <summary>
        /// Builds folder name for new statements using channel statement version.
        /// </summary>
        [TestMethod]
        public void GetStatementFolderName_ForNewStatements_StatementChannelVersion_RunsWithoutFault()
        {
            // Act
            var actual = DestinationSaveHelper.GetStatementFolderName("GAG-FY-2021-10068956-1_0", false, 1);

            // Assert
            actual.Should().Be(FolderNames.NewStatements);
        }

        /// <summary>
        /// Builds folder name for new statements using channel statement version.
        /// </summary>
        [TestMethod]
        public void GetStatementFolderName_ForIndicativeStatements_StatementChannelVersion_RunsWithoutFault()
        {
            // Act
            var actual = DestinationSaveHelper.GetStatementFolderName("GAG-FY-2021-10068956-1_0", true, 1);

            // Assert
            actual.Should().Be(FolderNames.IndicativeStatements);
        }

        /// <summary>
        /// Builds folder name for new statements using channel statement version.
        /// </summary>
        /// <param name="id">Provider funding id.</param>
        /// <param name="channelStatementVersion">Channel statement version.</param>
        [TestMethod]
        [DataRow("GAG-FY-2021-10068956-2_0", null)]
        [DataRow("GAG-FY-2021-10068956-2_0", 2)]
        [DataRow("GAG-FY-2021-10068956-11_0", 11)]
        public void GetStatementFolderName_ForUpdatedStatements_StatementChannelVersion_RunsWithoutFault(string id, int? channelStatementVersion)
        {
            // Act
            var actual = DestinationSaveHelper.GetStatementFolderName(id, false, channelStatementVersion);

            // Assert
            actual.Should().Be(FolderNames.UpdatedStatements);
        }

        /// <summary>
        /// Builds file name from tokenised string.
        /// </summary>
        [TestMethod]
        public void BuildFilename_ReturnsFileName()
        {
            // Arrange
            var tokenisedFileName = "FUNDINGSTREAMCODE_UKPRN_FUNDINGPERIOD_YEARFROM_YEARTO_Non tokenised part_LANAME_Another non tokenised part_LACODE";
            var datePartToday = DateTime.Now.ToString("yyyy-MM-dd HH-MM");

            // Act
            var actual = DestinationSaveHelper.BuildFilename(tokenisedFileName, "GAG", "12345678", "2122", "2021", "2022", "Camden", "Cam", datePartToday);

            // Assert
            actual.Should().Be("GAG_12345678_2122_2021_2022_Non tokenised part_Camden_Another non tokenised part_Cam.pdf");
        }

        /// <summary>
        /// Returns the paths.
        /// </summary>
        [TestMethod]
        public void BuildDirectoryPath_ReturnsPaths()
        {
            // Arrange
            var fundingStreamCode = "Funding Stream Code";
            var fundingPeriodCode = "Funding Period Code";
            var folderDate = "Folder Date";
            var expected = new List<string>
            {
                $"{fundingStreamCode}\\{fundingPeriodCode}\\{folderDate}\\{FolderNames.NewStatements}\\",
                $"{fundingStreamCode}\\{fundingPeriodCode}\\{folderDate}\\{FolderNames.UpdatedStatements}\\",
                $"{fundingStreamCode}\\{fundingPeriodCode}\\{folderDate}\\{FolderNames.IndicativeStatements}\\"
            };

            // Act
            var actual = DestinationSaveHelper.BuildDirectoryPath(fundingStreamCode, fundingPeriodCode, folderDate);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }
    }
}