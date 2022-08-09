using System;
using System.Threading;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using MediatR;
using WeSafe.DAL.Abstractions;
using WeSafe.DAL.Entities;
using WeSafe.DAL.Extensions;
using WeSafe.Dashboard.WebApi.Commands.Subscribers;
using WeSafe.Dashboard.WebApi.Enumerations;
using WeSafe.Dashboard.WebApi.Models;

namespace WeSafe.Dashboard.WebApi.Commands.Clients
{
    /// <summary>
    /// Represents a client creation operation handler.
    /// </summary>
    public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, int>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IClientRepository _clientRepository;
        private readonly IMediator _mediator;

        #endregion

        #region Constructors

        public CreateClientCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _clientRepository = unitOfWork.GetCustomRepository<IClientRepository, Client>();
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        #endregion

        public async Task<int> Handle(CreateClientCommand request, CancellationToken cancellationToken)
        {
            var client = _mapper.Map<Client>(request.Client);

            client.CreatedAt = DateTimeOffset.UtcNow;

            _clientRepository.Insert(client);

            await _unitOfWork.SaveChangesAsync();

            await _mediator.Send(new CreateSubscriberCommand
            {
                Subscriber = new SubscriberModel
                {
                    ClientId = client.Id,
                    Name = client.Name,
                    Phone = client.Phone,
                    Permissions = SubscriberPermission.Owner,
                    IsActive = true
                }
            }, cancellationToken);

            return client.Id;
        }
    }
}