namespace WeSafe.Dashboard.WebApi.Enumerations
{
    /// <summary>
    /// Represents a device token verifying status.
    /// </summary>
    public enum DeviceVerificationStatus
    {
        /// <summary>
        /// Device is exists and ready to use.
        /// </summary>
        Exists,

        /// <summary>
        /// Device is exists and attached to a client.
        /// </summary>
        Attached,

        /// <summary>
        /// Device is not found.
        /// </summary>
        NotFound
    }
}