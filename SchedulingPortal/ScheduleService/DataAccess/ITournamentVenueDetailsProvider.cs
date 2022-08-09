using ProxyReference;
using System.Threading.Tasks;

namespace ScheduleService.DataAccess
{
    public interface ITournamentVenueDetailsProvider
    {
        /// <summary>
        /// Gets tournament data by organizationId and tournamentId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="venueId">Key of the venue.</param>
        /// <returns>TournamentVenue.</returns>
        Task<TournamentVenue> GetVenueDetailsAsync(string organizationId, string venueId);
    }
}
