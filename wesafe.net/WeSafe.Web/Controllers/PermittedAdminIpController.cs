using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;

namespace WeSafe.Web.Controllers
{
    /// <summary>
    /// Represents Permitted Ip for admin operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("RequireAdministratorsRole")]
    public class PermittedAdminIpController : ControllerBase
    {
        private readonly IPermittedAdminIpService _permittedAdminIpService;

        public PermittedAdminIpController(IPermittedAdminIpService permittedAdminIpService)
        {
            _permittedAdminIpService = permittedAdminIpService;
        }

        /// <summary>
        /// Gets all permitted ips for admin.
        /// </summary>
        /// <returns>The permitted ip collection.</returns>
        [HttpGet]
        public IActionResult GetPermittedAdminIps()
        {
            var ips = _permittedAdminIpService.GetPermittedAdminIps();

            return Ok(ips);
        }

        /// <summary>
        /// Adds permitted ip for admin to the storage.
        /// </summary>
        /// <param name="model">The permitted ip model.</param>
        /// <returns>The action result.</returns>
        [HttpPost]
        public async Task<IActionResult> CreatePermittedAdminIpAsync(PermittedAdminIpModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var result = await _permittedAdminIpService.CreatePermittedAdminIp(model);

            return Ok(result);
        }

        /// <summary>
        /// Deletes permitted ip for admin from the storage.
        /// </summary>
        /// <param name="ipId">The ip identifier.</param>
        /// <returns>The action result.</returns>
        [HttpDelete("{ipId}")]
        public async Task<IActionResult> RemovePermittedAdminIpSync([FromRoute] int ipId)
        {
            var result = await _permittedAdminIpService.RemovePermittedAdminIp(ipId);

            return Ok(result);
        }
    }
}