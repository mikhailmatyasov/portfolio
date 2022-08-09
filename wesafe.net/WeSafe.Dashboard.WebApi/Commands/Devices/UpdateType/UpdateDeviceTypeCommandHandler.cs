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
    public class UpdateDeviceTypeCommandHandler : AsyncRequestHandler<UpdateDeviceTypeCommand>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IDeviceRepository _deviceRepository;

        #endregion

        #region Constructors

        public UpdateDeviceTypeCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _deviceRepository = unitOfWork.GetCustomRepository<IDeviceRepository, Device>();
        }

        #endregion

        protected override async Task Handle(UpdateDeviceTypeCommand request, CancellationToken cancellationToken)
        {
            var device = await _deviceRepository.FindAsync(request.DeviceId);

            device.DeviceType = request.DeviceType;

            await _unitOfWork.SaveChangesAsync();
        }
    }
}