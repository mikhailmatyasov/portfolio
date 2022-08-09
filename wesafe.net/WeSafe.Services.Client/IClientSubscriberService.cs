using System.Collections.Generic;
using System.Threading.Tasks;
using WeSafe.Services.Client.Models;
using WeSafe.Shared.Results;

namespace WeSafe.Services.Client
{
    public interface IClientSubscriberService
    {
        Task<IEnumerable<ClientSubscriberModel>> GetClientSubscribers(int clientId);

        Task<ClientSubscriberModel> GetClientSubscriberById(int clientId, int subscriberId);

        Task<IExecutionResult> CreateClientSubscriber(int clientId, ClientSubscriberModel model);

        Task<IExecutionResult> UpdateClientSubscriber(int clientId, ClientSubscriberModel model);

        Task<IExecutionResult> RemoveClientSubscriber(int clientId, int subscriberId);

        Task<IEnumerable<AssignmentModel>> GetSubscriberAssignments(int clientId, int subscriberId);

        Task SaveSubscriberAssignments(int clientId, int subscriberId, IEnumerable<AssignmentModel> assignments);
    }
}