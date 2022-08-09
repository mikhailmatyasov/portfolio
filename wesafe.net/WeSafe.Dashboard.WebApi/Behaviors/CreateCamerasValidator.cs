using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using MediatR.Pipeline;
using WeSafe.DAL.Abstractions;
using WeSafe.DAL.Entities;
using WeSafe.DAL.Extensions;
using WeSafe.Dashboard.WebApi.Commands.Devices;
using WeSafe.Web.Common.Exceptions;

namespace WeSafe.Dashboard.WebApi.Behaviors
{
    /// <summary>
    /// Represents a cameras creation operation validator
    /// </summary>
    public class CreateCamerasValidator : IRequestPreProcessor<CreateCamerasCommand>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IDeviceRepository _deviceRepository;
        private readonly ICameraRepository _cameraRepository;

        #endregion

        #region Constructors

        public CreateCamerasValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _deviceRepository = unitOfWork.GetCustomRepository<IDeviceRepository, Device>();
            _cameraRepository = unitOfWork.GetCustomRepository<ICameraRepository, DAL.Entities.Camera>();
        }

        #endregion

        public async Task Process(CreateCamerasCommand request, CancellationToken cancellationToken)
        {
            var device = await _deviceRepository.FindByMacAddressAsync(request.MacAddress, true);

            if (device == null)
            {
                throw new NotFoundException($"The device with '{request.MacAddress}' mac address is not found.");
            }

            var deviceCameras = await _cameraRepository.GetCamerasByDeviceIdAsync(device.Id);

            request.Cameras = request.Cameras.Where(c => 
                    deviceCameras.All(dc => dc.Ip != c.Ip && dc.Port != c.Port))
                .ToList();
        }
    }
}
