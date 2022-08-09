namespace WeSafe.Authentication.WebApi.Enumerations
{
    /// <summary>
    /// Represents a result of login verification command.
    /// </summary>
    public enum LoginStatus
    {
        /// <summary>
        /// Login verified and not exists.
        /// </summary>
        Ok,

        /// <summary>
        /// Login already exists.
        /// </summary>
        Exists
    }
}