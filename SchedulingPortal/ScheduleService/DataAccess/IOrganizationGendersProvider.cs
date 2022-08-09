using ScheduleService.CacheEntities;
using System.Threading.Tasks;

namespace ScheduleService.DataAccess
{
    public interface IOrganizationGendersProvider
    {
        /// <summary>
        /// Gets organization genders by organizationId.
        /// </summary>
        /// <param name="organizationId">Key of the organization.</param>
        /// <returns>OrganizationGenders.</returns>
        Task<OrganizationGenders> GetOrganizationGendersAsync(string organizationId);
    }
}
