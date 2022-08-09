using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.DAL;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Shared.Results;

namespace WeSafe.Services
{
    /// <inheritdoc cref="IEmailService"/>>
    public class EmailService : BaseService, IEmailService
    {
        #region Fields

        private readonly EmailMapper _mapper;

        #endregion

        #region Constructor

        public EmailService(WeSafeDbContext context, ILoggerFactory loggerFactory, EmailMapper mapper) : base(context, loggerFactory)
        {
            _mapper = mapper;
        }

        #endregion

        #region IEmailService

        public async Task<List<EmailModel>> GetEmailsAsync()
        {
            return await DbContext.Emails.Select(e => new EmailModel()
            {
                Id = e.Id,
                MailAddress = e.MailAddress,
                NotifyServerException = e.NotifyServerException
            }).ToListAsync();
        }

        public async Task<IExecutionResult> CreateEmail(EmailModel model)
        {
            ValidateEmail(model);

            var email = _mapper.ToEmail(model);

            await DbContext.Emails.AddAsync(email);
            await SaveChangesAsync();

            return ExecutionResult.Success();
        }

        public async Task<IExecutionResult> RemoveEmail(int id)
        {
            var email = await DbContext.Emails.FindAsync(id);

            if (email == null)
                throw new NullReferenceException("Email with id " + id + " is not found.");

            DbContext.Emails.Remove(email);
            await SaveChangesAsync();

            return ExecutionResult.Success();
        }

        public async Task<IExecutionResult> ChangeNotifyServerExceptionValue(int id)
        {
            var email = await DbContext.Emails.FindAsync(id);

            if (email == null)
                throw new InvalidOperationException("Email with id " + id + " is not found.");

            email.NotifyServerException = !email.NotifyServerException;
            await SaveChangesAsync();

            return ExecutionResult.Success();
        }

        public async Task<bool> IsExistsAsync(string email)
        {
            return await DbContext.Emails.AnyAsync(e => e.MailAddress == email);
        }

        public async Task<bool> IsExistsAsync(int id)
        {
            return await DbContext.Emails.AnyAsync(e => e.Id == id);
        }

        #endregion

        #region Private Methods

        private async void ValidateEmail(EmailModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (await IsExistsAsync(model.MailAddress))
                throw new InvalidOperationException($"{nameof(model.MailAddress)} already exists.");
        }

        #endregion
    }
}
