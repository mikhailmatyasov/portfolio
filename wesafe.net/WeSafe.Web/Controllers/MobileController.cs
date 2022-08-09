using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Shared;
using WeSafe.Shared.Roles;
using WeSafe.Web.Core.Authentication;
using WeSafe.Web.Models;

namespace WeSafe.Web.Controllers
{
    /// <summary>
    /// Represents Mobile operations.
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/[controller]")]
    public class MobileController : ControllerBase
    {
        private readonly IMobileService _mobileService;
        private readonly IAuthTokenGenerator _authToken;

        public MobileController(IMobileService mobileService, IAuthTokenGenerator generator)
        {
            _mobileService = mobileService;
            _authToken = generator;
        }

        /// <summary>
        /// Signs user from mobile device.
        /// </summary>
        /// <param name="model">The mobile authorization model.</param>
        /// <returns>The created token response.</returns>
        [HttpPost("signin")]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn([FromBody] MobileSignInModel model)
        {
            if (model == null) return BadRequest(ModelState);

            var user = await _mobileService.SignIn(model);

            if (user == null || !user.IsActive) return Unauthorized();

            var response = CreateTokenResponse(user);

            return Ok(response);
        }

        /// <summary>
        /// Updates Firebase token.
        /// </summary>
        /// <param name="model">The update firebase token model.</param>
        /// <returns>The action result.</returns>
        [HttpPost("firebase")]
        [Authorize]
        public async Task<IActionResult> UpdateFirebase([FromBody] FirebaseTokenModel model)
        {
            if (model == null) return BadRequest(ModelState);

            await _mobileService.UpdateFirebaseToken(User.Identity.Name, model);

            return Ok();
        }

        /// <summary>
        /// Gets user's filtered events.
        /// </summary>
        /// <param name="skip">The number of skipped events for pagination.</param>
        /// <param name="take">The number of taken events for pagination.</param>
        /// <param name="sort">The sorted field indicator for filtering events.</param>
        /// <param name="deviceId">The device identifier for filtering events.</param>
        /// <param name="cameraId">The camera identifier for filtering events.</param>
        /// <param name="fromDate">The date from which events should be selected.</param>
        /// <param name="toDate">The date to which events should be selected.</param>
        /// <returns>The user's event collection.</returns>
        [HttpGet("events")]
        [Authorize]
        public async Task<IActionResult> GetEvents(int? skip = null, int? take = null, string sort = null,
            int? deviceId = null, int? cameraId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var mobileId = User.Identity.Name;

            if (fromDate != null) fromDate = DateTime.SpecifyKind(fromDate.Value, DateTimeKind.Utc);
            if (toDate != null) toDate = DateTime.SpecifyKind(toDate.Value, DateTimeKind.Utc);

            return Ok(await _mobileService.GetEvents(new EventSearchRequest
            {
                MobileId = mobileId,
                Skip = skip,
                Take = take ?? 50,
                SortBy = PageRequest.ParseSort(sort),
                FromDate = fromDate,
                ToDate = toDate
            }));
        }

        /// <summary>
        /// Gets user's filtered events.
        /// </summary>
        /// <param name="filter">The filtering query.</param>
        /// <returns>The user's event collection.</returns>
        [HttpGet("search-events")]
        [Authorize]
        public async Task<IActionResult> SearchEvents([FromQuery] MobileEventFilter filter)
        {
            var searchRequest = new EventSearchRequest
            {
                Skip = filter.Skip,
                Take = filter.Take,
                FromDate = filter.FromDate,
                ToDate = filter.ToDate,
                DeviceIds = filter.DeviceIds,
                CameraIds = filter.CameraIds,
                MobileId = User.Identity.Name
            };

            return Ok(await _mobileService.GetEvents(searchRequest));
        }

        /// <summary>
        /// Gets the event by identifier.
        /// </summary>
        /// <param name="eventId">The event identifier.</param>
        /// <returns>The event.</returns>
        [HttpGet("events/{eventId}")]
        [Authorize]
        public async Task<IActionResult> GetEvent(int eventId)
        {
            var mobileId = User.Identity.Name;

            return Ok(await _mobileService.GetEvent(mobileId, eventId));
        }

        /// <summary>
        /// Gets the system status.
        /// </summary>
        /// <returns>The system status.</returns>
        [HttpGet("systemstatus")]
        [Authorize]
        public async Task<IActionResult> GetSystemStatus()
        {
            var mobileId = User.Identity.Name;
            var result = await _mobileService.GetSystemSettings(mobileId);

            return Ok(result);
        }

        /// <summary>
        /// Sets the flag that indicates if device is armed.
        /// </summary>
        /// <param name="model">The device arm model.</param>
        /// <returns>The action result.</returns>
        [HttpPost("devicearm")]
        [Authorize]
        public async Task<IActionResult> DeviceArm([FromBody] DeviceArmModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var mobileId = User.Identity.Name;

            await _mobileService.DeviceArm(mobileId, model);

            return Ok();
        }

        /// <summary>
        /// Mutes the system.
        /// </summary>
        /// <param name="model">The mute model.</param>
        /// <returns>The action result.</returns>
        [HttpPost("mute")]
        [Authorize]
        public async Task<IActionResult> MuteSystem([FromBody] MobileMuteModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var mobileId = User.Identity.Name;

            await _mobileService.Mute(mobileId, model);

            return Ok();
        }

        /// <summary>
        /// Mutes the camera.
        /// </summary>
        /// <param name="model">The camera settings with mute model.</param>
        /// <returns>The action result.</returns>
        [HttpPost("mutecamera")]
        [Authorize]
        public async Task<IActionResult> SaveCameraSettings(SettingsModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var mobileId = User.Identity.Name;

            await _mobileService.SaveCameraSettings(mobileId, model);

            return Ok();
        }

        private TokenResponse CreateTokenResponse(MobileUserModel user)
        {
            var expiresAt = DateTime.UtcNow.Add(AuthOptions.LifetimeMobile);

            return _authToken.CreateToken(new TokenRequest(user.Phone, user.Phone, expiresAt)
            {
                Role = UserRoles.Users
            });
        }
    }
}