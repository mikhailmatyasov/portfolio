using Model.Dto.Venues;
using System.Threading.Tasks;

namespace ScheduleService.Services.Venues
{
    public interface IVenuesService
    {
        /// <summary>
        /// Gets venues data by organizationId and tournamentId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <returns>Venues.</returns>
        Task<VenuesDto> GetVenuesAsync(
            string organizationId,
            string tournamentId);
    }
}
