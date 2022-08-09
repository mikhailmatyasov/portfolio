using ProxyReference;
using System.Collections.Generic;

namespace ScheduleService.CacheEntities
{
    public class TournamentVenues
    {
        public string TournamentKey { get; init; }

        public IEnumerable<TournamentVenue> Venues { get; init; } = new TournamentVenue[0];
    }
}
