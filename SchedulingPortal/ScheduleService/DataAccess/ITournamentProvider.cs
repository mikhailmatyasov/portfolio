using ProxyReference;
using System.Threading.Tasks;

namespace ScheduleService.DataAccess
{
    public interface ITournamentProvider
    {
        /// <summary>
        /// Gets tournament data by organizationId and tournamentId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <returns>Tournament.</returns>
        Task<Tournament> GetTournamentAsync(string organizationId, string tournamentId);
    }
}
