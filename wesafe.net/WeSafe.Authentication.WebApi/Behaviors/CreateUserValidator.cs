using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR.Pipeline;
using WeSafe.Authentication.WebApi.Commands.Users;
using WeSafe.Web.Common.Authentication.Abstract;
using WeSafe.Web.Common.Exceptions;

namespace WeSafe.Authentication.WebApi.Behaviors
{
    /// <summary>
    /// Represents a operation validator for <see cref="CreateUserCommand"/>.
    /// </summary>
    /// <remarks>
    /// Checks if user with the specified user name already exists. If true - throws an exception <see cref="BadRequestException"/>.
    /// </remarks>
    public class CreateUserValidator : IRequestPreProcessor<CreateUserCommand>
    {
        #region Fields

        private readonly IUserManager _userManager;

        #endregion

        #region Constructors

        public CreateUserValidator(IUserManager userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        #endregion

        public async Task Process(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);

            if ( user != null )
            {
                throw new BadRequestException($"User with username '{request.UserName}' already exists.");
            }
        }
    }
}