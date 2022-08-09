namespace WeSafe.Web.Common.Authentication.Abstract
{
    /// <summary>
    /// Provides an abstraction to support user name.
    /// </summary>
    public interface ISupportUserName
    {
        /// <summary>
        /// Gets or sets a user name (login).
        /// </summary>
        string UserName { get; set; }
    }
}