using System;
using System.Threading;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using MediatR;
using WeSafe.DAL.Abstractions;
using WeSafe.DAL.Entities;
using WeSafe.DAL.Extensions;

namespace WeSafe.Dashboard.WebApi.Commands.Subscribers
{
    /// <summary>
    /// Represents a client subscriber creation operation handler.
    /// </summary>
    public class CreateSubscriberCommandHandler : IRequestHandler<CreateSubscriberCommand, int>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly ISubscriberRepository _subscriberRepository;

        #endregion

        #region Constructors

        public CreateSubscriberCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _subscriberRepository = unitOfWork.GetCustomRepository<ISubscriberRepository, ClientSubscriber>();
        }

        #endregion

        public async Task<int> Handle(CreateSubscriberCommand request, CancellationToken cancellationToken)
        {
            var subscriber = new ClientSubscriber
            {
                ClientId = request.Subscriber.ClientId,
                Phone = request.Subscriber.Phone,
                Permissions = request.Subscriber.Permissions.ToString(),
                IsActive = request.Subscriber.IsActive,
                CreatedAt = DateTimeOffset.UtcNow,
                Name = request.Subscriber.Name
            };

            _subscriberRepository.Insert(subscriber);

            await _unitOfWork.SaveChangesAsync();

            return subscriber.Id;
        }
    }
}