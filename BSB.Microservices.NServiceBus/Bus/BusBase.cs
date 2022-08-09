using Microsoft.Extensions.Logging;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BSB.Microservices.NServiceBus
{
    public abstract class BusBase : AsyncDisposable, IRestartableBus
    {
        public Task RecoveryTask { get; private set; }

        public BusStatus Status { get; protected set; } = BusStatus.Stopped;

        public EndpointData Endpoint { get; protected set; }

        protected Guid EndpointId { get; set; }

        protected IEndpointInstance EndpointInstance => _endpointManager.Get(EndpointId);

        protected readonly ILogger Logger;

        private bool HasStarted { get; set; }

        private List<Task<Task>> StartupTasks { get; set; }

        private CancellationTokenSource CancellationTokenSource { get; set; }

        private readonly IEndpointManager _endpointManager;
        private readonly IBusRecoveryManager _recoveryManager;
        private readonly IEnumerable<IBusStartupService> _startupServices;

        protected BusBase(
            EndpointData endpoint,
            ILogger logger,
            IEndpointManager endpointManager,
            IBusRecoveryManager recoveryManager,
            IEnumerable<IBusStartupService> startupServices)
        {
            Endpoint = endpoint;
            Logger = logger;

            HasStarted = false;

            _endpointManager = endpointManager;
            _recoveryManager = recoveryManager;
            _startupServices = startupServices;
        }

        public virtual async Task StartAsync()
        {
            ThrowIfDisposed();

            if(Status == BusStatus.Started || Status == BusStatus.Started)
            {
                return;
            }

            CancellationTokenSource = new CancellationTokenSource();

            Status = Status == BusStatus.Restarting ? Status : BusStatus.Starting;

            if(EndpointId == Guid.Empty)
            {
                EndpointId = await _endpointManager.StartAsync(Endpoint);
            }
            else
            {
                await _endpointManager.StartAsync(EndpointId, Endpoint);
            }

            if (!HasStarted)
            {
                StartupTasks = _startupServices.Distinct().ToList()
                    .Select(x => Task.Factory.StartNew(() => x.StartAsync(CancellationTokenSource.Token)))
                    .ToList();

                Logger.LogInformation(default(EventId), $"{GetType().Name} Started.");
            }

            RecoveryTask = _recoveryManager.StartEndpointRecoveryMonitorAsync(
                Endpoint,
                RotateEndpointAsync,
                CancellationTokenSource.Token);

            HasStarted = true;
            Status = BusStatus.Started;
        }

        public virtual async Task StopAsync()
        {
            ThrowIfDisposed();

            await CancelTasks();

            if (Status == BusStatus.Stopping || Status == BusStatus.Stopped)
            {
                return;
            }

            Status = Status == BusStatus.Restarting ? Status : BusStatus.Stopping;

            await _endpointManager.StopAsync(EndpointId);

            Status = Status == BusStatus.Restarting ? Status : BusStatus.Stopped;
        }

        public virtual async Task RestartAsync()
        {
            ThrowIfDisposed();

            Status = BusStatus.Restarting;

            await StopAsync();
            await StartAsync();

            Status = BusStatus.Started;
        }

        protected override async Task DisposeAsync()
        {
            Status = BusStatus.Stopping;

            await CancelTasks();

            await _endpointManager.StopAsync(EndpointId);

            Status = BusStatus.Stopped;
        }

        private async Task CancelTasks()
        {
            if (CancellationTokenSource != null && !CancellationTokenSource.IsCancellationRequested)
            {
                CancellationTokenSource.Cancel();
            }

            if (RecoveryTask != null)
            {
                await RecoveryTask;
            }

            if(StartupTasks != null)
            {
                await Task.WhenAll(StartupTasks);
            }
        }

        protected async Task RotateEndpointAsync(Guid endpointId)
        {
            var oldEndpointId = EndpointId;

            EndpointId = endpointId;

            await _endpointManager.DisposeEndpointAsync(oldEndpointId);
        }
    }
}
