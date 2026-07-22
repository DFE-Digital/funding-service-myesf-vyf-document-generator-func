using Microsoft.Graph;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Implementations.Helpers
{
    /// <summary>
    /// An authentication provider for OAuth refres token flow.
    /// </summary>
    internal class GraphApiAuthenticationProvider : IAuthenticationProvider
    {
        private readonly Dictionary<string, string> _requestParameters;
        private readonly string _authorityUri;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphApiAuthenticationProvider"/> class.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="redirectUri">The redirect uri.</param>
        /// <param name="authorityUri">The authority uri.</param>
        public GraphApiAuthenticationProvider(string clientId, string clientSecret, string refreshToken, string redirectUri, string authorityUri)
        {
            _requestParameters = new Dictionary<string, string>
            {
                { "client_id", clientId },
                { "redirect_uri", redirectUri },
                { "client_secret", clientSecret },
                { "refresh_token", refreshToken },
                { "grant_type", "refresh_token" }
            };

            _authorityUri = authorityUri;
            _httpClient = new HttpClient();
        }

        /// <<inheritdoc/>
        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, _authorityUri)
            {
                Content = new FormUrlEncodedContent(_requestParameters)
            };

            var httpResponse = await _httpClient.SendAsync(httpRequestMessage);

            var content = await httpResponse.Content.ReadAsStringAsync();
            var responseDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

            var accessToken = (string)responseDictionary["access_token"];
            request.Headers.Add("Authorization", "Bearer " + accessToken);
        }
    }
}