using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.DAL.Entities;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Shared.Results;
using WeSafe.Shared.Roles;
using WeSafe.Web.Core.Authentication;
using WeSafe.Web.Core.Models;

namespace WeSafe.Web.Core.Controllers
{
    /// <summary>
    /// Represents Account operations.
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IDeviceService _deviceService;
        private readonly IClientService _clientService;
        private readonly SignInManager<User> _signInManager;
        private readonly IAuthTokenGenerator _authToken;
        private readonly DemoFeatureOptions _options;
        private const string loginInvalidCredentialsError = "Wrong username or password.";

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager,
            IDeviceService deviceService, IClientService clientService, IAuthTokenGenerator generator, IOptionsSnapshot<DemoFeatureOptions> options)
        {
            _userManager = userManager;
            _deviceService = deviceService;
            _clientService = clientService;
            _signInManager = signInManager;
            _authToken = generator;
            _options = options.Value;
        }

        /// <summary>
        /// Authorizes user.
        /// </summary>
        /// <param name="model">Sign in model.</param>
        /// <returns>The token.</returns>
        [HttpPost("token")]
        [Authorize("ReqiredLoginThroughIp")]
        public async Task<IActionResult> TokenAsync([FromBody] LoginModel model)
        {
            if (model == null) return BadRequest(ModelState);

            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user == null || !user.IsActive || (!_options.Enabled && user.Demo))
                return Unauthorized(new { MessageError = loginInvalidCredentialsError });

            var role = (await _userManager.GetRolesAsync(user)).Single();

            bool isAdmin = (role == UserRoles.Administrators);
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, !isAdmin);

            if (result.Succeeded)
            {
                var response = await CreateTokenResponse(user, model.Origin);

                return Ok(response);
            }

            string error = result.IsLockedOut ? "User is locked. Try again in 10 minutes." : loginInvalidCredentialsError;

            return Unauthorized(new { MessageError = error });
        }

        /// <summary>
        /// Checks device token existence and availability.
        /// </summary>
        /// <param name="model">The device token model.</param>
        /// <returns>The action result.</returns>
        [HttpPost("token-status")]
        [AllowAnonymous]
        public async Task<IActionResult> FindDeviceTokenAsync([FromBody] DeviceTokenModel model)
        {
            if (model == null)
                return BadRequest(ModelState);

            var device = await _deviceService.GetDeviceByToken(model.DeviceToken);

            if (device == null)
                return NotFound();

            if (device.ClientId != null)
                return Conflict();

            return Ok(ExecutionResult.Success());
        }

        /// <summary>
        /// Checks if user name is taken. 
        /// </summary>
        /// <param name="model">The user name model.</param>
        /// <returns>The action result.</returns>
        [HttpPost("login-status")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLoginStatusAsync([FromBody] UserNameModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user != null)
                return StatusCode(422, new { Error = "User with this username already exists." });

            return Ok(ExecutionResult.Success());
        }

        /// <summary>
        /// Signs up user.
        /// </summary>
        /// <param name="model">The sign up model.</param>
        /// <returns>The action result.</returns>
        [HttpPost("signup")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUpAsync([FromBody] SignUpModel model)
        {
            if (model == null) return BadRequest(ModelState);

            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user != null)
                return StatusCode(422, new { Error = "User with this username already exists." });

            var result = await _clientService.SignUpClient(new ClientModel
            {
                Token = model.DeviceToken,
                DeviceType = model.DeviceType,
                Phone = model.Phone,
                CreatedAt = DateTimeOffset.UtcNow,
                IsActive = true,
                Name = model.Name
            }, model.TimeZone, model.Password);

            if (result is FailedExecutionResult errorResult)
                return StatusCode(422, new { Error = errorResult.Errors.FirstOrDefault() });

            user = new User
            {
                ClientId = ((PayloadExecutionResult<int>)result).Payload,
                DisplayName = model.Name,
                IsActive = true,
                PhoneNumber = model.Phone,
                UserName = model.UserName
            };

            var identityResult = await _userManager.CreateAsync(user, model.Password);

            if (!identityResult.Succeeded)
            {
                return Ok(ExecutionResult.Failed(identityResult.Errors.Select(c => c.Description)));
            }

            await _userManager.AddToRoleAsync(user, UserRoles.Users);

            var response = await CreateTokenResponse(user, null);

            return Ok(response);
        }

        /// <summary>
        /// Gets the profile of authorized user.
        /// </summary>
        /// <returns>The user profile.</returns>
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfileAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            var profile = new ProfileModel
            {
                DisplayName = user.DisplayName
            };

            return Ok(profile);
        }

        /// <summary>
        /// Updates the profile of authorized user.
        /// </summary>
        /// <param name="model">The user profile model.</param>
        /// <returns>The action result.</returns>
        [HttpPost("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfileAsync([FromBody] ProfileModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            user.DisplayName = model.DisplayName;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded) return StatusCode(500, result.Errors.Select(c => c.Description));

            if (!String.IsNullOrWhiteSpace(model.OldPassword) && !String.IsNullOrWhiteSpace(model.Password))
            {
                if (model.OldPassword == model.Password) return BadRequest("Password must not equal old password.");

                result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.Password);

                if (!result.Succeeded) return StatusCode(500, result.Errors.Select(c => c.Description));
            }

            if (user.ClientId != null)
            {
                var client = await _clientService.GetClientById(user.ClientId.Value);

                if (client == null) throw new InvalidOperationException();

                client.Name = model.DisplayName;

                return Ok(await _clientService.UpdateClient(client));
            }

            return Ok(ExecutionResult.Success());
        }

        private async Task<TokenResponse> CreateTokenResponse(User user, string origin)
        {
            var role = (await _userManager.GetRolesAsync(user)).Single();
            var expiresAt = DateTime.UtcNow.Add(origin == "desktop" ? AuthOptions.LifetimeDesktop : AuthOptions.Lifetime);

            return _authToken.CreateToken(new TokenRequest(user.Id, user.UserName, expiresAt)
            {
                Role = role,
                DisplayName = user.DisplayName,
                Demo = user.Demo
            });
        }
    }
}