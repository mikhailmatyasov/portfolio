using System;
using System.Threading;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using MediatR.Pipeline;
using WeSafe.Authentication.WebApi.Commands.MobileLogin;
using WeSafe.DAL.Abstractions;
using WeSafe.DAL.Entities;
using WeSafe.DAL.Extensions;
using WeSafe.Web.Common.Exceptions;

namespace WeSafe.Authentication.WebApi.Behaviors
{
    /// <summary>
    /// Represents a mobile device login operation validator.
    /// </summary>
    /// <remarks>
    /// Checks if any active subscriber with the given phone number is exists.
    /// </remarks>
    public class MobileValidator : IRequestPreProcessor<MobileLoginCommand>
    {
        #region Fields

        private readonly ISubscriberRepository _subscriberRepository;

        #endregion

        #region Constructors

        public MobileValidator(IUnitOfWork unitOfWork)
        {
            if ( unitOfWork == null )
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            _subscriberRepository = unitOfWork.GetCustomRepository<ISubscriberRepository, ClientSubscriber>();
        }

        #endregion

        public async Task Process(MobileLoginCommand request, CancellationToken cancellationToken)
        {
            if ( !await _subscriberRepository.HasActiveClientsAsync(request.PhoneNumber) )
            {
                throw new UnauthorizedException($"The user with phone number {request.PhoneNumber} not found");
            }
        }
    }
}