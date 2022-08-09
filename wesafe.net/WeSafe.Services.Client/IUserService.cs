using System.Threading.Tasks;
using WeSafe.Services.Client.Models;
using WeSafe.Shared;
using WeSafe.Shared.Results;

namespace WeSafe.Services.Client
{
    public interface IUserService
    {
        Task<PageResponse<UserModel>> GetUsersAsync(PageRequest pageRequest);

        Task<UserModel> GetUserByIdAsync(string userId);

        Task<IExecutionResult> CreateUserAsync(UserModel dto, string password, int? clientId = null);

        Task<IExecutionResult> UpdateUserAsync(UserModel dto);

        Task<IExecutionResult> DeleteUserAsync(string userId);

        Task<IExecutionResult> UnlockUser(string userId);
    }
}