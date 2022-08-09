using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Shared.Enumerations;
using WeSafe.Shared.Results;

namespace WeSafe.Web.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LicensePlateRestrictionController : ControllerBase
    {
        private readonly ILicensePlateRestrictionService _licensePlateRestrictionService;

        public LicensePlateRestrictionController(ILicensePlateRestrictionService licensePlateRestrictionService)
        {
            _licensePlateRestrictionService = licensePlateRestrictionService;
        }

        [HttpPost("{deviceId}")]
        public async Task<IActionResult> CreateLicensePlateRestrictionAsync([FromRoute] int deviceId, [FromBody] LicensePlateRestrictionModel model)
        {
            if (model == null)
                throw new InvalidOperationException(nameof(model));

            var result = await _licensePlateRestrictionService.AddLicensePlateRestrictionAsync(deviceId, model);

            return Ok(result);
        }


        [HttpGet("{deviceId}")]
        public IActionResult GetLicensePlateRestrictionAsync([FromRoute] int deviceId)
        {
            var result = _licensePlateRestrictionService.GetLicensePlateRestrictions(deviceId);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCameraAsync([FromRoute] int id)
        {
            var result = await _licensePlateRestrictionService.DeleteLicensePlateRestrictionAsync(id);

            if (!result.IsSuccess) throw new InvalidOperationException(result.ToString(), null);

            return Ok(new { Id = id });
        }

        [HttpGet("licensePlateTypes")]
        public IActionResult GetLicensePlateTypes()
        {
            return Ok(Enum.GetValues(typeof(LicensePlateType))
                .Cast<LicensePlateType>()
                .ToDictionary(t => (int)t, t => t.ToString()));
        }
    }
}