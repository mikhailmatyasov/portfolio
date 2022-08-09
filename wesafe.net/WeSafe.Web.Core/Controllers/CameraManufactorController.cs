using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;

namespace WeSafe.Web.Core.Controllers
{
    /// <summary>
    /// Represents Camera manufactor operations.
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    public class CameraManufactorController : Controller
    {
        private readonly ICameraManufactorService _cameraManufactorService;

        public CameraManufactorController(ICameraManufactorService cameraManufactorService)
        {
            _cameraManufactorService = cameraManufactorService;
        }

        /// <summary>
        /// Gets camera manufactors.
        /// </summary>
        /// <returns>The camera manufactor collection.</returns>
        [HttpGet]
        public IEnumerable<CameraManufactorModel> GetCameraManufactors()
        {
            return _cameraManufactorService.GetCameraManufactors();
        }
    }
}
