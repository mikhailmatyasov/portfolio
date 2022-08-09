using System;
using System.Threading;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using MediatR.Pipeline;
using WeSafe.DAL.Abstractions;
using WeSafe.DAL.Entities;
using WeSafe.DAL.Extensions;
using WeSafe.Dashboard.WebApi.Commands.Subscribers;

namespace WeSafe.Dashboard.WebApi.Behaviors
{
    /// <summary>
    /// Represents a create subscriber operation validator.
    /// </summary>
    /// <remarks>
    /// Checks if any client subscriber with the given phone number is exists.
    /// </remarks>
    public class CreateSubscriberValidator : IRequestPreProcessor<CreateSubscriberCommand>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly ISubscriberRepository _subscriberRepository;

        #endregion

        #region Constructors

        public CreateSubscriberValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _subscriberRepository = unitOfWork.GetCustomRepository<ISubscriberRepository, ClientSubscriber>();
        }

        #endregion

        public async Task Process(CreateSubscriberCommand request, CancellationToken cancellationToken)
        {
            if ( await _subscriberRepository.AnyAsync(request.Subscriber.ClientId, request.Subscriber.Phone) )
            {
                throw new InvalidOperationException($"Subscriber with phone number {request.Subscriber.Phone} already exists.");
            }
        }
    }
}