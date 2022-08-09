using System;

namespace WeSafe.Web.Common.Authentication.Abstract
{
    /// <summary>
    /// Provides an abstraction of the current authenticated user.
    /// </summary>
    public interface ICurrentUser
    {
        /// <summary>
        /// Gets a current user identifier.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets a current user role
        /// </summary>
        string Role { get; }

        /// <summary>
        /// Gets a current user client id.
        /// </summary>
        int? ClientId { get; }

        /// <summary>
        /// Checks if the specified role is assigned to a current user.
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        bool IsInRole(string role);

        /// <summary>
        /// Checks if current user has administrator privileges.
        /// </summary>
        /// <returns></returns>
        bool IsAdmin();
    }
}