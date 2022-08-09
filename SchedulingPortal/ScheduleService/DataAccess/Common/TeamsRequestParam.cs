using System.Collections.Generic;

namespace ScheduleService.DataAccess.Common
{
    public class TeamsRequestParam
    {
        public string LeagueKey { get; set; }

        public IEnumerable<string> TeamKeys { get; set; }
    }
}
