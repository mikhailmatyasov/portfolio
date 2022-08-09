using System;
using System.Threading;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using MediatR.Pipeline;
using WeSafe.DAL.Abstractions;
using WeSafe.DAL.Entities;
using WeSafe.DAL.Extensions;
using WeSafe.Dashboard.WebApi.Abstractions;

namespace WeSafe.Dashboard.WebApi.Behaviors
{
    /// <summary>
    /// Represents a base class to validate command operation with searching device by the token.
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    public abstract class DeviceByTokenValidator<TCommand> : IRequestPreProcessor<TCommand> where TCommand : IDeviceByToken
    {
        #region Constructors

        protected DeviceByTokenValidator(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            DeviceRepository = unitOfWork.GetCustomRepository<IDeviceRepository, Device>();
        }

        #endregion

        #region Properties

        protected IUnitOfWork UnitOfWork { get; }

        protected IDeviceRepository DeviceRepository { get; }

        #endregion

        public async Task Process(TCommand request, CancellationToken cancellationToken)
        {
            var device = await DeviceRepository.FindByTokenAsync(request.DeviceToken, true);

            if ( device == null )
            {
                throw new InvalidOperationException($"The device with '{request.DeviceToken}' token is not found.");
            }

            await Process(device, request, cancellationToken);
        }

        protected abstract Task Process(Device device, TCommand request, CancellationToken cancellationToken);
    }
}