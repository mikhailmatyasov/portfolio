using MailKit.Net.Smtp;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MimeKit;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using System.Linq;

namespace WeSafe.Services
{
    /// <inheritdoc cref="IEmailSender"/>
    public class EmailSender : IEmailSender
    {
        #region Fields

        private readonly EmailCredentialsOptions _options;

        #endregion

        #region Constuctor

        public EmailSender(IOptions<EmailCredentialsOptions> options)
        {
            _options = options.Value;
        }

        #endregion

        #region IEmailSender

        public async Task SendEmailAsync(EmailMessageModel message)
        {
            var mimeMessage = GetMimeMessage(message);
            
            using (var smtp = new SmtpClient())
            {
                await smtp.ConnectAsync(_options.SmtpHost, _options.SmtpPort, false);
                await smtp.AuthenticateAsync(_options.Email, _options.Password);
                await smtp.SendAsync(mimeMessage);

                await smtp.DisconnectAsync(true);
            }
        }

        #endregion

        #region Private Methods

        private MimeMessage GetMimeMessage(EmailMessageModel messageModel)
        {
            MimeMessage mimeMessage = new MimeMessage()
            {
                Subject = messageModel.Subject,
                Body = new TextPart(MimeKit.Text.TextFormat.Html) 
                {
                    Text = messageModel.Body
                }
            };

            mimeMessage.From.Add(MailboxAddress.Parse(messageModel.FromAddress.Address));
            mimeMessage.To.AddRange(messageModel.ToAddresses.Select(m => MailboxAddress.Parse(m.Address)));
            
            return mimeMessage;
        }

        #endregion
    }
}
