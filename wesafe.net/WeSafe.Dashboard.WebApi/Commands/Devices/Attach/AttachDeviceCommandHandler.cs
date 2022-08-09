using System;
using System.Threading;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using MediatR;
using WeSafe.DAL.Abstractions;
using WeSafe.DAL.Entities;
using WeSafe.DAL.Extensions;

namespace WeSafe.Dashboard.WebApi.Commands.Devices
{
    /// <summary>
    /// Represents a attach device command handler.
    /// </summary>
    public class AttachDeviceCommandHandler : AsyncRequestHandler<AttachDeviceCommand>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IDeviceRepository _deviceRepository;

        #endregion

        #region Constructors

        public AttachDeviceCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _deviceRepository = unitOfWork.GetCustomRepository<IDeviceRepository, Device>();
        }

        #endregion

        protected override async Task Handle(AttachDeviceCommand request, CancellationToken cancellationToken)
        {
            var device = await _deviceRepository.FindByTokenAsync(request.DeviceToken);

            device.ClientId = request.ClientId;
            device.ActivationDate = DateTimeOffset.UtcNow;

            await _unitOfWork.SaveChangesAsync();
        }
    }
}