using Model.Dto.Schedules;
using System.Threading.Tasks;

namespace ScheduleService.Services.Schedules
{
    public interface ISchedulesService
    {
        /// <summary>
        /// Gets Schedules data by organizationId and tournamentId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <returns>TournamentGames mapped data.</returns>
        Task<ScheduleDto> GetSchedulesAsync(string organizationId, string tournamentId);
    }
}
