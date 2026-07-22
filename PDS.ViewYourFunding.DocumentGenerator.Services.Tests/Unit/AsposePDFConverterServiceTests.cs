using Aspose.Pdf;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Core.Logging;
using PDS.ViewYourFunding.DocumentGenerator.Services.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Tests
{
    /// <summary>
    /// Tests for the aspose pdf converter service.
    /// </summary>
    [TestClass]
    public class AsposePDFConverterServiceTests
    {
        /// <summary>
        /// Test that a fully mocked test runs without fault.
        /// </summary>
        [TestMethod]
        [TestCategory("Unit")]
        public void CreatePdfFromHtml_FullyMocked_RunsWithoutFault()
        {
            // Arrange
            var service = new AsposeHtmlPDFConverterService(GetMockLoggerAdapter().Object);

            // Act
            var actual = service.CreatePdfFromHtml(
                "<html><body>A</body></html>",
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                "TITLE");

            // Assert
            actual.Should().NotBeNull();
            actual.Should().NotBeEmpty();

            using var stream = new MemoryStream(actual);
            using var pdf = new Document(stream);

            pdf.Pages.Count.Should().Be(1);
            pdf.Info.Title.Should().Be("TITLE");
        }

        /// <summary>
        /// Test that a fully mocked test with bookmarks runs without fault.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void CreatePdfFromHtml_FullyMockedWithBookmarks_CreatesPdfWithBookmarks()
        {
            // Arrange
            var service = new AsposeHtmlPDFConverterService(GetMockLoggerAdapter().Object);
            var bookmarks = new List<Bookmark>
    {
        new Bookmark
        {
            Title = "A",
            InstanceToMatch = 1,
            TextToFind = "A"
        }
    };

            // Act
            var actual = service.CreatePdfFromHtml(
                "<html><body>A</body></html>",
                null, null, null, null, null, null,
                bookmarks,
                "TITLE");

            // Assert
            actual.Should().NotBeNull();
            actual.Should().NotBeEmpty();

            using var stream = new MemoryStream(actual);
            using var pdf = new Aspose.Pdf.Document(stream);

            pdf.Info.Title.Should().Be("TITLE");
            pdf.Outlines.Count.Should().BeGreaterThan(0);
        }

        /// <summary>
        /// Test same stream data returns true.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void ComparePdfs_WhenSameData()
        {
            // Arrange
            var service = new AsposeHtmlPDFConverterService(GetMockLoggerAdapter().Object);
            var streamOne = GetPdfAsStream("This is data 1");
            var streamTwo = GetPdfAsStream("This is data 1");
            var expected = new FileComparisonDetails { Result = true, TextMissingInOriginalFile = null, TextMissingInComparedFile = null };

            // Act
            var actual = service.ComparePdfs(streamOne, streamTwo);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        /// <summary>
        /// Test different stream data with same length returns false.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void ComparePdfs_WhenDifferentData()
        {
            // Arrange
            var service = new AsposeHtmlPDFConverterService(GetMockLoggerAdapter().Object);
            var streamOne = GetPdfAsStream("This is data 1.Some not needed text there.");
            var streamTwo = GetPdfAsStream("This is not data 1. Some extra text here.");
            var expected = new FileComparisonDetails { Result = false, TextMissingInOriginalFile = "1.|Some|extra|here.", TextMissingInComparedFile = "1.Some|needed|there." };

            // Act
            var actual = service.ComparePdfs(streamOne, streamTwo);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        /// <summary>
        /// Test different stream data with different length returns false.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void ComparePdfs_WhenAnyBlankFile()
        {
            // Arrange
            var service = new AsposeHtmlPDFConverterService(GetMockLoggerAdapter().Object);
            var streamOne = GetPdfAsStream(string.Empty);
            var streamTwo = GetPdfAsStream("This is not data 1. Some extra text here.");
            var expected = new FileComparisonDetails { Result = false, TextMissingInOriginalFile = Constants.DocumentConstants.FileIsBlankMessage, TextMissingInComparedFile = Constants.DocumentConstants.FileIsBlankMessage };

            // Act
            var actual = service.ComparePdfs(streamOne, streamTwo);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        /// <summary>
        /// CreateComparisonResultDocument returns correct bytes.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void CreateComparisonResultDocument_Returns_CorrectByteSize()
        {
            // Arrange
            var service = new AsposeHtmlPDFConverterService(GetMockLoggerAdapter().Object);
            var expectedBytes = 143;

            var comparisonResults = new List<ComparisonResult>
            {
                new ComparisonResult { FileName = "FileName1", Result = "File Contents are same." },
                new ComparisonResult { FileName = "FileName2", Result = "File Contents are same." }
            };

            // Act
            var actual = service.CreateComparisonResultDocument(comparisonResults);

            // Assert
            actual.Should().HaveCount(expectedBytes);
        }

        private Mock<ILoggerAdapter<AsposeHtmlPDFConverterService>> GetMockLoggerAdapter()
        {
            Mock<ILoggerAdapter<AsposeHtmlPDFConverterService>> mockLogger = new Mock<ILoggerAdapter<AsposeHtmlPDFConverterService>>(MockBehavior.Strict);

            mockLogger.Setup(l => l.LogInformation(It.IsAny<string>()));
            mockLogger.Setup(l => l.LogError(It.IsAny<Exception>(), It.IsAny<string>()));

            return mockLogger;
        }

        private Stream GetPdfAsStream(string data)
        {
            Document document = new Document();

            Page page = document.Pages.Add();
            page.Paragraphs.Add(new Aspose.Pdf.Text.TextFragment(data));

            MemoryStream documentStream = new MemoryStream();
            document.Save(documentStream, SaveFormat.Pdf);
            documentStream.Flush();

            documentStream.Seek(0, SeekOrigin.Begin);
            return documentStream;
        }
    }
}