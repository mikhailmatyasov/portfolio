using MediatR;
using WeSafe.Bus.Contracts.Register;
using WeSafe.Shared.Enumerations;

namespace WeSafe.Dashboard.WebApi.Commands.Register
{
    public class RegisterCommand : IRequest, IRegisterContract
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string DeviceToken { get; set; }

        public DeviceType DeviceType { get; set; }
    }
}