using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using WeSafe.Authentication.WebApi.Models;
using WeSafe.Authentication.WebApi.Services.Abstract;
using WeSafe.DAL.Entities;
using WeSafe.Web.Common.Authentication;
using WeSafe.Web.Common.Authentication.Abstract;

namespace WeSafe.Authentication.WebApi.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, TokenResponseModel>
    {
        private const string _originDesktop = "desktop";

        private readonly IAuthTokenGenerator _authTokenGenerator;
        private readonly IUserManager _userManager;

        public LoginCommandHandler(IUserManager userManager, IAuthTokenGenerator authTokenGenerator)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _authTokenGenerator = authTokenGenerator ?? throw new ArgumentNullException(nameof(authTokenGenerator));
        }

        public async Task<TokenResponseModel> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);

            return await CreateTokenResponse(user, request.Origin);
        }

        private async Task<TokenResponseModel> CreateTokenResponse(User user, string origin)
        {
            var role = await _userManager.GetUserRole(user);
            var expiresAt = DateTime.UtcNow.Add(origin == _originDesktop ? AuthOptions.LifetimeDesktop : AuthOptions.Lifetime);

            return _authTokenGenerator.CreateToken(new TokenRequestModel(user.Id, user.UserName, expiresAt)
            {
                Role = role,
                DisplayName = user.DisplayName,
                Demo = user.Demo,
                ClientId = user.ClientId
            });
        }
    }
}
