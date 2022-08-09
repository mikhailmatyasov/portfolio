using System.Threading;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using WeSafe.DAL.Entities;
using WeSafe.Dashboard.WebApi.Commands.Devices;

namespace WeSafe.Dashboard.WebApi.Behaviors
{
    /// <summary>
    /// Represents a <see cref="GetDeviceByTokenCommand"/> operation validation.
    /// </summary>
    public class GetDeviceByTokenValidator : DeviceByTokenValidator<GetDeviceByTokenCommand>
    {
        #region Constructors

        public GetDeviceByTokenValidator(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        #endregion

        protected override Task Process(Device device, GetDeviceByTokenCommand request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}