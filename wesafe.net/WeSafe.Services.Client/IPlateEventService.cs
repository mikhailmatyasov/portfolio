using System.Collections.Generic;
using System.Threading.Tasks;
using WeSafe.Services.Client.Models;
using WeSafe.Shared;

namespace WeSafe.Services.Client
{
    /// <summary>
    /// Represents plate event interface.
    /// </summary>
    public interface IPlateEventService
    {
        /// <summary>
        /// Adds plate event to storage.
        /// </summary>
        /// <param name="eventModel">Plate event model.</param>
        /// <returns>Task.</returns>
        Task AddPlateEventAsync(PlateEventModel eventModel);

        /// <summary>
        /// Gets filtered plate events collection.
        /// </summary>
        /// <param name="deviceId">The plate event device identifier.</param>
        /// <param name="searchModel">The search model.</param>
        /// <returns>The plate events collection.</returns>
        Task<PageResponse<PlateEventDisplayModel>> GetPlateEventsAsync(int deviceId, PlateEventSearchModel searchModel);
    }
}
