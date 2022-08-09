using System.Threading.Tasks;

namespace ScheduleService.Services.Common
{
    public interface IOrganizationService
    {
        Task<string> GetOrganizationLogoUrlAsync(string organizationId);
    }
}
