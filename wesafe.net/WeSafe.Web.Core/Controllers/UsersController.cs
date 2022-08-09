using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Services.Extensions;
using WeSafe.Shared;
using WeSafe.Shared.Results;
using WeSafe.Shared.Roles;

namespace WeSafe.Web.Core.Controllers
{
    /// <summary>
    /// Represents Users operations.
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Gets users.
        /// </summary>
        /// <param name="skip">The number of skipped users for pagination.</param>
        /// <param name="take">The number of taken users for pagination.</param>
        /// <param name="sort">The sorted field indicator for filtering users.</param>
        /// <returns>The user collection.</returns>
        [HttpGet]
        public async Task<IActionResult> GetUsersAsync(int? skip = null, int? take = null, string sort = null)
        {
            if (User.IsInRole(UserRoles.Users)) throw new Exception("Not allowed");

            var result = await _userService.GetUsersAsync(new PageRequest
            {
                Skip = skip,
                Take = take,
                SortBy = PageRequest.ParseSort(sort)
            });

            return Ok(result);
        }

        /// <summary>
        /// Gets user.
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <returns>The user.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserByIdAsync(string id)
        {
            if (User.IsInRole(UserRoles.Users)) throw new Exception("Not allowed");

            var user = await _userService.GetUserByIdAsync(id);

            if (user == null) return Ok(null);

            return Ok(user);
        }

        /// <summary>
        /// Adds user to the storage.
        /// </summary>
        /// <param name="model">The user model.</param>
        /// <returns>The action result.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateUserAsync([FromBody] UpsertUserModel model)
        {
            if (User.IsInRole(UserRoles.Users)) throw new Exception("Not allowed");

            if (!model.Phone.IsValidPhoneNumber())
                throw new InvalidOperationException("Phone " + model.Phone + " is not valid");

            if (model == null) throw new ArgumentNullException(nameof(model));
            if (String.IsNullOrWhiteSpace(model.Password)) throw new Exception("Password must be set");

            var result = await _userService.CreateUserAsync(model, model.Password);

            return Ok(result);
        }

        /// <summary>
        /// Updates user.
        /// </summary>
        /// <param name="model">The user model.</param>
        /// <returns>The action result.</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateUserAsync([FromBody] UpsertUserModel model)
        {
            if (User.IsInRole(UserRoles.Users)) throw new Exception("Not allowed");

            if (model == null) throw new ArgumentNullException(nameof(model));

            var result = await _userService.UpdateUserAsync(model);

            return Ok(result);
        }

        /// <summary>
        /// Deletes user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>The action result.</returns>
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUserAsync(string userId)
        {
            if (User.IsInRole(UserRoles.Users)) throw new Exception("Not allowed");

            var result = await _userService.DeleteUserAsync(userId);

            if (result is FailedExecutionResult errorResult)
                return StatusCode(422, new { Error = errorResult.Errors.FirstOrDefault() });

            return Ok();
        }

        /// <summary>
        /// Unlocks the user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>The action result.</returns>
        [HttpPost("{userId}/unlock")]
        public async Task<IActionResult> UnlockUserAsync([FromRoute] string userId)
        {
            if (User.IsInRole(UserRoles.Users))
                throw new Exception("Not allowed");

            await _userService.UnlockUser(userId);

            return Ok();
        }
    }
}