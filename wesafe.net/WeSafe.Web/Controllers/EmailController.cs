using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;

namespace WeSafe.Web.Controllers
{
    /// <summary>
    /// Represents Email operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("RequireAdministratorsRole")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        /// <summary>
        /// Gets emails.
        /// </summary>
        /// <returns>The email collection.</returns>
        [HttpGet]
        public async Task<IActionResult> GetEmailsAsync()
        {
            var emails = await _emailService.GetEmailsAsync();

            return Ok(emails);
        }

        /// <summary>
        /// Adds email to storage.
        /// </summary>
        /// <param name="model">The email model.</param>
        /// <returns>The action result.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateEmailAsync(EmailModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (await _emailService.IsExistsAsync(model.MailAddress))
                return BadRequest(model);

            var result = await _emailService.CreateEmail(model);

            return Ok(result);
        }

        /// <summary>
        /// Deletes email from storage by id.
        /// </summary>
        /// <param name="id">The email identifier.</param>
        /// <returns>The action result.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveEmailSync([FromRoute] int id)
        {
            if (!await _emailService.IsExistsAsync(id))
                return BadRequest(id);

            var result = await _emailService.RemoveEmail(id);

            return Ok(result);
        }

        /// <summary>
        /// Changes the email flag that indicates if it should be used for server exception notifications.
        /// </summary>
        /// <param name="id">The email identifier.</param>
        /// <returns>The action result.</returns>
        [HttpPost("{id}")]
        public async Task<IActionResult> ChangeNotifyServerExceptionValue(int id)
        {
            if (!await _emailService.IsExistsAsync(id))
                return BadRequest(id);

            var result = await _emailService.ChangeNotifyServerExceptionValue(id);

            return Ok(result);
        }
    }
}