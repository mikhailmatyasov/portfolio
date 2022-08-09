using Model.Dto.Stats;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScheduleService.Services.Stats
{
    public interface IStatsService
    {
        /// <summary>
        /// Gets players collection data by organizationId and tournamentId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <returns>Venues.</returns>
        Task<IEnumerable<StatDto>> GetStatsAsync(string organizationId, string tournamentId);
    }
}
