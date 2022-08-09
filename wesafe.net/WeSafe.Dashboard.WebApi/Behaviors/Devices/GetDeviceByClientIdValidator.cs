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
    public class GetDeviceByClientIdValidator : IRequestPreProcessor<GetDeviceByClientIdCommand>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IDeviceRepository _deviceRepository;

        #endregion

        #region Constructors

        public GetDeviceByClientIdValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _deviceRepository = unitOfWork.GetCustomRepository<IDeviceRepository, Device>();
        }

        #endregion

        public async Task Process(GetDeviceByClientIdCommand request, CancellationToken cancellationToken)
        {
            var device = await _deviceRepository.FindByClientIdAsync(request.ClientId, request.DeviceId);

            if ( device == null )
            {
                throw new InvalidOperationException($"The client device with '{request.DeviceId}' id is not found.");
            }
        }
    }
}