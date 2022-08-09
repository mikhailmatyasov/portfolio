using MediatR;
using WeSafe.Web.Common.Authentication.Abstract;

namespace WeSafe.Authentication.WebApi.Commands.Users
{
    /// <summary>
    /// Represents a user creation command.
    /// </summary>
    public class CreateUserCommand : IRequest<string>, IUser
    {
        #region Constructors

        public CreateUserCommand()
        {
            Id = null;
        }

        #endregion

        #region IUser implementation

        public string Id { get; }

        public string UserName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string DisplayName { get; set; }

        public bool IsActive { get; set; }

        public string RoleName { get; set; }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets a user password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets client identifier
        /// </summary>
        public int? ClientId { get; set; }

        #endregion
    }
}