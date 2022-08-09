using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using WeSafe.Nano.Services.Abstraction.Abstraction.Services;
using WeSafe.Nano.Services.Abstraction.Models;


namespace WeSafe.Nano.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VideoRecordsController : ControllerBase
    {
        private readonly IVideoRecordsService _videoRecordsService;

        public VideoRecordsController(IVideoRecordsService videoRecordsService)
        {
            _videoRecordsService = videoRecordsService;
        }

        /// <summary>
        /// Gets video records paths.
        /// </summary>
        /// <returns>The video records paths.</returns>
        [HttpGet]
        public IActionResult GetVideos([FromQuery] VideoRecordSearchModel videoRecordSearchModel)
        {
            if (videoRecordSearchModel == null)
                return BadRequest($"{nameof(videoRecordSearchModel)} can not be null.");

            var videoRecordPaths = _videoRecordsService.GetVideoFilesPaths(videoRecordSearchModel);

            var absolutePaths = videoRecordPaths.Select(p =>
                $"{Request.Scheme}://{Request.Host}/{p}").ToList();

            return Ok(absolutePaths);
        }

        /// <summary>
        /// Gets event video record.
        /// </summary>
        /// <returns>The video records paths.</returns>
        [HttpGet("eventVideo")]
        public IActionResult GetEventVideo(int cameraId, DateTime eventDate)
        {
            if (cameraId <= 0)
                return BadRequest($"{nameof(cameraId)} invalid camera identifier");

            if (eventDate == default)
                return BadRequest($"{nameof(eventDate)} can not be null.");

            var videoRecordPath = _videoRecordsService.GetVideoFile(cameraId, eventDate);
            var absolutePath = $"{Request.Scheme}://{Request.Host}/{videoRecordPath}";

            return Ok(new { videoUrl = absolutePath });
        }
    }
}
