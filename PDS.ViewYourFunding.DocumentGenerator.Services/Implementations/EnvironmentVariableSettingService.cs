using Microsoft.Extensions.Configuration;
using PDS.ViewYourFunding.DocumentGenerator.Services.Config;
using System;

namespace PDS.ViewYourFunding.DocumentGenerator.Services
{
    /// <summary>
    /// A settings service that looks up settings from environmental variables.
    /// </summary>
    public class EnvironmentVariableSettingService : ISettingService
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentVariableSettingService"/> class.
        /// </summary>
        public EnvironmentVariableSettingService()
        {
            _configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();
        }

        /// <inheritdoc/>
        public IndicativeConfiguration GetIndicativeConfiguration()
        {
            return _configuration.Get<IndicativeConfiguration>();
        }

        /// <inheritdoc/>
        public string GetSetting(string key)
        {
            return Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Process);
        }
    }
}