using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WeSafe.Authentication.WebApi.Enumerations;
using WeSafe.Web.Common.Authentication.Abstract;

namespace WeSafe.Authentication.WebApi.Commands.VerifyLogin
{
    /// <summary>
    /// Represents a <see cref="VerifyLoginCommand"/> handler
    /// </summary>
    public class VerifyLoginCommandHandler : IRequestHandler<VerifyLoginCommand, LoginStatus>
    {
        #region Fields

        private readonly IUserManager _userManager;

        #endregion

        #region Constructors

        public VerifyLoginCommandHandler(IUserManager userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        #endregion

        public async Task<LoginStatus> Handle(VerifyLoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);

            if ( user != null )
            {
                return LoginStatus.Exists;
            }

            return LoginStatus.Ok;
        }
    }
}