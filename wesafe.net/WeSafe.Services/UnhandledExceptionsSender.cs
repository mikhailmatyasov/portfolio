using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WeSafe.DAL;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;

namespace WeSafe.Services
{
    /// <inheritdoc cref="IUnhandledExceptionsSender"/>
    public class UnhandledExceptionsSender : BaseService, IUnhandledExceptionsSender
    {
        #region Fields

        private readonly IEmailSender _emailSender;
        private readonly EmailCredentialsOptions _options;
        private const string _subject = "WeSafe monitoring";

        #endregion

        #region Constructor

        public UnhandledExceptionsSender(WeSafeDbContext context, ILoggerFactory loggerFactory,
            IEmailSender emailSender, IOptions<EmailCredentialsOptions> options) : base(context, loggerFactory)
        {
            _emailSender = emailSender;
            _options = options.Value;
        }

        #endregion

        #region IUnhandledExceptionsSender

        public async Task SendUnhandledExceptionsAsync(IEnumerable<UnhandledExceptionModel> unhandledExceptions)
        {
            IEnumerable<string> destinationEmails = await GetEmails();

            if (destinationEmails.Any())
            {
                var tasks = unhandledExceptions.SelectMany(e => SendExceptionToEmails(e, destinationEmails)).ToArray();
                await Task.WhenAll(tasks);
            }
        }

        #endregion

        #region Private Methods

        private IEnumerable<Task> SendExceptionToEmails(UnhandledExceptionModel exception, IEnumerable<string> destinationEmails)
        {
            var tasks = new List<Task>();
            var message = GetMessage(exception, destinationEmails);
            tasks.Add(_emailSender.SendEmailAsync(message));

            return tasks;
        }

        private string GetMessageBody(UnhandledExceptionModel unhandledException)
        {
            return $"User: {unhandledException.UserName ?? Consts.Consts.defaultUserName}" +
                                 $"Error: {unhandledException.ErrorMessage}" +
                                 $"StackTrace: {unhandledException.StackTrace}" +
                                 $"Date: {unhandledException.DateTime}";
        }

        private EmailMessageModel GetMessage(UnhandledExceptionModel unhandledException, IEnumerable<string> destinationEmails)
        {
            return new EmailMessageModel()
            {
                FromAddress = new MailAddress(_options.Email),
                ToAddresses = destinationEmails.Select(e => new MailAddress(e)).ToList(),
                Subject = _subject,
                Body = GetMessageBody(unhandledException)
            };
        }

        private async Task<List<string>> GetEmails()
        {
            return await DbContext.Emails.Where(e => e.NotifyServerException)
                .Select(email => email.MailAddress).AsNoTracking().ToListAsync();
        }

        #endregion
    }
}