using Microsoft.Extensions.Configuration;
using PDS.ViewYourFunding.DocumentGenerator.Services.Tests.Models;

namespace PDS.ViewYourFunding.DocumentGenerator.Services.Tests.Config
{
    /// <summary>
    /// The configuration helper.
    /// </summary>
    public static class ConfigHelper
    {
        /// <summary>
        /// Gets the iconfiguration root.
        /// </summary>
        /// <returns>The configuration.</returns>
        public static IConfigurationRoot GetIConfigurationRoot()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true)
                .AddEnvironmentVariables()
                .Build();
        }

        /// <summary>
        /// Gets the service configuration.
        /// </summary>
        /// <returns>The service configuration.</returns>
        public static ServiceConfiguration GetServiceConfiguration()
        {
            var config = GetIConfigurationRoot();

            var serviceConfiguration = new ServiceConfiguration();
            config.Bind(serviceConfiguration);

            return serviceConfiguration;
        }
    }
}