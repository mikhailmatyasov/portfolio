using AutoMapper;
using MassTransit;
using MediatR.Pipeline;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WeSafe.Authentication.WebApi.Commands.Users;
using WeSafe.Authentication.WebApi.Validators.Users;
using WeSafe.Bus.Contracts.User;
using WeSafe.Web.Common.Exceptions;

namespace WeSafe.Authentication.WebApi.Consumers
{
    public class CreateUserValidatorConsumer : IConsumer<ICreateUserValidationContract>
    {
        private readonly IRequestPreProcessor<CreateUserCommand> _createRequestPreprocessor;

        private readonly CreateUserCommandValidator _commandUserValidator;

        private readonly IMapper _mapper;

        public CreateUserValidatorConsumer(IRequestPreProcessor<CreateUserCommand> createRequestPreprocessor, CreateUserCommandValidator commandUserValidator, IMapper mapper)
        {
            _createRequestPreprocessor = createRequestPreprocessor ??
                                         throw new ArgumentNullException(nameof(createRequestPreprocessor));

            _commandUserValidator =
                commandUserValidator ?? throw new ArgumentNullException(nameof(createRequestPreprocessor));

            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Consume(ConsumeContext<ICreateUserValidationContract> context)
        {
            try
            {
                var command = _mapper.Map<CreateUserCommand>(context.Message);

                var validationResult = await _commandUserValidator.ValidateAsync(command, CancellationToken.None);
                if (!validationResult.IsValid)
                    throw new BadRequestException(string.Join(',', validationResult.Errors.Select(x => x.ErrorMessage)));

                await _createRequestPreprocessor.Process(command, CancellationToken.None);

                await context.RespondAsync<ICreateUserValidationResult>(new
                {
                    IsValid = true
                });
            }
            catch (Exception ex)
            {
                await context.RespondAsync<ICreateUserValidationResult>(new
                {
                    IsValid = false,
                    ErrorMessage = ex.Message
                });
            }
        }
    }
}
