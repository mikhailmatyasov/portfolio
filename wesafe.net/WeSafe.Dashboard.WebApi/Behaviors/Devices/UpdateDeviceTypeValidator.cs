using System;
using System.Threading;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using MediatR.Pipeline;
using WeSafe.DAL.Abstractions;
using WeSafe.DAL.Entities;
using WeSafe.DAL.Extensions;
using WeSafe.Dashboard.WebApi.Commands.Devices;

namespace WeSafe.Dashboard.WebApi.Behaviors
{
    /// <summary>
    /// Represents a <see cref="UpdateDeviceTypeCommand"/> operation validator.
    /// </summary>
    public class UpdateDeviceTypeValidator : IRequestPreProcessor<UpdateDeviceTypeCommand>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IDeviceRepository _deviceRepository;

        #endregion

        #region Constructors

        public UpdateDeviceTypeValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _deviceRepository = unitOfWork.GetCustomRepository<IDeviceRepository, Device>();
        }

        #endregion

        public async Task Process(UpdateDeviceTypeCommand request, CancellationToken cancellationToken)
        {
            var device = await _deviceRepository.FindAsync(request.DeviceId);

            if ( device == null )
            {
                throw new InvalidOperationException($"The device with '{request.DeviceId}' id is not found.");
            }
        }
    }
}