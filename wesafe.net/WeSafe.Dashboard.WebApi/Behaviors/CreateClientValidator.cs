using System;
using System.Threading;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using MediatR.Pipeline;
using WeSafe.DAL.Abstractions;
using WeSafe.DAL.Entities;
using WeSafe.DAL.Extensions;
using WeSafe.Dashboard.WebApi.Commands.Clients;

namespace WeSafe.Dashboard.WebApi.Behaviors
{
    /// <summary>
    /// Represents a create client operation validator.
    /// </summary>
    /// <remarks>
    /// Checks if any client with the given phone number is exists.
    /// </remarks>
    public class CreateClientValidator : IRequestPreProcessor<CreateClientCommand>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IClientRepository _clientRepository;

        #endregion

        #region Constructors

        public CreateClientValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _clientRepository = unitOfWork.GetCustomRepository<IClientRepository, Client>();
        }

        #endregion

        public async Task Process(CreateClientCommand request, CancellationToken cancellationToken)
        {
            if ( await _clientRepository.FindByPhoneAsync(request.Client.Phone, true) != null )
            {
                throw new InvalidOperationException($"Client with phone number {request.Client.Phone} already exists.");
            }
        }
    }
}