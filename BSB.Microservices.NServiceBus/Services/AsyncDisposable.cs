using System;
using System.Threading.Tasks;

namespace BSB.Microservices.NServiceBus
{
    /// <summary>
    /// Provides a mechanism for releasing unmanged resources asynchronously.
    /// </summary>
    public abstract class AsyncDisposable : IDisposable
    {
        /// <summary>
        /// Returns true if the object is disposing.
        /// </summary>
        public bool IsDisposing { get; protected set; }

        /// <summary>
        /// Returns true if the object has completed disposing.
        /// </summary>
        public bool IsDisposed { get; protected set; }

        /// <summary>
        /// Releases unmanged resources
        /// </summary>
        public void Dispose()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases unmanged resources
        /// </summary>
        public void Dispose(bool waitForCompletion)
        {
            var dispose = Task.Run(async () =>
            {
                IsDisposing = true;

                await DisposeAsync();

                IsDisposed = true;
            });

            if (waitForCompletion)
            {
                dispose.ConfigureAwait(true).GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// Releases unmanged resources
        /// </summary>
        protected abstract Task DisposeAsync();

        /// <summary>
        /// Throws <see cref="ObjectDisposedException" /> if the object has called <see cref="Dispose(bool)"/> or <see cref="DisposeAsync"/>
        /// </summary>
        public void ThrowIfDisposed()
        {
            if (IsDisposing || IsDisposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }
    }
}
