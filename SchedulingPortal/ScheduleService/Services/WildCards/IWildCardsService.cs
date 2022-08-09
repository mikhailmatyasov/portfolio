using Model.Dto.WildCards;
using System.Threading.Tasks;

namespace ScheduleService.Services.WildCards
{
    public interface IWildCardsService
    {
        /// <summary>
        /// Gets Wild Cards data by organizationId, tournamentId and flightId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <returns>Wild Cards.</returns>
        Task<WildCardsDto> GetWildCardsAsync(string organizationId, string tournamentId);
    }
}
