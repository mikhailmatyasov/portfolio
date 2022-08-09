using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeSafe.Services.Client.Models;
using WeSafe.Shared;

namespace WeSafe.Services.Client
{
    /// <summary>
    /// Represents unhandled exceptions service that handle exceptions.
    /// </summary>
    public interface IUnhandledExceptionsService
    {
        /// <summary>
        /// Inserts exceptions to the queue.
        /// </summary>
        /// <param name="exceptionModels">Collection of unhandled exceptions.</param>
        /// <returns>Task.</returns>
        Task InsertUnhandledLogsAsync(IEnumerable<UnhandledExceptionModel> exceptionModels);

        /// <summary>
        /// Gets unhandled exceptions that fit the query.
        /// </summary>
        /// <param name="pageRequest">Record query.</param>
        /// <returns>Task.</returns>
        Task<PageResponse<UnhandledExceptionModel>> GetUnhandledExceptions(UnhandledExceptionRecordQuery pageRequest);

        /// <summary>
        /// Deletes expired unhandled exceptions.
        /// </summary>
        /// <param name="time">Not expired time interval.</param>
        /// <returns>Task.</returns>
        Task DeleteUnhandledExceptionsOlderThan(TimeSpan time);
    }
}
