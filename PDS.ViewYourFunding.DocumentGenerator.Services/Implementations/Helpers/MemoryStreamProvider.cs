using System.Collections.Generic;

namespace PDS.ViewYourFunding.DocumentGenerator.Services
{
    /// <summary>
    /// Memory stream provider (taken from https://github.com/aspose-html/Aspose.HTML-for-.NET).
    /// </summary>
    internal class MemoryStreamProvider : Aspose.Html.IO.ICreateStreamProvider
    {
        /// <summary>
        /// Gets list of MemoryStream objects created during the document rendering
        /// </summary>
        public List<System.IO.MemoryStream> Streams { get; } = new List<System.IO.MemoryStream>();

        /// <inheritdoc/>
        public System.IO.Stream GetStream(string name, string extension)
        {
            var result = new System.IO.MemoryStream();
            Streams.Add(result);

            return result;
        }

        /// <inheritdoc/>
        public System.IO.Stream GetStream(string name, string extension, int page)
        {
            var result = new System.IO.MemoryStream();
            Streams.Add(result);

            return result;
        }

        /// <inheritdoc/>
        public void ReleaseStream(System.IO.Stream stream)
        {
            //  Here You can release the stream filled with data and, for instance, flush it to the hard-drive
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            foreach (var stream in Streams)
            {
                stream.Dispose();
            }
        }
    }
}