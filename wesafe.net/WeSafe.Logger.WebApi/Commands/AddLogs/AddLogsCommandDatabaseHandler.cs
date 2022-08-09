using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WeSafe.Logger.Abstraction.Services;
using WeSafe.Logger.WebApi.Services.Abstract;

namespace WeSafe.Logger.WebApi.Commands.AddLogs
{
    public class AddLogsCommandDatabaseHandler : IRequestHandler<AddLogsCommand>
    {
        private readonly IWeSafeLogStorage _weSafeLogStorage;
        private readonly IEnumerable<IDeviceLogMapper> _deviceAndCameraNamesMappers;
        private readonly IDeviceLogFilter _deviceLogFilter;

        public AddLogsCommandDatabaseHandler(IWeSafeLogStorage weSafeLogStorage, IEnumerable<IDeviceLogMapper> deviceAndCameraNamesMappers, IDeviceLogFilter deviceLogFilter)
        {
            _weSafeLogStorage = weSafeLogStorage ?? throw new ArgumentNullException(nameof(weSafeLogStorage));
            _deviceAndCameraNamesMappers = deviceAndCameraNamesMappers ?? throw new ArgumentNullException(nameof(deviceAndCameraNamesMappers));
            _deviceLogFilter = deviceLogFilter ?? throw new ArgumentNullException(nameof(deviceLogFilter));
        }

        public async Task<Unit> Handle(AddLogsCommand request, CancellationToken cancellationToken)
        {
            var mapperTasks = _deviceAndCameraNamesMappers.Select(m => m.Map(request.Logs)).ToList();

            await Task.WhenAll(mapperTasks);

            request.Logs = await _deviceLogFilter.Filter(request.Logs);

            await _weSafeLogStorage.Add(request.Logs);

            return Unit.Value;
        }
    }
}
