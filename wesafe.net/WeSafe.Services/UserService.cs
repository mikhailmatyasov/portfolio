using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.DAL.Entities;
using WeSafe.DAL.Extensions;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Services.Extensions;
using WeSafe.Shared;
using WeSafe.Shared.Results;
using WeSafe.Shared.Roles;

namespace WeSafe.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly UserMapper _mapper;

        public UserService(UserManager<User> userManager, UserMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<PageResponse<UserModel>> GetUsersAsync(PageRequest pageRequest)
        {
            var users = _userManager.Users
                                    .Where(c => !c.IsDeleted)
                                    .Select(_mapper.Projection)
                                    .OrderBy(c => c.UserName);

            var result = await users.ApplyPageRequest(pageRequest);
            var items = await result.Query.ToListAsync();

            foreach (var user in items)
            {
                var iUser = await _userManager.FindByIdAsync(user.Id);
                var roles = await _userManager.GetRolesAsync(iUser);

                user.RoleName = roles.Count > 0 ? roles[0] : null;

                if (await _userManager.IsLockedOutAsync(iUser))
                    user.IsLocked = true;
            }

            return new PageResponse<UserModel>
            {
                Items = items,
                Total = result.Total
            };
        }

        public async Task<UserModel> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return null;

            var result = _mapper.ToUserModel(user);
            var roles = await _userManager.GetRolesAsync(user);

            result.RoleName = roles.Count > 0 ? roles[0] : null;

            return result;
        }

        public async Task<IExecutionResult> CreateUserAsync(UserModel dto, string password, int? clientId = null)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (String.IsNullOrWhiteSpace(dto.RoleName)) dto.RoleName = UserRoles.Users;

            if (!dto.Phone.IsValidPhoneNumber())
                throw new InvalidOperationException("Phone " + dto.Phone + " is not valid");

            CheckAccess(dto);

            var user = _mapper.ToUser(dto);

            user.ClientId = clientId;

            var identityResult = await _userManager.CreateAsync(user, password);

            if (!identityResult.Succeeded) return ExecutionResult.Failed(identityResult.Errors.Select(c => c.Description));

            identityResult = await _userManager.AddToRoleAsync(user, dto.RoleName);

            if (!identityResult.Succeeded) return ExecutionResult.Failed(identityResult.Errors.Select(c => c.Description));

            return ExecutionResult.Payload(user.Id);
        }

        public async Task<IExecutionResult> UpdateUserAsync(UserModel dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            if (!dto.Phone.IsValidPhoneNumber())
                throw new InvalidOperationException("Phone " + dto.Phone + " is not valid");

            CheckAccess(dto);

            var user = await _userManager.FindByIdAsync(dto.Id);

            if (user == null) return ExecutionResult.Failed("User not found");

            var identityResult = await _userManager.UpdateAsync(_mapper.ToUser(user, dto));

            if (!identityResult.Succeeded) return ExecutionResult.Failed(identityResult.Errors.Select(c => c.Description));

            if (dto is UpsertUserModel passwordUpdateModel && !String.IsNullOrWhiteSpace(passwordUpdateModel.Password))
            {
                await SetNewPasswordAsync(user, passwordUpdateModel.Password);
            }

            var roles = await _userManager.GetRolesAsync(user);

            identityResult = await _userManager.RemoveFromRolesAsync(user, roles);

            if (!identityResult.Succeeded) return ExecutionResult.Failed(identityResult.Errors.Select(c => c.Description));

            identityResult = await _userManager.AddToRoleAsync(user, dto.RoleName);

            if (!identityResult.Succeeded) return ExecutionResult.Failed(identityResult.Errors.Select(c => c.Description));

            return ExecutionResult.Success();
        }

        public async Task<IExecutionResult> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return ExecutionResult.Failed("User with id " + userId + " is not found");

            user.IsDeleted = true;

            var identityResult = await _userManager.UpdateAsync(user);

            if (!identityResult.Succeeded) return ExecutionResult.Failed(identityResult.Errors.Select(c => c.Description));

            return ExecutionResult.Success();
        }

        public async Task<IExecutionResult> UnlockUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return ExecutionResult.Failed("User with id " + userId + " is not found");

            user.LockoutEnd = null;

            var identityResult = await _userManager.UpdateAsync(user);

            if (!identityResult.Succeeded)
                return ExecutionResult.Failed(identityResult.Errors.Select(c => c.Description));

            return ExecutionResult.Success();
        }

        private void CheckAccess(UserModel dto)
        {
            if (dto.RoleName != UserRoles.Administrators && dto.RoleName != UserRoles.Users)
                throw new Exception("Role name is wrong");
        }

        private async Task SetNewPasswordAsync(User user, string newPassword)
        {
            ValidatePassword(user, newPassword);
            await ChangePasswordAsync(user, newPassword);
        }

        private void ValidatePassword(User user, string newPassword)
        {
            var hashedPassword = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, newPassword);
            if (hashedPassword == PasswordVerificationResult.Success)
            {
                throw new ArgumentException("Password must not equal old password.");
            }
        }

        private async Task ChangePasswordAsync(User user, string newPassword)
        {
            await RemoveOldPasswordAsync(user);
            await AddNewPasswordAsync(user, newPassword);
        }

        private async Task RemoveOldPasswordAsync(User user)
        {
            var removePasswordResult = await _userManager.RemovePasswordAsync(user);
            if (!removePasswordResult.Succeeded)
            {
                throw new InvalidOperationException(removePasswordResult.Errors.Select(c => c.Description).ToString());
            }
        }

        private async Task AddNewPasswordAsync(User user, string newPassword)
        {
            var isUserHasPasswordAsync = await _userManager.HasPasswordAsync(user);
            if (!isUserHasPasswordAsync)
            {
                var changePasswordResult = await _userManager.AddPasswordAsync(user, newPassword);
                if (!changePasswordResult.Succeeded)
                {
                    throw new InvalidOperationException(changePasswordResult.Errors.Select(c => c.Description).ToString());
                }
            }
        }
    }
}