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
    /// Represents a <see cref="GetDevicesByClientIdCommand"/> handler.
    /// </summary>
    public class GetDevicesByClientIdCommandHandler : IRequestHandler<GetDevicesByClientIdCommand, IEnumerable<DeviceModel>>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IMapper _mapper;

        #endregion

        #region Constructors

        public GetDevicesByClientIdCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _deviceRepository = unitOfWork.GetCustomRepository<IDeviceRepository, Device>();
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        #endregion

        public async Task<IEnumerable<DeviceModel>> Handle(GetDevicesByClientIdCommand request, CancellationToken cancellationToken)
        {
            var devices = await _deviceRepository.FindAllByClientIdAsync(request.ClientId);

            return _mapper.Map<IEnumerable<DeviceModel>>(devices);
        }
    }
}