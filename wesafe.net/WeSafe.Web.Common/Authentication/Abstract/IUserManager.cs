using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WeSafe.DAL.Entities;

namespace WeSafe.Web.Common.Authentication.Abstract
{
    public interface IUserManager
    {
        Task<bool> IsAdmin(User user);

        Task<string> GetUserRole(User user);

        string GetCurrentUserId();

        Task<User> GetUserAsync(ClaimsPrincipal claims);

        Task<User> FindByNameAsync(string name);

        Task<IList<string>> GetRolesAsync(User user);

        Task<SignInResult> CheckPasswordSignInAsync(User user, string password, bool isAdmin);

        Task<string> CreateAsync(User user, string password, string role);
    }
}
