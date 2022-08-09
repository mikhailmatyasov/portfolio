using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Shared;

namespace WeSafe.Web.Core.Controllers
{
    /// <summary>
    /// Represents clients operations.
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize("RequireAdministratorsRole")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly IDeviceService _deviceService;

        public ClientsController(IClientService clientService, IDeviceService deviceService)
        {
            _clientService = clientService;
            _deviceService = deviceService;
        }

        /// <summary>
        /// Get filtered clients.
        /// </summary>
        /// <param name="skip">The number of skipped clients for pagination.</param>
        /// <param name="take">The number of taken clients for pagination.</param>
        /// <param name="sort">The sorted field indicator for filtering clients.</param>
        /// <param name="search">The search text.</param>
        /// <returns>The client collection.</returns>
        [HttpGet]
        public async Task<IActionResult> GetClientsAsync(int? skip = null, int? take = null, string sort = null, string search = null)
        {
            var result = await _clientService.GetClients(new PageRequest
            {
                Skip = skip,
                Take = take,
                SearchText = search,
                SortBy = PageRequest.ParseSort(sort)
            });

            return Ok(result);
        }

        /// <summary>
        /// Gets client.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <returns>The client.</returns>
        [HttpGet("{clientId}")]
        public async Task<IActionResult> GetClientAsync(int clientId)
        {
            var result = await _clientService.GetClientById(clientId);

            return Ok(result);
        }

        /// <summary>
        /// Updates client.
        /// </summary>
        /// <param name="model">The client model.</param>
        /// <returns>The action result.</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateClientAsync([FromBody] ClientModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var result = await _clientService.UpdateClient(model);

            return Ok(result);
        }

        /// <summary>
        /// Gets client devices.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <returns>The device collection.</returns>
        [HttpGet("{clientId}/devices")]
        public async Task<IActionResult> GetClientDevicesAsync(int clientId)
        {
            var result = await _deviceService.GetDevices(new DeviceRequest
            {
                ClientId = clientId
            });

            return Ok(result.Items);
        }
    }
}