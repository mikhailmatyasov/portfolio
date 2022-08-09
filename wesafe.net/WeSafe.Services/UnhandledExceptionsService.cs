using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.DAL;
using WeSafe.DAL.Entities;
using WeSafe.DAL.Extensions;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Shared;

namespace WeSafe.Services
{
    /// <inheritdoc cref="IUnhandledExceptionsService"/>>
    public class UnhandledExceptionsService : BaseService, IUnhandledExceptionsService
    {
        #region Fields

        private readonly ExceptionMapper _exceptionMapper;

        #endregion

        #region Constructor

        public UnhandledExceptionsService(WeSafeDbContext context, ILoggerFactory loggerFactory, ExceptionMapper exceptionMapper) : base(context, loggerFactory)
        {
            _exceptionMapper = exceptionMapper;
        }

        #endregion

        #region IUnhandledExceptionsService

        public async Task InsertUnhandledLogsAsync(IEnumerable<UnhandledExceptionModel> exceptionModels)
        {
            IEnumerable<UnhandledException> unhandledExceptions = exceptionModels.Select(_exceptionMapper.ToUnhandledException).ToList();
            List<string> validUserIds = DbContext.Users.Select(u => u.Id).ToList();

            foreach (UnhandledException unhandledException in unhandledExceptions)
            {
                if (string.IsNullOrEmpty(unhandledException.UserId) || validUserIds.Any(c => c == unhandledException.UserId))
                    await DbContext.UnhandledExceptions.AddAsync(unhandledException);
            }

            await SaveChangesAsync();
        }

        public async Task<PageResponse<UnhandledExceptionModel>> GetUnhandledExceptions(UnhandledExceptionRecordQuery pageRequest)
        {
            var clientDeviceLogs = GetUnhandledExceptionsQuery(pageRequest).OrderByDescending(d => d.DateTime);
            var result = await clientDeviceLogs.ApplyPageRequest(pageRequest);

            return new PageResponse<UnhandledExceptionModel>
            {
                Items = GetUnhandledExceptions(result.Query),
                Total = result.Total
            };
        }

        public async Task DeleteUnhandledExceptionsOlderThan(TimeSpan time)
        {
            //[LS] To delete records without loading data from entity framework.
            var dateTime = DateTime.UtcNow.Add(-time);
            await DbContext.Database.ExecuteSqlCommandAsync("DELETE FROM \"UnhandledExceptions\" WHERE \"DateTime\" <= @maxDateTime",
                new NpgsqlParameter("@maxDateTime", dateTime));
        }

        #endregion

        #region Private methods

        private IEnumerable<UnhandledExceptionModel> GetUnhandledExceptions(IQueryable<UnhandledException> resultQuery)
        {
            //[LS] : To optimize select query to database.
            return resultQuery?.Select(unhandledException => new UnhandledExceptionModel
            {
                UserName = unhandledException.User.UserName,
                ErrorMessage = unhandledException.ErrorMessage,
                DateTime = unhandledException.DateTime,
                StackTrace = unhandledException.StackTrace

            }).AsNoTracking().ToList();
        }

        private IQueryable<UnhandledException> GetUnhandledExceptionsQuery(UnhandledExceptionRecordQuery pageRequest)
        {
            var unhandledExceptions = DbContext.UnhandledExceptions.Include(e => e.User).AsQueryable();

            if (!string.IsNullOrWhiteSpace(pageRequest.UserName))
            {
                unhandledExceptions = pageRequest.UserName == Consts.Consts.defaultUserName
                    ? unhandledExceptions.Where(e => e.User == null)
                    : unhandledExceptions.Where(e => e.User.UserName == pageRequest.UserName);
            }

            if (pageRequest.FromDate.HasValue)
                unhandledExceptions = unhandledExceptions.Where(e => e.DateTime > pageRequest.FromDate.Value);

            if (pageRequest.ToDate.HasValue)
                unhandledExceptions = unhandledExceptions.Where(e => e.DateTime <= pageRequest.ToDate.Value);

            if (!string.IsNullOrWhiteSpace(pageRequest.SearchText))
                unhandledExceptions = unhandledExceptions.Where(e => e.ErrorMessage.ToLower().Contains(pageRequest.SearchText.ToLower()));

            return unhandledExceptions;
        }

        #endregion
    }
}
