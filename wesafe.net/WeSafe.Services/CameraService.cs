using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.DAL;
using WeSafe.DAL.Entities;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Services.Extensions;
using WeSafe.Shared;
using WeSafe.Shared.Extensions;
using WeSafe.Shared.Results;
using DateTimeScheduler = WeSmart.Alpr.Core.Scheduler.DateTimeScheduler;

namespace WeSafe.Services
{
    /// <summary>
    /// Provides the APIs for managing camera in a persistence store.
    /// </summary>
    public class CameraService : BaseService, ICameraService
    {
        private bool _disposed;

        /// <summary>
        /// The <see cref="CameraSchedulerValidator"/> used to validate camera.
        /// </summary>
        protected CameraSchedulerValidator Validator { get; }

        public CameraService(WeSafeDbContext context, ILoggerFactory loggerFactory, CameraSchedulerValidator validator) : base(context, loggerFactory)
        {
            Validator = validator;
        }

        /// <summary>
        /// Releases all resources used by the camera manager.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Returns an IQueryable of cameras.
        /// </summary>
        public virtual IQueryable<Camera> Cameras => DbContext.Cameras;

        /// <summary>
        /// Gets camera specified by id as asynchronous operation.
        /// </summary>
        /// <param name="cameraId">Camera id.</param>
        /// <returns>The task that represents the asynchronous operation, containing the camera.</returns>
        public virtual Task<Camera> GetByIdAsync(int cameraId)
        {
            ThrowIfDisposed();

            return Cameras.FirstOrDefaultAsync(x => x.Id == cameraId);
        }

        /// <summary>
        /// Gets camera specified by name as asynchronous operation.
        /// </summary>
        /// <param name="name">Camera name.</param>
        /// <returns>The task that represents the asynchronous operation, containing the camera.</returns>
        public virtual Task<Camera> GetByNameAsync(string name)
        {
            ThrowIfDisposed();

            return Cameras.FirstOrDefaultAsync(x => x.CameraName == name);
        }

        /// <summary>
        /// Adds a specified camera as asynchronous operation.
        /// </summary>
        /// <param name="camera">Camera to add.</param>
        /// <returns>The task that represents the asynchronous operation, containing the added camera.</returns>
        public virtual async Task<IExecutionResult> CreateAsync(Camera camera)
        {
            ThrowIfDisposed();

            if (camera == null) throw new ArgumentNullException(nameof(camera));

            if (Validator != null)
            {
                var result = await Validator.ValidateAsync(this, camera);

                if (!result.IsSuccess) return result;
            }

            if ( camera.IsSchedulerEmpty() )
            {
                camera.Schedule = DateTimeScheduler.DefaultWeekDaysHourScheduler().Serialize();
            }

            await DbContext.Cameras.AddAsync(camera);
            await DbContext.SaveChangesAsync();

            return ExecutionResult.Payload(camera.Id);
        }

        /// <summary>
        /// Updates a specified camera as asynchronous operation.
        /// </summary>
        /// <param name="camera">Camera to update.</param>
        /// <returns>The task that represents the asynchronous operation.</returns>
        public virtual async Task<IExecutionResult> UpdateAsync(Camera camera)
        {
            ThrowIfDisposed();

            if (camera == null) throw new ArgumentNullException(nameof(camera));

            if (Validator != null)
            {
                var result = await Validator.ValidateAsync(this, camera);

                if (!result.IsSuccess) return result;
            }

            if ( camera.IsSchedulerEmpty() )
            {
                camera.Schedule = DateTimeScheduler.DefaultWeekDaysHourScheduler().Serialize();
            }

            DbContext.Attach(camera);
            DbContext.Update(camera);

            await DbContext.SaveChangesAsync();

            return new SuccessExecutionResult();
        }

        /// <summary>
        /// Deletes a camera by specified id as asynchronous operation.
        /// </summary>
        /// <param name="cameraId">Camera id to delete.</param>
        /// <returns>The task that represents the asynchronous operation.</returns>
        public virtual async Task<IExecutionResult> DeleteAsync(int cameraId)
        {
            ThrowIfDisposed();

            var camera = await GetByIdAsync(cameraId);

            if (camera == null) return ExecutionResult.Failed("Camera not found.");

            DbContext.Cameras.Remove(camera);

            await DbContext.SaveChangesAsync();

            return new SuccessExecutionResult();
        }

        public virtual async Task<IExecutionResult> CreateRangeAsync(int deviceId, IEnumerable<BaseCameraModel> cameraList)
        {
            ThrowIfDisposed();

            if (cameraList == null || !cameraList.Any())
                throw new ArgumentNullException(nameof(cameraList));

            if(deviceId < 1)
                throw new InvalidOperationException(nameof(deviceId));

            var cameras = await GetCamerasAsync(deviceId, cameraList);

            await DbContext.Cameras.AddRangeAsync(cameras);
            await DbContext.SaveChangesAsync();

            return new SuccessExecutionResult();
        }

        /// <summary>
        /// Releases the unmanaged resources used by the camera service and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                DbContext.Dispose();
                _disposed = true;
            }
        }

        /// <summary>
        /// Throws if this class has been disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        private async Task<IEnumerable<Camera>> GetCamerasAsync(int deviceId, IEnumerable<BaseCameraModel> cameraList)
        {
            var cameras = new List<Camera>();

            foreach (var cameraModel in cameraList)
            {
                var camera = new Camera()
                {
                    DeviceId = deviceId,
                    CameraName = !string.IsNullOrEmpty(cameraModel.CameraName) ? cameraModel.CameraName : "Unnamed",
                    Ip = cameraModel.Ip,
                    Port = cameraModel.Port,
                    Login = cameraModel.Login,
                    Password = cameraModel.Password.Encrypt(),
                    SpecificRtcpConnectionString = cameraModel.SpecificRtcpConnectionString.Encrypt(),
                    IsActive = cameraModel.IsActive,
                    Roi = cameraModel.Roi,
                    Schedule = cameraModel.Schedule,
                    RecognitionSettings = cameraModel.RecognitionSettings
                };

                if (Validator != null)
                {
                    var result = await Validator.ValidateAsync(this, camera);

                    if (!result.IsSuccess)
                        continue;
                }

                if ( camera.IsSchedulerEmpty() )
                {
                    camera.Schedule = DateTimeScheduler.DefaultWeekDaysHourScheduler().Serialize();
                }

                cameras.Add(camera);
            }

            return cameras;
        }
    }
}