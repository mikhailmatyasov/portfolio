using ScheduleService.CacheEntities;
using System.Threading.Tasks;

namespace ScheduleService.DataAccess
{
    public interface ITournamentVenuesProvider
    {
        /// <summary>
        /// Gets tournament venues by organizationId and tournamentId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <returns>Tournament venues.</returns>
        Task<TournamentVenues> GetTournamentVenuesAsync(string organizationId, string tournamentId);
    }
}
