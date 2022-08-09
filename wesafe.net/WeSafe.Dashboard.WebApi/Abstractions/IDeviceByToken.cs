namespace WeSafe.Dashboard.WebApi.Abstractions
{
    /// <summary>
    /// Provides an abstraction to support searching device by token.
    /// </summary>
    public interface IDeviceByToken
    {
        /// <summary>
        /// Gets or sets a device token to search for.
        /// </summary>
        public string DeviceToken { get; set; }
    }
}