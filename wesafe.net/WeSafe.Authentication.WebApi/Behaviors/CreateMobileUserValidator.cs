using System;
using System.Threading;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using MediatR.Pipeline;
using WeSafe.Authentication.WebApi.Commands.MobileUser;
using WeSafe.DAL.Abstractions;
using WeSafe.DAL.Entities;
using WeSafe.DAL.Extensions;

namespace WeSafe.Authentication.WebApi.Behaviors
{
    /// <summary>
    /// Represents a operation validator for <see cref="CreateMobileUserCommand"/>.
    /// </summary>
    /// <remarks>
    /// Checks if mobile user with the given phone number already exists. If true - throws a <see cref="InvalidOperationException"/>.
    /// </remarks>
    public class CreateMobileUserValidator : IRequestPreProcessor<CreateMobileUserCommand>
    {
        #region Fields

        private readonly IMobileUserRepository _repository;

        #endregion

        #region Constructors

        public CreateMobileUserValidator(IUnitOfWork unitOfWork)
        {
            if ( unitOfWork == null )
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            _repository = unitOfWork.GetCustomRepository<IMobileUserRepository, MobileUser>();
        }

        #endregion

        public async Task Process(CreateMobileUserCommand request, CancellationToken cancellationToken)
        {
            if ( await _repository.FindByPhoneNumberAsync(request.PhoneNumber, true) != null )
            {
                throw new InvalidOperationException($"Mobile user with {request.PhoneNumber} phone number already exists.");
            }
        }
    }
}