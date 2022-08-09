using System.Collections.Generic;
using System.Net.Mail;

namespace WeSafe.Services.Client.Models
{
    /// <summary>
    /// Represents email message model.
    /// </summary>
    public class EmailMessageModel
    {
        /// <summary>
        /// Source email address.
        /// </summary>
        public MailAddress FromAddress { get; set; }

        /// <summary>
        /// Collection of destination email addresses.
        /// </summary>
        public List<MailAddress> ToAddresses { get; set; }

        /// <summary>
        /// Subject of the email message.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Body text of the email message.
        /// </summary>
        public string Body { get; set; }
    }
}
