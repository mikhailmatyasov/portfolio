using System.Threading.Tasks;
using WeSafe.Services.Client.Models;

namespace WeSafe.Services.Client
{
    /// <summary>
    /// Represents an email sender that sends email.
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        /// Sends message to email.
        /// </summary>
        /// <param name="message">Sent message.</param>
        /// <returns>Task.</returns>
        Task SendEmailAsync(EmailMessageModel message);
    }
}
