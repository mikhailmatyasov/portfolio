using System.Threading.Tasks;
using WeSafe.Services.Client.Models;

namespace WeSafe.Services.Client
{
    /// <summary>
    /// Provides an abstraction for managing global application settings.
    /// </summary>
    public interface IGlobalSettingsService
    {
        /// <summary>
        /// Gets global settings.
        /// </summary>
        /// <returns></returns>
        Task<GlobalSettingsModel> GetGlobalSettings();

        /// <summary>
        /// Updates or creates global settings
        /// </summary>
        /// <param name="model">The specified global settings</param>
        /// <returns></returns>
        Task UpdateGlobalSettings(GlobalSettingsModel model);
    }
}