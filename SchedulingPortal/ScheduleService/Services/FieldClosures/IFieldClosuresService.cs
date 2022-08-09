using Model.Dto.FieldClosures;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScheduleService.Services.FieldClosures
{
    public interface IFieldClosuresService
    {
        /// <summary>
        /// Gets field closures data by organizationId and tournamentId.
        /// </summary>
        /// <param name="organizationId">Key of the organization that owns the tournament.</param>
        /// <param name="tournamentId">Key of the tournament.</param>
        /// <returns>FieldClosure mapped data.</returns>
        Task<IEnumerable<FieldClosuresDto>> GetFieldClosuresAsync(string organizationId, string tournamentId);
    }
}
