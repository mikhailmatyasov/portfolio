using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.DAL.Entities;
using WeSafe.Services;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Shared;

namespace WeSafe.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DemoController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ICameraService _cameraService;
        private readonly CameraMapper _mapper;
        private readonly DemoFeatureOptions _options;
        private readonly ICameraLogService _cameraLogService;

        public DemoController(UserManager<User> userManager, ICameraService cameraService, CameraMapper mapper, IOptionsSnapshot<DemoFeatureOptions> options,
            ICameraLogService cameraLogService)
        {
            _cameraService = cameraService;
            _mapper = mapper;
            _options = options.Value;
            _userManager = userManager;
            _cameraLogService = cameraLogService;
        }

        [HttpGet("cameras")]
        public async Task<IActionResult> GetCamerasAsync()
        {
            if (!await IsDemoFeatureEnabled())
                return Forbid();

            return Ok(await _cameraService.Cameras
                                          .Where(c => c.IsActive)
                                          .OrderBy(c => c.CameraName)
                                          .Select(_mapper.Projection)
                                          .ToListAsync());
        }

        [HttpGet("events")]
        [Authorize("RequireUsersRole")]
        public async Task<IActionResult> GetEvents(int? skip = null, int? take = null, string sort = null,
            int? deviceId = null, int? cameraId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (!await IsDemoFeatureEnabled())
                return Forbid();

            if (fromDate != null)
                fromDate = DateTime.SpecifyKind(fromDate.Value, DateTimeKind.Utc);

            if (toDate != null)
                toDate = DateTime.SpecifyKind(toDate.Value, DateTimeKind.Utc);

            return Ok(await _cameraLogService.GetEvents(new EventRequest
            {
                DeviceId = deviceId,
                CameraId = cameraId,
                Skip = skip,
                Take = take ?? 50,
                SortBy = PageRequest.ParseSort(sort),
                FromDate = fromDate,
                ToDate = toDate
            }));
        }

        private async Task<bool> IsDemoFeatureEnabled()
        {
            if (!_options.Enabled)
                return false;

            var user = await _userManager.GetUserAsync(User);

            return user.Demo;
        }
    }
}
