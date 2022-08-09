using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WeSafe.DAL.Abstractions;
using WeSafe.DAL.Extensions;
using WeSafe.Dashboard.WebApi.Models;

namespace WeSafe.Dashboard.WebApi.Commands.Camera
{
    public class GetCameraNamesByIdCommandHandler : IRequestHandler<GetCameraNamesByIdCommand, IEnumerable<CameraModelResponse>>
    {
        private readonly ICameraRepository _cameraRepository;
        private readonly IMapper _mapper;

        public GetCameraNamesByIdCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _cameraRepository = unitOfWork.GetCustomRepository<ICameraRepository, DAL.Entities.Camera>();
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<CameraModelResponse>> Handle(GetCameraNamesByIdCommand request, CancellationToken cancellationToken)
        {
            var cameras = await _cameraRepository.GetCamerasNames(request.Ids.Distinct());

            return _mapper.Map<IEnumerable<CameraModelResponse>>(cameras);
        }
    }
}
