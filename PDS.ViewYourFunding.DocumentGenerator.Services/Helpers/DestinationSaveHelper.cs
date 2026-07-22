using PDS.ViewYourFunding.DocumentGenerator.Services.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Helpers
{
    /// <summary>
    /// Helper methods to build filepaths and names.
    /// </summary>
    public static class DestinationSaveHelper
    {
        /// <summary>
        /// Build the filename.
        /// </summary>
        /// <param name="fundingStreamCode">A funding stream code (e.g. DSG).</param>
        /// <param name="fundingPeriodCode">The funding period code.</param>
        /// <param name="id">The funding id.</param>
        /// <param name="indicative">Indicative statement.</param>
        /// <param name="statementVersionNumber">statement Version Number.</param>
        /// <param name="fileName">The fileName.</param>
        /// <param name="folderName">The folderName.</param>
        /// <returns>A file name in format 'ABC_12345678_2020-01-01.pdf'.</returns>
        public static string BuildFileNameWithPath(string fundingStreamCode, string fundingPeriodCode, string id, bool indicative, int? statementVersionNumber, string fileName, string folderName)
        {
            return $"{fundingStreamCode}\\{fundingPeriodCode}\\{folderName}\\{GetStatementFolderName(id, indicative, statementVersionNumber)}\\{fileName}";
        }

        /// <summary>
        /// Gets the folder name where the statement will be saved on basis of version.
        /// </summary>
        /// <param name="id">The funding id.</param>
        /// <param name="indicative">Indicative funding.</param>
        /// <param name="statementVersionNumber">The version from channel type statement.</param>
        /// <returns>The folder name.</returns>
        public static string GetStatementFolderName(string id, bool indicative, int? statementVersionNumber)
        {
            if (indicative)
            {
                return FolderNames.IndicativeStatements;
            }

            return FundingVersionHelper.IsFirstVersionOfFunding(id, statementVersionNumber) ? FolderNames.NewStatements : FolderNames.UpdatedStatements;
        }

        /// <summary>
        /// Gets the date folder name.
        /// </summary>
        /// <returns>The folder name where statement lands.</returns>
        public static string GetDatePathComponent()
        {
            return DateTime.Today.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// Gets the date time folder name.
        /// </summary>
        /// <returns>The folder name where statement lands.</returns>
        public static string GetDateTimePathComponent()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH-MM");
        }

        /// <summary>
        /// Build the filename.
        /// </summary>
        /// <param name="tokenisedFileName">The tokenised file name to use.</param>
        /// <param name="fundingStreamCode">A funding stream code (e.g. DSG).</param>
        /// <param name="ukprn">The UKPRN (e.g. 12345678).</param>
        /// <param name="fundingPeriod">The funding period (e.g. 2122).</param>
        /// <param name="yearFrom">The first year of academic/financial period (e.g. 2021).</param>
        /// <param name="yearTo">The second year of academic/financial period (e.g. 2022).</param>
        /// <param name="laName">The local authority name.</param>
        /// <param name="laCode">The local authority code.</param>
        /// <param name="folderName">The folder name.</param>
        /// <param name="fileType">The file type.</param>
        /// <returns>The formatted file name.</returns>
        public static string BuildFilename(string tokenisedFileName, string fundingStreamCode, string ukprn, string fundingPeriod, string yearFrom, string yearTo, string laName, string laCode, string folderName, string fileType = Constants.DocumentConstants.Pdf)
        {
            var fileName = new StringBuilder(tokenisedFileName);

            fileName.Replace("FUNDINGSTREAMCODE", fundingStreamCode);
            fileName.Replace("UKPRN", ukprn);
            fileName.Replace("FUNDINGPERIOD", fundingPeriod);
            fileName.Replace("YEARFROM", yearFrom);
            fileName.Replace("YEARTO", yearTo);
            fileName.Replace("CURRENTDATE", folderName);

            if (laName != null)
            {
                fileName.Replace("LANAME", laName);
            }

            if (laCode != null)
            {
                fileName.Replace("LACODE", laCode);
            }

            return $"{fileName}.{fileType}";
        }

        /// <summary>
        /// Build the directory path.
        /// </summary>
        /// <param name="fundingStreamCode">A funding stream code (e.g. DSG).</param>
        /// <param name="fundingPeriodCode">The academic year.</param>
        /// <param name="folderDate">The folder name.</param>
        /// <returns>A directory paths.</returns>
        public static IList<string> BuildDirectoryPath(string fundingStreamCode, string fundingPeriodCode, string folderDate)
        {
            var basePath = $"{fundingStreamCode}\\{fundingPeriodCode}\\{folderDate}\\";
            return new List<string>
            {
                $"{basePath}{FolderNames.NewStatements}\\",
                $"{basePath}{FolderNames.UpdatedStatements}\\",
                $"{basePath}{FolderNames.IndicativeStatements}\\"
            };
        }
    }
}