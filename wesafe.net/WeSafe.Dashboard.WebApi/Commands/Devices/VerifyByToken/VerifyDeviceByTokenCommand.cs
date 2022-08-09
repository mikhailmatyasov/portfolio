using MediatR;
using WeSafe.Dashboard.WebApi.Abstractions;
using WeSafe.Dashboard.WebApi.Enumerations;

namespace WeSafe.Dashboard.WebApi.Commands.Devices
{
    /// <summary>
    /// Represents a verification of a device specified by the token.
    /// </summary>
    public class VerifyDeviceByTokenCommand : IRequest<DeviceVerificationStatus>, IDeviceByToken
    {
        public string DeviceToken { get; set; }
    }
}