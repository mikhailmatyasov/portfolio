using ProxyReference;
using System.Collections.Generic;

namespace ScheduleService.CacheEntities
{
    public class OrganizationGenders
    {
        public string OrganizationId { get; init; }

        public IEnumerable<Gender> Genders { get; init; }
    }
}
