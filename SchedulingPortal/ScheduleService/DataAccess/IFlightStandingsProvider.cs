using ProxyReference;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScheduleService.DataAccess
{
    public interface IFlightStandingsProvider
    {
        /// <summary>
        /// Gets flight standings list by organizationId, tournamentId and flightIds.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <param name="flightIds">List of the flights with flight updated info.</param>
        /// <returns>Flight standings list.</returns>
        Task<IEnumerable<TournamentFlightStandings>> GetFlightStandingsListAsync(
            string organizationId,
            string tournamentId,
            IEnumerable<KeyValuePair<string, DateTime>> flightIds);

        /// <summary>
        /// Gets flight standings list by organizationId, tournamentId and flightId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <param name="flightId">Key list of the flight.</param>
        /// <param name="updated">Flight last updated.</param>
        /// <returns>Flight standings.</returns>
        Task<TournamentFlightStandings> GetFlightStandingsAsync(
            string organizationId,
            string tournamentId,
            string flightId,
            DateTime updated);
    }
}
