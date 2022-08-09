using Model.Dto.Schedules;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScheduleService.Services.Standings
{
    public interface IStandingsService
    {
        /// <summary>
        /// Gets Standings data by organizationId, tournamentId and flightId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <returns>Standings.</returns>
        Task<IEnumerable<StandingsBracketGroupedDto>> GetStandingsAsync(string organizationId, string tournamentId);
    }
}
