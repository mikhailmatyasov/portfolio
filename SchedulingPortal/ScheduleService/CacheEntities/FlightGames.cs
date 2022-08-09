using ProxyReference;
using System.Collections.Generic;

namespace ScheduleService.CacheEntities
{
    public class FlightGames
    {
        public string FlightKey { get; init; }

        public IEnumerable<TournamentGame> Games { get; init; } = new TournamentGame[0];
    }
}
