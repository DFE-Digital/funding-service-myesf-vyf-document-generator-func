using System.Threading.Tasks;

namespace PDS.ViewYourFunding.DocumentGenerator.Services
{
    /// <summary>
    /// An interface for making http calls.
    /// </summary>
    public interface IHttpService
    {
        /// <summary>
        /// Read a string from a URI.
        /// </summary>
        /// <param name="uri">The URI (e.g. http://www.example.org/whatever ).</param>
        /// <returns>The response as a string.</returns>
        Task<string> ReadAsStringAsync(string uri);

        /// <summary>
        /// Read bytes array from the URI.
        /// </summary>
        /// <param name="uri">The URI (e.g. http://www.example.org/whatever ).</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<byte[]> ReadAsByteArrayAsync(string uri);
    }
}