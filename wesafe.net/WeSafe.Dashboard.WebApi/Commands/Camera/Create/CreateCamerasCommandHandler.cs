using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using MediatR;
using WeSafe.DAL.Abstractions;
using WeSafe.DAL.Entities;
using WeSafe.DAL.Extensions;
using WeSafe.Dashboard.WebApi.Commands.Devices;

namespace WeSafe.Dashboard.WebApi.Commands.Camera
{
    /// <summary>
    /// Represents a cameras creation command handler.
    /// </summary>
    public class CreateCamerasCommandHandler : AsyncRequestHandler<CreateCamerasCommand>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICameraRepository _cameraRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IMapper _mapper;

        #endregion

        #region Constructors

        public CreateCamerasCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _cameraRepository = unitOfWork.GetCustomRepository<ICameraRepository, DAL.Entities.Camera>();
            _deviceRepository = unitOfWork.GetCustomRepository<IDeviceRepository, Device>();
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        #endregion

        protected override async Task Handle(CreateCamerasCommand request, CancellationToken cancellationToken)
        {
            var device = await _deviceRepository.FindByMacAddressAsync(request.MacAddress);

            var cameras = request.Cameras.Select(c =>
            {
                var camera = _mapper.Map<DAL.Entities.Camera>(c);
                camera.Device = device;

                return camera;
            }).ToList();

            await _cameraRepository.InsertAsync(cameras, cancellationToken);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}