using MediatR;
using Newtonsoft.Json;
using WeSafe.Authentication.WebApi.Models;
using WeSafe.Web.Common.Authentication;

namespace WeSafe.Authentication.WebApi.Commands.MobileLogin
{
    /// <summary>
    /// Represents a mobile device login command
    /// </summary>
    public class MobileLoginCommand : IRequest<TokenResponseModel>
    {
        /// <summary>
        /// Gets or sets a phone number.
        /// </summary>
        [JsonProperty("UserName")]
        public string PhoneNumber { get; set; }
    }
}