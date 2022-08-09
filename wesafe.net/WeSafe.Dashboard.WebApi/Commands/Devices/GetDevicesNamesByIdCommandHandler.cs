using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WeSafe.DAL.Abstractions;
using WeSafe.DAL.Entities;
using WeSafe.DAL.Extensions;
using WeSafe.Dashboard.WebApi.Models;

namespace WeSafe.Dashboard.WebApi.Commands.Devices
{
    public class GetDevicesNamesByIdCommandHandler : IRequestHandler<GetDevicesNamesByIdCommand, IEnumerable<DeviceModel>>
    {
        #region Fields

        private readonly IDeviceRepository _deviceRepository;
        private readonly IMapper _mapper;

        #endregion

        #region Constructors

        public GetDevicesNamesByIdCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            _deviceRepository = unitOfWork.GetCustomRepository<IDeviceRepository, Device>();
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        #endregion

        #region IRequestHandler

        public async Task<IEnumerable<DeviceModel>> Handle(GetDevicesNamesByIdCommand request, CancellationToken cancellationToken)
        {
            var ids = request.Ids.Distinct();

            var devices = await _deviceRepository.GetDevicesWithNames(ids);

            return _mapper.Map<IEnumerable<DeviceModel>>(devices).ToList();
        }

        #endregion
    }
}
