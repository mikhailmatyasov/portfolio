using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;

namespace WeSafe.Web.Core.Controllers
{
    [Route("api/v{version:apiVersion}/device-metadata")]
    [Route("api/device-metadata")]
    [ApiVersion("1.0")]
    [ApiController]
    public class DeviceMetadataController : ControllerBase
    {
        private readonly IDeviceMetadataService _metadataService;

        public DeviceMetadataController(IDeviceMetadataService metadataService)
        {
            _metadataService = metadataService;
        }

        [HttpPost("{deviceId}")]
        public async Task<IActionResult> UpdateDeviceMetadata(int deviceId, [FromBody] MetadataModel model)
        {
            await _metadataService.UpdateDeviceMetadataAsync(deviceId, model);

            return Ok();
        }

        [HttpPost("{deviceId}/cameras")]
        public async Task<IActionResult> UpdateCamerasMetadata(int deviceId, [FromBody] IEnumerable<CameraMetadataModel> model)
        {
            await _metadataService.UpdateCamerasMetadataAsync(deviceId, model);

            return Ok();
        }
    }
}