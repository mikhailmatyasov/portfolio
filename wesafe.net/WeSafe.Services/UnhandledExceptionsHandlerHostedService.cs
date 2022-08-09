using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Services.Extensions;

namespace WeSafe.Services
{
    /// <summary>
    /// Represents unhandled exceptions hosted service.
    /// </summary>
    public class UnhandledExceptionsHandlerHostedService : IHostedService
    {
        #region Fields

        private Timer _insertUnhandledExceptionsTimer;
        private Timer _deleteUnhandledExceptionsTimer;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private ConcurrentQueue<UnhandledExceptionModel> ExceptionModels { get; } = new ConcurrentQueue<UnhandledExceptionModel>();

        #endregion

        #region Constructor

        public UnhandledExceptionsHandlerHostedService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        #endregion

        #region Public Methods

        public void AddExceptionsToTheQueue(UnhandledExceptionModel unhandledException)
        {
            ExceptionModels.Enqueue(unhandledException);
        }

        #endregion

        #region IHostedService

        /// <summary>
        /// Starts service.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _insertUnhandledExceptionsTimer = new Timer(HandleUnhandledExceptions, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(20));

            _deleteUnhandledExceptionsTimer = new Timer(DeleteUnhandledExceptions, null, TimeSpan.Zero,
                TimeSpan.FromHours(12));

            return Task.CompletedTask;
        }

        /// <summary>
        /// Stops service.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _insertUnhandledExceptionsTimer?.Change(Timeout.Infinite, 0);
            _deleteUnhandledExceptionsTimer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        #endregion

        #region Private Methods

        private async void HandleUnhandledExceptions(object state)
        {
            if (!ExceptionModels.IsEmpty)
            {
                IEnumerable<UnhandledExceptionModel> unhandledExceptionModels = ExceptionModels.DequeueRange();
                
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    await InsertUnhandledExceptions(scope, unhandledExceptionModels);
                    await AlertUnhandledExceptions(scope, unhandledExceptionModels);
                }
            }
        }

        private async void DeleteUnhandledExceptions(object state)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                IUnhandledExceptionsService unhandledExceptionsService = scope.ServiceProvider.GetRequiredService<IUnhandledExceptionsService>();
                var options = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<CleanupLogsOptions>>();

                await unhandledExceptionsService.DeleteUnhandledExceptionsOlderThan(TimeSpan.FromDays(options.Value.KeepingUnhandledExceptionsDays));
            }
        }

        private async Task InsertUnhandledExceptions(IServiceScope scope, IEnumerable<UnhandledExceptionModel> unhandledExceptions)
        {
            IUnhandledExceptionsService unhandledExceptionsService = scope.ServiceProvider.GetRequiredService<IUnhandledExceptionsService>();
            await unhandledExceptionsService.InsertUnhandledLogsAsync(unhandledExceptions);
        }

        private async Task AlertUnhandledExceptions(IServiceScope scope, IEnumerable<UnhandledExceptionModel> unhandledExceptions)
        {
            IUnhandledExceptionsSender alertService = scope.ServiceProvider.GetRequiredService<IUnhandledExceptionsSender>();
            await alertService.SendUnhandledExceptionsAsync(unhandledExceptions);
        }

        #endregion
    }
}
