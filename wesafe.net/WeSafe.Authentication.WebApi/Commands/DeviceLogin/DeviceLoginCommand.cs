using MediatR;
using Newtonsoft.Json;
using WeSafe.Web.Common.Authentication;

namespace WeSafe.Authentication.WebApi.Commands.DeviceLogin
{
    /// <summary>
    /// Represents a device login command.
    /// </summary>
    public class DeviceLoginCommand : IRequest<TokenResponseModel>
    {
        /// <summary>
        /// Gets or sets a device identifier. It should be a valid device MAC address.
        /// </summary>
        [JsonProperty("Device")]
        public string MacAddress { get; set; }

        /// <summary>
        /// Gets or sets additional security value for authentication process.
        /// </summary>
        public string Secret { get; set; }
    }
}