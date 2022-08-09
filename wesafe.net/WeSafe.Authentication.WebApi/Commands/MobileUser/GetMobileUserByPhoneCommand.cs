using MediatR;
using WeSafe.Authentication.WebApi.Models;

namespace WeSafe.Authentication.WebApi.Commands.MobileUser
{
    /// <summary>
    /// Represents a command to get a mobile user with the specified phone number.
    /// </summary>
    public class GetMobileUserByPhoneCommand : IRequest<MobileUserModel>
    {
        /// <summary>
        /// Gets or sets a phone number of the mobile user to search for.
        /// </summary>
        public string PhoneNumber { get; set; }
    }
}