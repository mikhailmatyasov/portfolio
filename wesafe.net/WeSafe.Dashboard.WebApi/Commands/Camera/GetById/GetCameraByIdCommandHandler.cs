using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using WeSafe.Dashboard.WebApi.Models;
using WeSafe.Web.Common.Exceptions;

namespace WeSafe.Dashboard.WebApi.Commands.Camera
{
    public class GetCameraByIdCommandHandler : IRequestHandler<GetCameraByIdCommand, CameraModelResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetCameraByIdCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<CameraModelResponse> Handle(GetCameraByIdCommand request, CancellationToken cancellationToken)
        {
            var camera = await _unitOfWork.GetRepository<DAL.Entities.Camera>().FindAsync(request.Id);
            if (camera == null)
                throw new NotFoundException("The camera not found. Please contact with support.");

            return _mapper.Map<CameraModelResponse>(camera);
        }
    }
}
