using Model.Dto.Common;
using System.Threading.Tasks;

namespace ScheduleService.Services.Common
{
    public interface IPageHeaderService
    {
        /// <summary>
        /// Gets data for page header by organizationId and tournamentId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <returns>Page header data.</returns>
        Task<PageHeaderDto> GetPageHeaderDataAsync(string organizationId, string tournamentId);
    }
}
