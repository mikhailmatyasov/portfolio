using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.Services;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Shared.Results;

namespace WeSafe.Web.Core.Controllers
{
    /// <summary>
    /// Represents Camera operations.
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class CamerasController : ControllerBase
    {
        private readonly ICameraService _cameraService;
        private readonly IDeviceService _deviceService;
        private readonly CameraMapper _mapper;

        public CamerasController(ICameraService cameraService, IDeviceService deviceService, CameraMapper mapper)
        {
            _cameraService = cameraService;
            _deviceService = deviceService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all cameras ordered by camera name.
        /// </summary>
        /// <returns>The ordered camera collection.</returns>
        [HttpGet]
        [Authorize("RequireAdministratorsRole")]
        public async Task<IActionResult> GetCamerasAsync()
        {
            return Ok(await _cameraService.Cameras
                                          .OrderBy(c => c.CameraName)
                                          .Select(_mapper.Projection)
                                          .ToListAsync());
        }

        /// <summary>
        /// Gets camera by identifier.
        /// </summary>
        /// <param name="id">The camera identifier.</param>
        /// <returns>The camera.</returns>
        [HttpGet("{id}")]
        [Authorize("RequireAdministratorsRole")]
        public async Task<IActionResult> GetCameraByIdAsync(int id)
        {
            var camera = await _cameraService.GetByIdAsync(id);

            if (camera == null) return Ok(null);

            return Ok(_mapper.ToCameraModel(camera));
        }

        /// <summary>
        /// Adds camera to the storage.
        /// </summary>
        /// <param name="model">The camera model.</param>
        /// <returns>The action result.</returns>
        [HttpPost]
        [Authorize("RequireAdministratorsRole")]
        public async Task<IActionResult> CreateCameraAsync([FromBody] CameraModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            model.Status = null;

            var result = await _cameraService.CreateAsync(_mapper.ToCamera(model));

            if (!result.IsSuccess) throw new InvalidOperationException(result.ToString(), null);

            return Ok(new { Id = ((PayloadExecutionResult<int>)result).Payload });
        }

        /// <summary>
        /// Updates camera.
        /// </summary>
        /// <param name="model">The camera model.</param>
        /// <returns>The updated camera identifier.</returns>
        [HttpPut]
        [Authorize("RequireAdministratorsRole")]
        public async Task<IActionResult> UpdateCameraAsync([FromBody] CameraModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var camera = await _cameraService.GetByIdAsync(model.Id);

            if (camera == null) throw new InvalidOperationException("Camera not found", null);

            var result = await _cameraService.UpdateAsync(_mapper.ToCamera(camera, model));

            if (!result.IsSuccess) throw new InvalidOperationException(result.ToString(), null);

            return Ok(new { Id = model.Id });
        }

        /// <summary>
        /// Deletes camera from the storage.
        /// </summary>
        /// <param name="id">The camera identifier.</param>
        /// <returns>The action result.</returns>
        [HttpDelete("{id}")]
        [Authorize("RequireDevicesRole")]
        public async Task<IActionResult> DeleteCameraAsync(int id)
        {
            var result = await _cameraService.DeleteAsync(id);

            if (!result.IsSuccess) throw new InvalidOperationException(result.ToString(), null);

            return Ok(new { Id = id });
        }

        /// <summary>
        /// Adds camera collection to the storage.
        /// </summary>
        /// <param name="cameraList">The camera collection model.</param>
        /// <returns>The action result.</returns>
        [HttpPost("{deviceMac}/cameras")]
        [Authorize("RequireDevicesRole")]
        public async Task<IActionResult> CreateCamerasAsync([FromRoute] string deviceMac, [FromBody] IEnumerable<BaseCameraModel> cameraList)
        {
            var device = await _deviceService.GetDeviceByMAC(deviceMac);

            if (device == null)
                return BadRequest($"Couldn't get device with MAC={deviceMac}");

            if (cameraList == null || !cameraList.Any())
                throw new ArgumentNullException(nameof(cameraList));

            var result = await _cameraService.CreateRangeAsync(device.Id, cameraList);

            if (!result.IsSuccess) throw new InvalidOperationException(result.ToString(), null);

            return Ok();
        }
    }
}