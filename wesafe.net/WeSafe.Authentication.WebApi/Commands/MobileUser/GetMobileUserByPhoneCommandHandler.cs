using System;
using System.Threading;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using MediatR;
using WeSafe.Authentication.WebApi.Models;
using WeSafe.DAL.Abstractions;
using WeSafe.DAL.Extensions;

namespace WeSafe.Authentication.WebApi.Commands.MobileUser
{
    /// <summary>
    /// Represents a command handler to get a mobile user with the specified phone number.
    /// </summary>
    public class GetMobileUserByPhoneCommandHandler : IRequestHandler<GetMobileUserByPhoneCommand, MobileUserModel>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMobileUserRepository _mobileUserRepository;
        private readonly IMapper _mapper;

        #endregion

        #region Constructors

        public GetMobileUserByPhoneCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mobileUserRepository = _unitOfWork.GetCustomRepository<IMobileUserRepository, DAL.Entities.MobileUser>();
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        #endregion

        public async Task<MobileUserModel> Handle(GetMobileUserByPhoneCommand request, CancellationToken cancellationToken)
        {
            var user = await _mobileUserRepository.FindByPhoneNumberAsync(request.PhoneNumber, true);

            return _mapper.Map<MobileUserModel>(user);
        }
    }
}