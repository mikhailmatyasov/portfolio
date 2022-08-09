using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;

namespace WeSafe.Web.Core.Controllers
{
    /// <summary>
    /// Represents Rtsp paths operations.
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    public class RtspPathController : Controller
    {
        private readonly IRtspPathService _rtspPathService;

        public RtspPathController(IRtspPathService rtspPathService)
        {
            _rtspPathService = rtspPathService;
        }

        /// <summary>
        /// Gets camera mark's rtsp paths.
        /// </summary>
        /// <param name="cameraMarkId">The camera mark identifier.</param>
        /// <returns>The rtsp path collection.</returns>
        [HttpGet("{cameraMarkId}")]
        public IEnumerable<RtspPathModel> GetRtspPaths(int cameraMarkId)
        {
            return _rtspPathService.GetRtspPaths(cameraMarkId);
        }   
    }
}
