using ProxyReference;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScheduleService.DataAccess
{
    public interface ITournamentTopScorerProvider
    {
        /// <summary>
        /// Gets players data w/ goals data by organizationId, tournamentId and flightIds.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <param name="flightIds">Keys of the Flights/AgeGroups.</param>
        /// <returns>Collection of TournamentTopScorer.</returns>
        Task<IEnumerable<TournamentTopScorer>> GetPlayersWithScoresAsync(
            string organizationId,
            string tournamentId,
            IEnumerable<string> flightIds);
    }
}
