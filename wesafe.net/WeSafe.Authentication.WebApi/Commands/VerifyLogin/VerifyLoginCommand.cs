using MediatR;
using WeSafe.Authentication.WebApi.Enumerations;
using WeSafe.Web.Common.Authentication.Abstract;

namespace WeSafe.Authentication.WebApi.Commands.VerifyLogin
{
    /// <summary>
    /// Represents a login verification command.
    /// </summary>
    public class VerifyLoginCommand : IRequest<LoginStatus>, ISupportUserName
    {
        /// <summary>
        /// Gets or sets a login to verify.
        /// </summary>
        public string UserName { get; set; }
    }
}