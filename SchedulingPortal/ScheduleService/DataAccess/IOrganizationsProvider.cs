using ProxyReference;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScheduleService.DataAccess
{
    public interface IOrganizationsProvider
    {
        /// <summary>
        /// Gets organizations list.
        /// </summary>
        /// <returns>Organizations list.</returns>
        Task<IEnumerable<Organization>> GetOrganizationsAsync();
    }
}
