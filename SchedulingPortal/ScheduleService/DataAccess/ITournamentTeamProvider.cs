using ProxyReference;
using System.Threading.Tasks;

namespace ScheduleService.DataAccess
{
    public interface ITournamentTeamProvider
    {
        /// <summary>
        /// Gets tournament team data by organizationId, tournamentId and teamId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <param name="teamId">Key of the team.</param>
        /// <returns>Tournament team.</returns>
        Task<TeamWithAdminsPlayers3> GetTournamentTeamAsync(string organizationId, string tournamentId, string teamId);
    }
}
