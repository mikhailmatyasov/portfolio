using Model.Dto.ClubStandings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScheduleService.Services.ClubStandings
{
    public interface IClubStandingsService
    {
        /// <summary>
        /// Gets Club Standings data by organizationId and tournamentId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <returns>ClubStandings mapped data.</returns>
        Task<IEnumerable<ClubStandingsDto>> GetClubStandingsAsync(string organizationId, string tournamentId);
    }
}
