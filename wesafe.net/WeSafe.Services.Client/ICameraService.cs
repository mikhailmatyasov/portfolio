using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.DAL.Entities;
using WeSafe.Services.Client.Models;
using WeSafe.Shared.Results;

namespace WeSafe.Services.Client
{
    /// <summary>
    /// Provides the APIs for managing camera in a persistence store.
    /// </summary>
    public interface ICameraService
    {
        /// <summary>
        /// Returns an IQueryable of cameras.
        /// </summary>
        IQueryable<Camera> Cameras { get; }

        /// <summary>
        /// Gets camera specified by id as asynchronous operation.
        /// </summary>
        /// <param name="cameraId">Camera id.</param>
        /// <returns>The task that represents the asynchronous operation, containing the camera.</returns>
        Task<Camera> GetByIdAsync(int cameraId);

        /// <summary>
        /// Gets camera specified by name as asynchronous operation.
        /// </summary>
        /// <param name="name">Camera name.</param>
        /// <returns>The task that represents the asynchronous operation, containing the camera.</returns>
        Task<Camera> GetByNameAsync(string name);

        /// <summary>
        /// Adds a specified camera as asynchronous operation.
        /// </summary>
        /// <param name="camera">Camera to add.</param>
        /// <returns>The task that represents the asynchronous operation, containing the added camera.</returns>
        Task<IExecutionResult> CreateAsync(Camera camera);

        /// <summary>
        /// Updates a specified camera as asynchronous operation.
        /// </summary>
        /// <param name="camera">Camera to update.</param>
        /// <returns>The task that represents the asynchronous operation.</returns>
        Task<IExecutionResult> UpdateAsync(Camera camera);

        /// <summary>
        /// Deletes a camera by specified id as asynchronous operation.
        /// </summary>
        /// <param name="cameraId">Camera id to delete.</param>
        /// <returns>The task that represents the asynchronous operation.</returns>
        Task<IExecutionResult> DeleteAsync(int cameraId);

        Task<IExecutionResult> CreateRangeAsync(int deviceId, IEnumerable<BaseCameraModel> cameraList);
    }
}