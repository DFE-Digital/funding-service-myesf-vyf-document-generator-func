using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.DocumentGenerator.Services
{
    /// <summary>
    /// Make HTTP requests using a HttpClient.
    /// </summary>
    public class HttpService : IHttpService
    {
        private static HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpService"/> class.
        /// </summary>
        /// <param name="fundingApiSecretKey">The funding api secret key.</param>
        /// <param name="baseSiteUrl">The base URI.</param>
        public HttpService(string fundingApiSecretKey, string baseSiteUrl)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("x-secret-key", fundingApiSecretKey);
            _httpClient.BaseAddress = new Uri(baseSiteUrl);
        }

        /// <inheritdoc/>
        public async Task<string> ReadAsStringAsync(string uri)
        {
            var response = await _httpClient.GetAsync(uri);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Error occured while getting the html content for uri: {_httpClient.BaseAddress?.ToString()}{uri}");
            }

            return await response.Content.ReadAsStringAsync();
        }

        /// <inheritdoc/>
        public async Task<byte[]> ReadAsByteArrayAsync(string uri)
        {
            var response = await _httpClient.GetAsync(uri);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Error occured while getting the byte content for uri: {_httpClient.BaseAddress?.ToString()}{uri}");
            }

            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}