using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WeSafe.DAL.Entities;
using WeSafe.Web.Common.Authentication.Abstract;

namespace WeSafe.Authentication.WebApi.Commands.Users
{
    /// <summary>
    /// Represents a user creation command handler.
    /// </summary>
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, string>
    {
        #region Fields

        private readonly IUserManager _userManager;

        #endregion

        #region Constructors

        public CreateUserCommandHandler(IUserManager userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        #endregion

        public async Task<string> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            string userId = await _userManager.CreateAsync(new User
            {
                UserName = request.UserName,
                PhoneNumber = request.Phone,
                Email = request.Email,
                DisplayName = request.DisplayName,
                IsActive = request.IsActive,
                ClientId = request.ClientId
            }, request.Password, request.RoleName);

            return userId;
        }
    }
}