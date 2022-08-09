namespace WeSafe.Web.Common.Authentication.Abstract
{
    /// <summary>
    /// Provides an abstraction for the user.
    /// </summary>
    public interface IUser : ISupportUserName
    {
        /// <summary>
        /// Gets an user identifier.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets or sets a user phone number.
        /// </summary>
        string Phone { get; set; }

        /// <summary>
        /// Gets or sets a user email.
        /// </summary>
        string Email { get; set; }

        /// <summary>
        /// Gets or sets a user display name.
        /// </summary>
        string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating user is active or inactive.
        /// </summary>
        bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets a user role name.
        /// </summary>
        string RoleName { get; set; }
    }
}