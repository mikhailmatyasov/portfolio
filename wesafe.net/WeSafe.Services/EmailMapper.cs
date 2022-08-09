using WeSafe.DAL.Entities;
using WeSafe.Services.Client.Models;

namespace WeSafe.Services
{
    /// <summary>
    /// Represents email mapper.
    /// </summary>
    public class EmailMapper
    {
        /// <summary>
        /// Map email model to email entity model.
        /// </summary>
        /// <param name="email">Email model.</param>
        /// <returns>Email entity model.</returns>
        public Email ToEmail(EmailModel email)
        {
            return new Email()
            {
                Id = email.Id,
                MailAddress = email.MailAddress
            };
        }
    }
}
