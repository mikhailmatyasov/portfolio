using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;

namespace WeSafe.Nano.WebApi.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    public class GlobalSettingsController : ControllerBase
    {
        private readonly IGlobalSettingsService _globalSettingsService;

        public GlobalSettingsController(IGlobalSettingsService globalSettingsService)
        {
            _globalSettingsService = globalSettingsService;
        }

        /// <summary>
        /// Gets global settings.
        /// </summary>
        /// <returns>The global settings.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var settings = await _globalSettingsService.GetGlobalSettings();

            return Ok(settings);
        }

        /// <summary>
        /// Updates global settings.
        /// </summary>
        /// <param name="model">The specified global settings.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateAsync([FromBody] GlobalSettingsModel model)
        {
            await _globalSettingsService.UpdateGlobalSettings(model);

            return Ok();
        }
    }
}