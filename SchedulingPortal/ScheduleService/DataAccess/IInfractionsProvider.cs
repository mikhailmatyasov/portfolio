using ProxyReference;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScheduleService.DataAccess
{
    public interface IInfractionsProvider
    {
        /// <summary>
        /// Gets infractions list by organizationId, tournamentId and flightIds.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <param name="flightIds">Key list of the flights.</param>
        /// <returns>Infractions list.</returns>
        Task<IEnumerable<TournamentInfraction>> GetTournamentInfractionsAsync(
            string organizationId,
            string tournamentId,
            IEnumerable<string> flightIds);
    }
}
