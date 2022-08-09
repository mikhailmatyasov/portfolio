using Model.Dto.VenueDetails;
using System.Threading.Tasks;

namespace ScheduleService.Services.VenueDetails
{
    public interface IVenueDetailsService
    {
        /// <summary>
        /// Gets venue details data by organizationId and venueId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="venueId">Key of the venue.</param>
        /// <returns>Venue details mapped data.</returns>
        Task<VenueDetailsDto> GetVenueDetailsServiceAsync(string organizationId, string venueId);
    }
}
