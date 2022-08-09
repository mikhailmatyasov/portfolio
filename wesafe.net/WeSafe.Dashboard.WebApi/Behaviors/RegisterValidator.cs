using MassTransit;
using MediatR.Pipeline;
using System;
using System.Threading;
using System.Threading.Tasks;
using WeSafe.Bus.Components.Models.User;
using WeSafe.Bus.Contracts.User;
using WeSafe.Dashboard.WebApi.Commands.Clients;
using WeSafe.Dashboard.WebApi.Commands.Devices;
using WeSafe.Dashboard.WebApi.Commands.Register;
using WeSafe.Dashboard.WebApi.Models;
using WeSafe.Shared.Roles;
using WeSafe.Web.Common.Exceptions;

namespace WeSafe.Dashboard.WebApi.Behaviors
{
    /// <summary>
    /// Validates a register command operation.
    /// </summary>
    public class RegisterValidator : IRequestPreProcessor<RegisterCommand>
    {
        #region Fields

        private readonly IRequestPreProcessor<CreateClientCommand> _createClientPreprocessor;
        private readonly IRequestPreProcessor<AttachDeviceCommand> _attachDevicePreprocessor;
        private readonly IRequestClient<ICreateUserValidationContract> _createUserValidator;

        #endregion

        #region Constructors

        public RegisterValidator(IRequestPreProcessor<CreateClientCommand> createClientPreprocessor,
            IRequestPreProcessor<AttachDeviceCommand> attachDevicePreprocessor,
            IRequestClient<ICreateUserValidationContract> createUserValidator)
        {
            _createClientPreprocessor = createClientPreprocessor ??
                                        throw new ArgumentNullException(nameof(createClientPreprocessor));

            _attachDevicePreprocessor = attachDevicePreprocessor ??
                                        throw new ArgumentNullException(nameof(attachDevicePreprocessor));

            _createUserValidator = createUserValidator ??
                                   throw new ArgumentNullException(nameof(createUserValidator));
        }

        #endregion

        public async Task Process(RegisterCommand request, CancellationToken cancellationToken)
        {
            await _createClientPreprocessor.Process(new CreateClientCommand()
            {
                Client = new ClientModel()
                {
                    Phone = request.Phone
                }
            }, cancellationToken);

            await _attachDevicePreprocessor.Process(new AttachDeviceCommand()
            {
                DeviceToken = request.DeviceToken
            }, cancellationToken);

            var validationResponse = await _createUserValidator.GetResponse<ICreateUserValidationResult>(
                new CreateUserValidationContract()
                {
                    DisplayName = request.Name,
                    Phone = request.Phone,
                    UserName = request.UserName,
                    RoleName = UserRoles.Users,
                    Password = request.Password
                }, cancellationToken);

            if (validationResponse.Message.IsValid)
                return;

            throw new BadRequestException(validationResponse.Message.ErrorMessage);
        }
    }
}