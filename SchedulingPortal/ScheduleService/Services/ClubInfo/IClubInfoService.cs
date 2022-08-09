using Model.Dto.ClubInfo;
using System.Threading.Tasks;

namespace ScheduleService.Services.ClubInfo
{
    public interface IClubInfoService
    {
        /// <summary>
        /// Gets Club Info by organizationId, tournamentId and clubId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <param name="clubId">Key of the club.</param>
        /// <returns>Club Info.</returns>
        public Task<ClubInfoDto> GetClubInfoAsync(string organizationId, string tournamentId, string clubId);
    }
}
