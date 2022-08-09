using Model.Dto.Tournament;
using System.Threading.Tasks;

namespace ScheduleService.Services.Tournament
{
    public interface ITournamentService
    {
        /// <summary>
        /// Gets tournament data by organizationId and tournamentId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <returns>Tournament info.</returns>
        Task<TournamentDto> GetTournamentAsync(string organizationId, string tournamentId);
    }
}
