using System;
using System.Threading;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using MediatR;
using WeSafe.Authentication.WebApi.Commands.MobileUser;
using WeSafe.Authentication.WebApi.Enumerations;
using WeSafe.Authentication.WebApi.Models;
using WeSafe.Authentication.WebApi.Services.Abstract;
using WeSafe.Shared.Roles;
using WeSafe.Web.Common.Authentication;

namespace WeSafe.Authentication.WebApi.Commands.MobileLogin
{
    /// <summary>
    /// Represents a mobile device login operation handler.
    /// </summary>
    public class MobileLoginCommandHandler : IRequestHandler<MobileLoginCommand, TokenResponseModel>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthTokenGenerator _authTokenGenerator;
        private readonly IMediator _mediator;

        #endregion

        #region Constructors

        public MobileLoginCommandHandler(IUnitOfWork unitOfWork, IAuthTokenGenerator authTokenGenerator, IMediator mediator)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _authTokenGenerator = authTokenGenerator ?? throw new ArgumentNullException(nameof(authTokenGenerator));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        #endregion

        #region IRequestHandler implementation

        public async Task<TokenResponseModel> Handle(MobileLoginCommand request, CancellationToken cancellationToken)
        {
            var user = await GetMobileUser(request.PhoneNumber, cancellationToken);

            if ( user == null )
            {
                await _mediator.Send(new CreateMobileUserCommand
                {
                    PhoneNumber = request.PhoneNumber,
                    Status = MobileUserStatus.Active
                }, cancellationToken);

                user = await GetMobileUser(request.PhoneNumber, cancellationToken);
            }

            return CreateTokenResponse(user);
        }

        #endregion

        #region Private members

        private async Task<MobileUserModel> GetMobileUser(string phone, CancellationToken cancellationToken)
        {
            var user = await _mediator.Send(new GetMobileUserByPhoneCommand
            {
                PhoneNumber = phone
            }, cancellationToken);
            return user;
        }

        private TokenResponseModel CreateTokenResponse(MobileUserModel user)
        {
            var expiresAt = DateTime.UtcNow.Add(AuthOptions.LifetimeMobile);

            return _authTokenGenerator.CreateToken(new TokenRequestModel(user.Phone, user.Phone, expiresAt)
            {
                Role = UserRoles.Users
            });
        }

        #endregion
    }
}