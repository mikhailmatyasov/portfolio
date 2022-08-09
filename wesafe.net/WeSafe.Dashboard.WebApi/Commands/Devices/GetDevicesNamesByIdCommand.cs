using MediatR;
using System.Collections.Generic;
using WeSafe.Dashboard.WebApi.Models;

namespace WeSafe.Dashboard.WebApi.Commands.Devices
{
    public class GetDevicesNamesByIdCommand : IRequest<IEnumerable<DeviceModel>>
    {
        public IEnumerable<int> Ids { get; set; }
    }
}
