using MediatR;
using WeSafe.Authentication.WebApi.Enumerations;

namespace WeSafe.Authentication.WebApi.Commands.MobileUser
{
    /// <summary>
    /// Represents a mobile user creation command.
    /// </summary>
    public class CreateMobileUserCommand : IRequest<int>
    {
        /// <summary>
        /// Gets or sets a phone number of the mobile user.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets a status of the mobile user.
        /// </summary>
        public MobileUserStatus Status { get; set; }
    }
}