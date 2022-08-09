using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Services.Consts;
using WeSafe.Services.Extensions;
using WeSafe.Shared.Extensions;
using WeSafe.Web.Core.Models;
using Camera = WeSafe.DAL.Entities.Camera;
using CameraSettingsModel = WeSafe.Web.Core.Models.CameraSettingsModel;

namespace WeSafe.Web.Core.Controllers
{
    /// <summary>
    /// Represents device operations.
    /// </summary>
    [Route("api/v{version:apiVersion}")]
    [Route("api")]
    [ApiVersion("1.0")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceService _deviceService;
        private readonly ICameraService _cameraService;
        private readonly ILogger<DeviceController> _logger;
        private readonly IAuthTokenGenerator _authToken;
        private readonly IMobileService _mobileService;
        private readonly IDeviceIndicatorsService _deviceIndicatorsService;
        private readonly IDetectedCameraService _detectedCameraService;
        private readonly IDeviceMetadataService _metadataService;

        public DeviceController(IDeviceService deviceService, ICameraService cameraService, ILogger<DeviceController> logger, IAuthTokenGenerator generator,
            IMobileService mobileService, IDeviceIndicatorsService deviceIndicatorsService, IDetectedCameraService detectedCameraService,
            IDeviceMetadataService metadataService)
        {
            _deviceService = deviceService;
            _cameraService = cameraService;
            _logger = logger;
            _authToken = generator;
            _mobileService = mobileService;
            _deviceIndicatorsService = deviceIndicatorsService;
            _detectedCameraService = detectedCameraService;
            _metadataService = metadataService;
        }

        /// <summary>
        /// Authenticates device.
        /// </summary>
        /// <param name="model">The device authentication model.</param>
        /// <returns>The device token response.</returns>
        [HttpPost("device/auth")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody] DeviceAuthModel model)
        {
            var authToken = await _deviceService.GetDeviceAuthToken(model.Device);

            if (authToken == null /*|| (authToken.AuthToken != null && authToken.AuthToken != model.Secret)*/ )
            {
                Thread.Sleep(3000);

                return Unauthorized();
            }

            var expiresAt = DateTime.UtcNow.Add(TimeSpan.FromDays(15));

            var auth = _authToken.CreateToken(new TokenRequest(authToken.MACAddress, authToken.Name, expiresAt)
            {
                Role = "Devices"
            });

            await _deviceService.UpdateAuthToken(authToken.DeviceId, auth.AccessToken);

            return Ok(auth);
        }

        /// <summary>
        /// Gets device settings.
        /// </summary>
        /// <param name="mac">The device mac address.</param>
        /// <returns>The device settings.</returns>
        [HttpGet("device")]
        //[Authorize("RequireDevicesRole")]
        public async Task<IActionResult> GetDeviceSettings(string mac)
        {
            // TODO: Do not move this method to microservices - hard refactor before, please.
            var device = await _deviceService.GetDeviceByMAC(mac);

            if ( device == null )
            {
                return BadRequest($"Couldn't get device with MAC={mac}");
            }

            var deviceMetadata = await _metadataService.GetDeviceMetadataAsync(device.Id);
            var cameras = await _cameraService.Cameras.Where(c => c.DeviceId == device.Id && c.IsActive).ToListAsync();
            var detectedCameras = await _detectedCameraService.GetDetectedCamerasAsync(device.Id);

            var settings = new Models.DeviceSettingsModel
            {
                Id = device.Id,
                ApiVersion = "0.1.0",
                SwVersion = device.SWVersion,
                HwVersion = device.HWVersion,
                Cameras = GetCamerasDictionary(cameras, device.TimeZone),
                DetectedCameras = detectedCameras,
                SshPassword = device.CurrentSshPassword.Decrypt(),
                LastIndicatorsTime = await _deviceIndicatorsService.GetLastIndicatorsTime(device.Id),
                Metadata = deviceMetadata
            };

            return Ok(settings);
        }

        /// <summary>
        /// Clears previous device ssh password.
        /// </summary>
        /// <param name="mac">The device mac address.</param>
        /// <returns>The action result.</returns>
        [HttpPost("passwordIsChanged/{mac}")]
        //[Authorize("RequireDevicesRole")]
        public async Task<IActionResult> ClearPreviousDeviceSshPassword([FromRoute] string mac)
        {
            var device = await _deviceService.GetDeviceByMAC(mac);

            if (device == null)
                return BadRequest($"Couldn't get device with MAC={mac}");

            var result = _deviceService.ClearPreviousDeviceSshPassword(mac);

            return Ok(result);
        }

        /// <summary>
        /// Updates device.
        /// </summary>
        /// <param name="model">The device indicators.</param>
        /// <returns>The action result.</returns>
        [HttpPost("device")]
        //[Authorize("RequireDevicesRole")]
        public async Task<IActionResult> UpdateDevice([FromForm] DeviceUpdateIndicatorsModel model)
        {
            if ( String.IsNullOrWhiteSpace(model.MacAddress) )
            {
                throw new InvalidOperationException($"{nameof(model.MacAddress)} is null.");
            }

            if ( !Regex.IsMatch(model.MacAddress, Consts.macAddressRegularString) )
            {
                throw new InvalidOperationException($"Mac address {model.MacAddress} is not valid format.");
            }

            var device = await _deviceService.GetDeviceByMAC(model.MacAddress);

            if ( device == null )
            {
                return BadRequest($"Couldn't get device with MAC={model.MacAddress}");
            }

            await Task.WhenAll(_deviceService.UpdateDeviceIpAddress(device.Id, model.IpAddress),
                _deviceIndicatorsService.UpdateDeviceIndicators(device.Id, model));

            return Ok();
        }

        /// <summary>
        /// Update device status.
        /// </summary>
        /// <param name="model">The device status model.</param>
        /// <returns>The action result.</returns>
        [HttpPost("status")]
        [Authorize("RequireDevicesRole")]
        public async Task<IActionResult> UpdateStatus([FromBody] DeviceStatusModel model)
        {
            var statUpdate = await _deviceService.UpdateDeviceNetworkStatus(model);

            if (statUpdate)
            {
                await _mobileService.SendStatusChangedNotification(model.Id);
            }

            return Ok();
        }

        /// <summary>
        /// Gets camera settings.
        /// </summary>
        /// <param name="mac">The device mac address.</param>
        /// <returns>The camera settings.</returns>
        [HttpGet("settings")]
        //[Authorize("RequireDevicesRole")]
        public async Task<IActionResult> GetCameraSettings(string mac)
        {
            var device = await _deviceService.GetDeviceByMAC(mac);

            if (device == null) return BadRequest($"Couldn't get device with MAC={mac}");

            var cameras = await _cameraService.Cameras.Where(c => c.DeviceId == device.Id && c.IsActive).ToListAsync();

            return Ok(GetCamerasDictionary(cameras, device.TimeZone));
        }

        [HttpPost("detectedcameras")]
        public async Task<IActionResult> CreateDetectedCameras([FromBody] DeviceDetectedCamerasModel model)
        {
            foreach ( var camera in model.Cameras )
            {
                await _detectedCameraService.CreateDetectedCameraAsync(model.MacAddress, camera);
            }

            return Ok();
        }

        [HttpPost("connectedcameras")]
        public async Task<IActionResult> ConnectedDetectedCameras([FromForm] ConnectDetectingCameraModel camera)
        {
            await _detectedCameraService.ConnectDetectedCameraAsync(camera);

            return Ok();
        }

        [HttpPost("failurecameras")]
        public async Task<IActionResult> FailureDetectedCameras([FromBody] IEnumerable<FailureDetectingCameraModel> cameras)
        {
            foreach ( var camera in cameras )
            {
                await _detectedCameraService.FailureDetectedCameraAsync(camera);
            }

            return Ok();
        }

        private Dictionary<string, CameraSettingsModel> GetCamerasDictionary(List<Camera> cameras, string timeZone)
        {
            return cameras.ToDictionary(c => c.Id.ToString(), c =>
            {
                object roi = null;

                if (!String.IsNullOrEmpty(c.Roi))
                {
                    try
                    {
                        roi = JsonConvert.DeserializeObject(c.Roi);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, e.Message);
                    }
                }

                RecognitionSettings recognition = new RecognitionSettings { Confidence = 90, Sensitivity = 7, AlertFrequency = 30 };

                if (!String.IsNullOrEmpty(c.RecognitionSettings))
                {
                    try
                    {
                        recognition = JsonConvert.DeserializeObject<RecognitionSettings>(c.RecognitionSettings);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, e.Message);
                    }
                }

                bool active = c.IsActive && c.IsActiveSchedule(DateTime.UtcNow, timeZone);
                string decryptedPassword = c.Password.Decrypt();
                string hexadecimalPassword = WebUtility.UrlEncode(decryptedPassword);
                string rtspWithHexadecimalPassword = c.SpecificRtcpConnectionString.Decrypt().Replace(decryptedPassword, hexadecimalPassword);
                var camera = new CameraSettingsModel
                {
                    Id = c.Id,
                    Ip = c.Ip,
                    Port = c.Port,
                    Login = c.Login,
                    Password = decryptedPassword,
                    Active = active,
                    Rtsp = rtspWithHexadecimalPassword,
                    Roi = "{\"roi_ignore\":[],\"roi_pool\":[],\"roi_perimeter\":[]}",
                    RoiV2 = roi,
                    Settings = recognition,
                    Metadata = c.Metadata
                };

                return camera;
            });
        }
    }
}
