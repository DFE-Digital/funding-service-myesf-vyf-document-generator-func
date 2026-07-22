using PDS.ViewYourFunding.DocumentGenerator.Services.Config;

namespace PDS.ViewYourFunding.DocumentGenerator.Services
{
    /// <summary>
    /// An interface for looking up settings.
    /// </summary>
    public interface ISettingService
    {
        /// <summary>
        /// Get a setting by key.
        /// </summary>
        /// <param name="key">The key to lookup.</param>
        /// <returns>The setting as a string.</returns>
        string GetSetting(string key);

        /// <summary>
        /// Gets the indicative configuration.
        /// </summary>
        /// <returns>The IndicativeConfiguration.</returns>
        IndicativeConfiguration GetIndicativeConfiguration();
    }
}