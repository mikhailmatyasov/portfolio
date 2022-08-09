using IdentityServer4.Models;
using IdentityServer4.Services;
using System;
using System.Threading.Tasks;
using WeSafe.Authentication.WebApi.Services.Abstract;

namespace WeSafe.Authentication.WebApi.Services
{
    public class ProfileService : IProfileService
    {
        private readonly ICustomProfileServiceFactory _customProfileServiceFactory;

        public ProfileService(ICustomProfileServiceFactory customProfileServiceFactory)
        {
            _customProfileServiceFactory = customProfileServiceFactory;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var profileService = await GetCustomProfileService(context.Client.ClientId);

            await profileService.GetProfileData(context);

        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var profileService = await GetCustomProfileService(context.Client.ClientId);

            await profileService.IsActive(context);
        }

        private async Task<ICustomProfileService> GetCustomProfileService(string clientId)
        {
            var profileService = await _customProfileServiceFactory.CreateCustomProfileService(clientId);
            if (profileService == null)
                throw new InvalidOperationException("The profile service not found.");

            return profileService;
        }
    }
}
