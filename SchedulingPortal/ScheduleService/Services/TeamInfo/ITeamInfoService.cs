using Model.Dto.TeamInfo;
using System.Threading.Tasks;

namespace ScheduleService.Services.TeamInfo
{
    public interface ITeamInfoService
    {
        /// <summary>
        /// Gets Team Info by organizationId and tournamentId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <param name="teamId">Key of the team.</param>
        /// <returns>Team Info.</returns>
        public Task<TeamInfoDto> GetTeamInfoAsync(string organizationId, string tournamentId, string teamId);
    }
}
