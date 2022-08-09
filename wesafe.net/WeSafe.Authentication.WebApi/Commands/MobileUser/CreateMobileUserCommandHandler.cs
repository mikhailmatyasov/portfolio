using System;
using System.Threading;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using MediatR;
using WeSafe.Authentication.WebApi.Enumerations;
using WeSafe.DAL.Abstractions;
using WeSafe.DAL.Extensions;

namespace WeSafe.Authentication.WebApi.Commands.MobileUser
{
    /// <summary>
    /// Represents a mobile user creation handler
    /// </summary>
    public class CreateMobileUserCommandHandler : IRequestHandler<CreateMobileUserCommand, int>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMobileUserRepository _mobileUserRepository;

        #endregion

        #region Constructors

        public CreateMobileUserCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mobileUserRepository = _unitOfWork.GetCustomRepository<IMobileUserRepository, DAL.Entities.MobileUser>();
        }

        #endregion

        #region IRequestHandler implementation

        public async Task<int> Handle(CreateMobileUserCommand request, CancellationToken cancellationToken)
        {
            var user = new DAL.Entities.MobileUser
            {
                Phone = request.PhoneNumber,
                CreatedAt = DateTimeOffset.UtcNow,
                Status = request.Status == MobileUserStatus.None ? null : request.Status.ToString()
            };

            _mobileUserRepository.Insert(user);

            await _unitOfWork.SaveChangesAsync();

            return user.Id;
        }

        #endregion
    }
}