using System;
using System.Threading;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using WeSafe.DAL.Entities;
using WeSafe.Dashboard.WebApi.Commands.Devices;

namespace WeSafe.Dashboard.WebApi.Behaviors
{
    /// <summary>
    /// Represents a attach device operation validator
    /// </summary>
    public class AttachDeviceValidator : DeviceByTokenValidator<AttachDeviceCommand>
    {
        #region Constructors

        public AttachDeviceValidator(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        #endregion

        protected override Task Process(Device device, AttachDeviceCommand request, CancellationToken cancellationToken)
        {
            if ( device.ClientId != null )
            {
                throw new InvalidOperationException($"The device with '{request.DeviceToken}' already attached.");
            }

            return Task.CompletedTask;
        }
    }
}