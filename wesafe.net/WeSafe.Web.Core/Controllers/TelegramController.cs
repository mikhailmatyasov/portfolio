using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;

namespace WeSafe.Web.Core.Controllers
{
    /// <summary>
    /// Represents Telegram operations.
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize("RequireAdministratorsRole")]
    public class TelegramController : ControllerBase
    {
        private readonly ITelegramService _telegramService;

        public TelegramController(ITelegramService telegramService)
        {
            _telegramService = telegramService;
        }

        /// <summary>
        /// Gets the telegram user.
        /// </summary>
        /// <param name="chatId">The chat identifier.</param>
        /// <returns>The telegram user.</returns>
        [HttpGet("user/{chatId}")]
        public async Task<IActionResult> GetUser(long chatId)
        {
            var user = await _telegramService.GetTelegramUserByChatId(chatId);

            return Ok(user);
        }

        /// <summary>
        /// Registers telegram user.
        /// </summary>
        /// <param name="model">The telegram user registration model.</param>
        /// <returns>The action result.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterTelegramUserModel model)
        {
            if ( model == null ) throw new ArgumentNullException(nameof(model));
            if ( String.IsNullOrWhiteSpace(model.Phone) ) throw new ArgumentNullException(nameof(model.Phone));

            return Ok(await _telegramService.RegisterTelegramUser(model));
        }

        /// <summary>
        /// Gets devices statuses.
        /// </summary>
        /// <param name="chatId">The chat identifier.</param>
        /// <returns>The device status collection.</returns>
        [HttpGet("systemstatus/{chatId}")]
        public async Task<IActionResult> GetSystemStatus(long chatId)
        {
            var result = await _telegramService.GetSystemStatus(chatId);

            return Ok(result);
        }

        /// <summary>
        /// Gets telegram user settings.
        /// </summary>
        /// <param name="chatId">The chat identifier.</param>
        /// <returns>The telegram user settings.</returns>
        [HttpGet("settings/{chatId}")]
        public async Task<IActionResult> GetSettings(long chatId)
        {
            var result = await _telegramService.GetUserSettings(chatId);

            return Ok(result);
        }

        /// <summary>
        /// Mutes the system.
        /// </summary>
        /// <param name="model">The telegram mute model.</param>
        /// <returns>The action result.</returns>
        [HttpPost("mute")]
        public async Task<IActionResult> MuteSystem([FromBody] TelegramMuteModel model)
        {
            if ( model == null ) throw new ArgumentNullException(nameof(model));

            await _telegramService.Mute(model);

            return Ok();
        }

        /// <summary>
        /// Updates camera settings.
        /// </summary>
        /// <param name="chatId">The chat identifier.</param>
        /// <param name="model">The camera settings model.</param>
        /// <returns>The action result.</returns>
        [HttpPost("mutecamera/{chatId}")]
        public async Task<IActionResult> SaveCameraSettings(long chatId, CameraSettingsModel model)
        {
            if ( model == null ) throw new ArgumentNullException(nameof(model));

            await _telegramService.SaveCameraSettings(chatId, model);

            return Ok();
        }
    }
}