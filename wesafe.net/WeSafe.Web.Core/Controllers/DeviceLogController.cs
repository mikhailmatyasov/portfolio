using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.Services;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Shared;
using WeSafe.Shared.Enumerations;
using WeSafe.Web.Core.Mappers;
using WeSafe.Web.Core.Models;
using DeviceLogPresentationMapper = WeSafe.Web.Core.Mappers.DeviceLogPresentationMapper;

namespace WeSafe.Web.Core.Controllers
{
    /// <summary>
    /// Represents device logs operations.
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class DeviceLogController : Controller
    {
        private readonly IDeviceLogService _deviceLogService;
        private readonly DeviceLogPresentationMapper _deviceLogPresentationMapper;
        private readonly ClientDeviceLogPresentationMapper _clientDeviceLogPresentationMapper;
        private readonly DeviceLogFilterMapper _deviceLogFilterMapper;
        private readonly CreateDeviceLogsService _createDeviceLogsService;
        private readonly DownloadLimitsOptions _options;

        public DeviceLogController(IDeviceLogService deviceLogService, DeviceLogPresentationMapper deviceLogPresentationMapper,
            ClientDeviceLogPresentationMapper clientDeviceLogPresentationMapper, DeviceLogFilterMapper deviceLogFilterMapper,
            CreateDeviceLogsService createDeviceLogsService, IOptionsSnapshot<DownloadLimitsOptions> options)
        {
            _deviceLogService = deviceLogService;
            _deviceLogPresentationMapper = deviceLogPresentationMapper;
            _clientDeviceLogPresentationMapper = clientDeviceLogPresentationMapper;
            _deviceLogFilterMapper = deviceLogFilterMapper;
            _createDeviceLogsService = createDeviceLogsService;
            _options = options.Value;
        }

        /// <summary>
        /// Gets device logs.
        /// </summary>
        /// <param name="deviceLogFilter">The device lof filtered.</param>
        /// <returns>The filtered device log collection.</returns>
        [Authorize("RequireAdministratorsRole")]
        [HttpGet]
        public async Task<IActionResult> GetDevicesLogsAsync([FromQuery] DeviceLogPaginationFilter deviceLogFilter)
        {
            var recordQuery = GetClientDeviceLogRecordQuery(deviceLogFilter);

            var result = await _deviceLogService.GetClientDeviceLogsAsync(recordQuery);

            var deviceLogsResponse = new PageResponse<ClientDeviceLogPresentationModel>
            {
                Items = result.Items.Select(_clientDeviceLogPresentationMapper.ToClientDeviceLogPresentationModel).ToList(),
                Total = result.Total
            };

            return Ok(deviceLogsResponse);
        }

        /// <summary>
        /// Adds device logs to the storage.
        /// </summary>
        /// <param name="deviceLogModels">The device log collection.</param>
        [HttpPost]
        [Authorize("RequireDevicesRole")]
        public void CreateDevicesLogs([FromBody] IEnumerable<DeviceLogPresentationModel> deviceLogModels)
        {
            _createDeviceLogsService.AddLogsToTheList(deviceLogModels.Select(_deviceLogPresentationMapper.ToDeviceLogModel));
        }

        /// <summary>
        /// Inserts kept device logs to the storage. 
        /// </summary>
        [Authorize("RequireAdministratorsRole")]
        [HttpPost("insertLogs")]
        public void InsertLastLogs()
        {
            _createDeviceLogsService.InsertLastLogs();
        }

        /// <summary>
        /// Gets log levels.
        /// </summary>
        /// <returns>The log levels.</returns>
        [Authorize("RequireAdministratorsRole")]
        [HttpGet("logLevels")]
        public IActionResult GetDevicesLogLevels()
        {
            return Ok(Enum.GetValues(typeof(LogLevel))
                .Cast<LogLevel>()
                .ToDictionary(t => (int)t, t => t.ToString()));
        }

        /// <summary>
        /// Download file with device logs.
        /// </summary>
        /// <param name="deviceLogFilter">The device logs filter.</param>
        /// <returns>The file with device logs.</returns>
        [Authorize("RequireAdministratorsRole")]
        [HttpGet("downloadLogs")]
        public async Task<IActionResult> DownloadDevicesLogsFileAsync([FromQuery] DeviceLogFilter deviceLogFilter)
        {
            var recordQuery = GetClientDeviceLogRecordQuery(deviceLogFilter, true);
            var deviceLogPageResponse = await _deviceLogService.GetClientDeviceLogsAsync(recordQuery);
            var stream = GetDeviceLogMemoryStream(deviceLogPageResponse);

            return File(stream, "application/octet-stream", "DeviceLogs.txt");
        }

        private ClientDeviceLogRecordQuery GetClientDeviceLogRecordQuery(IDeviceLogFilter deviceLogFilter, bool isLimitRecordsNumber = false)
        {
            var recordQuery = new ClientDeviceLogRecordQuery();

            if (deviceLogFilter != null)
                recordQuery = _deviceLogFilterMapper.ToClientDeviceLogRecordQuery(deviceLogFilter);

            if (isLimitRecordsNumber && _options.DeviceLogsLoadLimit != 0)
                recordQuery.Take = _options.DeviceLogsLoadLimit;

            return recordQuery;
        }

        private MemoryStream GetDeviceLogMemoryStream(PageResponse<ClientDeviceLogModel> deviceLogPageResponse)
        {
            var serializer = new JsonSerializer();
            var stream = new MemoryStream();
            var jsonTextWriter = new JsonTextWriter(new StreamWriter(stream));
            var clientDeviceLogPresentationModels = deviceLogPageResponse.Items.Select(_clientDeviceLogPresentationMapper.ToClientDeviceLogPresentationModel).ToList();
            serializer.Serialize(jsonTextWriter, clientDeviceLogPresentationModels);
            jsonTextWriter.Flush();
            stream.Position = 0;

            return stream;
        }
    }
}