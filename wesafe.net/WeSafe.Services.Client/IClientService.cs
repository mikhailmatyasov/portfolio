using System.Threading.Tasks;
using WeSafe.Services.Client.Models;
using WeSafe.Shared;
using WeSafe.Shared.Results;

namespace WeSafe.Services.Client
{
    public interface IClientService
    {
        Task<PageResponse<ClientModel>> GetClients(PageRequest request);

        Task<ClientModel> GetClientById(int clientId);

        Task<IExecutionResult> CreateClient(ClientModel model);

        Task<IExecutionResult> UpdateClient(ClientModel model);

        Task<IExecutionResult> RemoveClient(int deviceId);

        Task<IExecutionResult> SignUpClient(ClientModel model, string timeZone, string password);

        Task<SystemSettingsModel> GetSystemSettings(int clientId);

        Task<PageResponse<CameraLogModel>> GetEvents(int clientId, EventBaseRequest request);
    }
}