using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WeSafe.DAL.Entities;
using WeSafe.Shared.Roles;
using WeSafe.Web.Common.Authentication.Abstract;

namespace WeSafe.Web.Common.Authentication
{
    public class WeSafeUserManager : IUserManager
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WeSafeUserManager(UserManager<User> userManager,
            SignInManager<User> signInManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<bool> IsAdmin(User user)
        {
            var role = await GetUserRole(user);

            return role == UserRoles.Administrators;
        }

        public string GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        public Task<User> GetUserAsync(ClaimsPrincipal claims)
        {
            return _userManager.GetUserAsync(claims);
        }

        public Task<User> FindByNameAsync(string name)
        {
            return _userManager.FindByNameAsync(name);
        }

        public Task<IList<string>> GetRolesAsync(User user)
        {
            return _userManager.GetRolesAsync(user);
        }

        public Task<SignInResult> CheckPasswordSignInAsync(User user, string password, bool isAdmin)
        {
            return _signInManager.CheckPasswordSignInAsync(user, password, isAdmin);
        }

        public async Task<string> CreateAsync(User user, string password, string role)
        {
            if ( user == null )
            {
                throw new ArgumentNullException(nameof(user));
            }

            if ( String.IsNullOrWhiteSpace(password) )
            {
                throw new ArgumentNullException(nameof(password));
            }

            if ( String.IsNullOrWhiteSpace(role) )
            {
                throw new ArgumentNullException(nameof(role));
            }

            var result = await _userManager.CreateAsync(user, password);

            if ( !result.Succeeded )
            {
                throw new InvalidOperationException($"{String.Join(",", result.Errors.Select(x => x.Description).ToList())}");
            }

            result = await _userManager.AddToRoleAsync(user, role);

            if ( !result.Succeeded )
            {
                throw new InvalidOperationException($"{String.Join(",", result.Errors.Select(x => x.Description).ToList())}");
            }

            return user.Id;
        }

        public async Task<string> GetUserRole(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Any())
                throw new InvalidOperationException($"The user {user.UserName} has no roles.");

            if (roles.Count > 1)
                throw new InvalidOperationException($"The user {user.UserName} has more that 1 role.");

            return roles.Single();
        }
    }
}
