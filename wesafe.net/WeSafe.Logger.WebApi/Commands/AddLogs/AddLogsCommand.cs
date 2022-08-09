using MediatR;
using System.Collections.Generic;
using WeSafe.Logger.Abstraction.Models;

namespace WeSafe.Logger.WebApi.Commands.AddLogs
{
    public class AddLogsCommand : IRequest
    {
        public IEnumerable<DeviceLogModel> Logs { get; set; }
    }
}
