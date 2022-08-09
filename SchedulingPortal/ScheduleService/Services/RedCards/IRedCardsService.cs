using Model.Dto.RedCards;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScheduleService.Services.RedCards
{
    public interface IRedCardsService
    {
        /// <summary>
        /// Gets red cards data by organizationId and tournamentId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <returns>Red cards data.</returns>
        Task<IEnumerable<GenderRedCardsDto>> GetRedCardsAsync(string organizationId, string tournamentId);
    }
}
