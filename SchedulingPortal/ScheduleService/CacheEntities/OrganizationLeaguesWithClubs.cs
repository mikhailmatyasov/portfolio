using ProxyReference;
using System.Collections.Generic;

namespace ScheduleService.CacheEntities
{
    public class OrganizationLeaguesWithClubs
    {
        public string OrganizationId { get; init; }

        public IEnumerable<LeagueWithClubs> LeagueWithClubs { get; init; }
    }
}
