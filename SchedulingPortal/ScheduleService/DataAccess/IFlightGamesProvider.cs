using ScheduleService.CacheEntities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScheduleService.DataAccess
{
    public interface IFlightGamesProvider
    {
        /// <summary>
        /// Gets flight games list by organizationId, tournamentId and flightIds.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <param name="flightIds">List of the flights with flight updated info.</param>
        /// <returns>Flight games list.</returns>
        Task<IEnumerable<FlightGames>> GetFlightGamesListAsync(
            string organizationId,
            string tournamentId,
            IEnumerable<KeyValuePair<string, DateTime>> flightIds);

        /// <summary>
        /// Gets flight games list by organizationId, tournamentId and flightId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <param name="flightId">Key of the flight.</param>
        /// <param name="updated">Flight last updated.</param>
        /// <returns>Flight games list.</returns>
        Task<FlightGames> GetFlightGamesAsync(
            string organizationId,
            string tournamentId,
            string flightId,
            DateTime updated);
    }
}
