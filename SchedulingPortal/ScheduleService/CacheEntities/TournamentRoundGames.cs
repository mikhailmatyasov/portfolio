using ProxyReference;
using System.Collections.Generic;

namespace ScheduleService.CacheEntities
{
    public class TournamentRoundGames
    {
        public string RoundIdKey { get; init; }

        public IEnumerable<TournamentGame> Games { get; init; } = new TournamentGame[0];
    }
}
