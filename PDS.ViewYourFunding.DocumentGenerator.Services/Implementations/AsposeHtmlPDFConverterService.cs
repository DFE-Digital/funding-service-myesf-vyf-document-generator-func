using Aspose.Cells;
using Aspose.Html;
using Aspose.Html.Drawing;
using Aspose.Pdf;
using Aspose.Pdf.Annotations;
using Aspose.Pdf.Text;
using Pds.Core.Logging;
using PDS.ViewYourFunding.DocumentGenerator.Services.Implementations.Extensions;
using PDS.ViewYourFunding.DocumentGenerator.Services.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PDS.ViewYourFunding.DocumentGenerator.Services
{
    /// <summary>
    /// Use Aspose.Html and Aspose.Pdf to generate a PDF.
    /// </summary>
    public class AsposeHtmlPDFConverterService : IPDFConverterService
    {
        private readonly ILoggerAdapter<AsposeHtmlPDFConverterService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsposeHtmlPDFConverterService"/> class.
        /// </summary>
        /// <param name="logger">The logger adapter object.</param>
        public AsposeHtmlPDFConverterService(ILoggerAdapter<AsposeHtmlPDFConverterService> logger)
        {
            _logger = logger;
            EnableLicenses();
        }

        /// <inheritdoc/>
        public byte[] CreatePdfFromHtml(
            string html,
            double? widthMM,
            double? heightMM,
            double? topMarginMM,
            double? rightMarginMM,
            double? bottomMarginMM,
            double? leftMarginMM,
            IEnumerable<Bookmark> bookmarks,
            string title)
        {
            byte[] pdfBytes;

            using (var streamProvider = new MemoryStreamProvider())
            {
                // Initialize an HTML document from the file
                using (var document = new HTMLDocument(html, new Url("http://localhost")))
                {
                    // Initialize PdfSaveOptions
                    var options = new Aspose.Html.Saving.PdfSaveOptions
                    {
                        PageSetup =
                        {
                            AnyPage = new Aspose.Html.Drawing.Page()
                            {
                                Margin = new Margin
                                {
                                    Top = new LengthOrAuto(Unit.FromMillimeters(topMarginMM ?? 7.62)),
                                    Right = new LengthOrAuto(Unit.FromMillimeters(rightMarginMM ?? 3.8)),
                                    Bottom = new LengthOrAuto(Unit.FromMillimeters(bottomMarginMM ?? 7.62)),
                                    Left = new LengthOrAuto(Unit.FromMillimeters(leftMarginMM ?? 3.8))
                                },
                                Size = new Size
                                {
                                    Width = Unit.FromMillimeters(widthMM ?? 210),
                                    Height = Unit.FromMillimeters(heightMM ?? 297)
                                }
                            }
                        }
                    };

                    var dtStart = DateTime.Now;
                    _logger?.LogInformation($"Aspose.Html.Converters.Converter.ConvertHTML started");

                    Aspose.Html.Converters.Converter.ConvertHTML(document, options, streamProvider);

                    var totalSeconds = (DateTime.Now - dtStart).TotalSeconds;
                    _logger?.LogInformation($"Aspose.Html.Converters.Converter.ConvertHTML ended in {totalSeconds} seconds");

                    dtStart = DateTime.Now;
                    _logger?.LogInformation($"asposeHtmlToPdfStream started");

                    using (var asposeHtmlToPdfStream = streamProvider.Streams.First())
                    {
                        asposeHtmlToPdfStream.Seek(0, SeekOrigin.Begin);

                        using (var pdfDocument = new Document(asposeHtmlToPdfStream))
                        {
                            pdfDocument.SetTitle(title);
                            AddBookmarks(pdfDocument, bookmarks);

                            using (var asposePdfStream = new MemoryStream())
                            {
                                pdfDocument.Save(asposePdfStream);

                                asposePdfStream.Seek(0, SeekOrigin.Begin);

                                totalSeconds = (DateTime.Now - dtStart).TotalSeconds;
                                _logger?.LogInformation($"asposeHtmlToPdfStream ended in {totalSeconds} seconds");
                                pdfBytes = asposePdfStream.ToArray();
                            }
                        }
                    }
                }
            }

            return pdfBytes;
        }

        /// <inheritdoc/>
        public FileComparisonDetails ComparePdfs(Stream sourceFile, Stream destinationFile)
        {
            var extractedTextSource = string.Empty;
            var extractedTextDestination = string.Empty;

            using (Document pdfDocument = new Document(sourceFile, true))
            {
                var textAbsorber = new TextAbsorber();
                pdfDocument.Pages.Accept(textAbsorber);
                extractedTextSource = textAbsorber.Text;
            }

            using (Document pdfDocument = new Document(destinationFile, true))
            {
                var textAbsorber = new TextAbsorber();
                pdfDocument.Pages.Accept(textAbsorber);
                extractedTextDestination = textAbsorber.Text;
            }

            if (string.IsNullOrEmpty(extractedTextSource) || string.IsNullOrEmpty(extractedTextDestination))
            {
                return new FileComparisonDetails
                {
                    Result = false,
                    TextMissingInOriginalFile = Constants.DocumentConstants.FileIsBlankMessage,
                    TextMissingInComparedFile = Constants.DocumentConstants.FileIsBlankMessage
                };
            }

            var result = new FileComparisonDetails();

            result.Result = extractedTextSource == extractedTextDestination;

            if (!result.Result)
            {
                var differences = extractedTextDestination.FindDifferences(extractedTextSource);
                result.TextMissingInOriginalFile = differences.Item1;
                result.TextMissingInComparedFile = differences.Item2;
            }

            return result;
        }

        /// <inheritdoc/>
        public byte[] CreateComparisonResultDocument(List<ComparisonResult> fileContent)
        {
            var workbook = new Workbook();

            var sheet = workbook.Worksheets[0];

            var importOptions = new ImportTableOptions();
            importOptions.InsertRows = true;

            sheet.Cells.ImportCustomObjects(fileContent, 1, 1, importOptions);

            MemoryStream asposeExcelStream = new MemoryStream();
            workbook.Save(asposeExcelStream, Aspose.Cells.SaveFormat.Csv);
            asposeExcelStream.Seek(0, SeekOrigin.Begin);

            return asposeExcelStream.ToArray();
        }

        private void AddBookmarks(Document pdfDocument, IEnumerable<Bookmark> bookmarks)
        {
            if (bookmarks == null)
            {
                return;
            }

            const int INCORRECT_OFFSET = 15;

            foreach (var bookmark in bookmarks)
            {
                var textFragmentAbsorber = new TextFragmentAbsorber(bookmark.TextToFind);
                pdfDocument.Pages.Accept(textFragmentAbsorber);

                var textFragmentCollection = textFragmentAbsorber.TextFragments;

                if (!textFragmentCollection.Any())
                {
                    continue;
                }

                var textFragment = textFragmentCollection[bookmark.InstanceToMatch]; // 1 indexed

                // Create a bookmark object
                var pdfOutline = new OutlineItemCollection(pdfDocument.Outlines)
                {
                    Title = bookmark.Title,
                    Destination = ExplicitDestination.CreateDestination(
                        textFragment.Page,
                        ExplicitDestinationType.XYZ,
                        textFragment.Position.XIndent,
                        textFragment.Position.YIndent + INCORRECT_OFFSET,
                        1)
                };

                pdfDocument.Outlines.Add(pdfOutline);
            }
        }

        private void EnableLicenses()
        {
            try
            {
                using (var stream = Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("PDS.ViewYourFunding.DocumentGenerator.Services.Implementations.Resources.Aspose.Total.lic"))
                {
                    if (stream == null)
                    {
                        return;
                    }

                    var htmlLicence = new Aspose.Html.License();
                    htmlLicence.SetLicense(stream);

                    stream.Seek(0, SeekOrigin.Begin);

                    var pdfLicence = new Aspose.Pdf.License();
                    pdfLicence.SetLicense(stream);
                }

                using (var stream = Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("PDS.ViewYourFunding.DocumentGenerator.Services.Implementations.Resources.Aspose.Total.lic"))
                {
                    if (stream == null)
                    {
                        return;
                    }

                    var htmlLicence = new Aspose.Html.License();
                    htmlLicence.SetLicense(stream);

                    stream.Seek(0, SeekOrigin.Begin);

                    var excelLicense = new Aspose.Cells.License();
                    excelLicense.SetLicense(stream);
                }
            }
            catch
            {
                // Do nothing
            }
        }
    }
}