namespace WeSafe.Shared.Enumerations
{
    /// <summary>
    /// Camera state
    /// </summary>
    public enum CameraState
    {
        /// <summary>
        /// Camera is successfully connected manually or other methods (ONVIF) and ready to work.
        /// </summary>
        Connected,

        /// <summary>
        /// Camera was detected and not been connected yet.
        /// </summary>
        Detected,

        /// <summary>
        /// Camera is connecting.
        /// </summary>
        Connecting,

        /// <summary>
        /// Camera is not connected.
        /// </summary>
        Failure
    }
}