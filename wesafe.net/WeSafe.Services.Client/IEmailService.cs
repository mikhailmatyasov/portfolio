using System.Collections.Generic;
using System.Threading.Tasks;
using WeSafe.Services.Client.Models;
using WeSafe.Shared.Results;

namespace WeSafe.Services.Client
{
    /// <summary>
    /// Represents email service.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Get emails.
        /// </summary>
        /// <returns>Task.</returns>
        Task<List<EmailModel>> GetEmailsAsync();

        /// <summary>
        /// Creates email.
        /// </summary>
        /// <param name="model">Email model.</param>
        /// <returns>Task.</returns>
        Task<IExecutionResult> CreateEmail(EmailModel model);

        /// <summary>
        /// Removes email.
        /// </summary>
        /// <param name="id">Email identifier.</param>
        /// <returns>Task.</returns>
        Task<IExecutionResult> RemoveEmail(int id);

        /// <summary>
        /// Changes notify server exception value.
        /// </summary>
        /// <param name="id">Email identifier.</param>
        /// <returns>Task.</returns>
        Task<IExecutionResult> ChangeNotifyServerExceptionValue(int id);

        /// <summary>
        /// Checks if email is created already.
        /// </summary>
        /// <param name="email">EMail string.</param>
        /// <returns>Task.</returns>
        Task<bool> IsExistsAsync(string email);

        /// <summary>
        /// Checks if email is created already.
        /// </summary>
        /// <param name="id">Email identifier.</param>
        /// <returns>Task.</returns>
        Task<bool> IsExistsAsync(int id);
    }
}
