using MediatR.Pipeline;
using System;
using System.Threading;
using System.Threading.Tasks;
using WeSafe.Authentication.WebApi.Commands.Login;
using WeSafe.Web.Common.Authentication.Abstract;
using WeSafe.Web.Common.Exceptions;

namespace WeSafe.Authentication.WebApi.Behaviors
{
    /// <summary>
    /// Represents a operation validator for <see cref="LoginCommand"/>.
    /// </summary>
    /// <remarks>
    /// Checks if user with specified user name and password is exists and active.
    /// </remarks>
    public class UserValidator : IRequestPreProcessor<LoginCommand>
    {
        #region Fields

        private readonly IUserManager _userManager;

        #endregion

        #region Constructors

        public UserValidator(IUserManager userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        #endregion

        public async Task Process(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user == null || !user.IsActive)
                throw new UnauthorizedException($"The user {request.UserName} not found");

            bool isAdmin = await _userManager.IsAdmin(user);

            var result = await _userManager.CheckPasswordSignInAsync(user, request.Password, !isAdmin);
            if (result.Succeeded)
                return;

            var errorMessage = result.IsLockedOut
                ? "User is locked. Try again in 10 minutes."
                : "Invalid password.";

            throw new UnauthorizedException(errorMessage);
        }
    }
}
