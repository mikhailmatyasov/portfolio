using MassTransit;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using WeSafe.Bus.Contracts.Register;

namespace WeSafe.Dashboard.WebApi.Commands.Register
{
    public class RegisterCommandHandler : AsyncRequestHandler<RegisterCommand>
    {
        #region Fields

        private readonly IBusControl _busControl;

        #endregion

        #region Constructors

        public RegisterCommandHandler(IBusControl busControl)
        {
            _busControl = busControl ?? throw new ArgumentNullException(nameof(busControl));
        }

        #endregion

        protected override async Task Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            await _busControl.Publish<IRegisterContract>(request, cancellationToken);
        }
    }
}