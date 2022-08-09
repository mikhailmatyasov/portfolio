using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using MediatR;
using WeSafe.DAL.Abstractions;
using WeSafe.DAL.Entities;
using WeSafe.DAL.Extensions;
using WeSafe.Dashboard.WebApi.Models;

namespace WeSafe.Dashboard.WebApi.Commands.Devices
{
    /// <summary>
    /// Represents a <see cref="GetDeviceByClientIdCommand"/> handler.
    /// </summary>
    public class GetDeviceByClientIdCommandHandler : IRequestHandler<GetDeviceByClientIdCommand, DeviceModel>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IMapper _mapper;

        #endregion

        #region Constructors

        public GetDeviceByClientIdCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _deviceRepository = unitOfWork.GetCustomRepository<IDeviceRepository, Device>();
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        #endregion

        public async Task<DeviceModel> Handle(GetDeviceByClientIdCommand request, CancellationToken cancellationToken)
        {
            var devices = await _deviceRepository.FindByClientIdAsync(request.ClientId, request.DeviceId);

            return _mapper.Map<DeviceModel>(devices);
        }
    }
}