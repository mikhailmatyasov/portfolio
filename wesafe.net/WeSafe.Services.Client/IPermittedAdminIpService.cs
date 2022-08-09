using System.Collections.Generic;
using System.Threading.Tasks;
using WeSafe.Services.Client.Models;
using WeSafe.Shared.Results;

namespace WeSafe.Services.Client
{
    public interface IPermittedAdminIpService
    {
        IEnumerable<PermittedAdminIpModel> GetPermittedAdminIps();

        Task<IExecutionResult> CreatePermittedAdminIp(PermittedAdminIpModel model);

        Task<IExecutionResult> RemovePermittedAdminIp(int id);

    }
}
