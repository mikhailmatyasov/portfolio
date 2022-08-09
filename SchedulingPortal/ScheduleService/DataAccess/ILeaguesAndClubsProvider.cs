using ScheduleService.CacheEntities;
using System.Threading.Tasks;

namespace ScheduleService.DataAccess
{
    public interface ILeaguesAndClubsProvider
    {
        /// <summary>
        /// Gets organization leagues with clubs by organizationId.
        /// </summary>
        /// <param name="organizationId">Key of the organization.</param>
        /// <returns>Organization leagues with clubs.</returns>
        Task<OrganizationLeaguesWithClubs> GetOrganizationLeaguesWithClubsAsync(string organizationId);
    }
}
