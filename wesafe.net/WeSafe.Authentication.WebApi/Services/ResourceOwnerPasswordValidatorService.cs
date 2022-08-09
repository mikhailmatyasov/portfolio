using IdentityServer4.Validation;
using System.Threading.Tasks;
using WeSafe.Authentication.WebApi.Services.Abstract;

namespace WeSafe.Authentication.WebApi.Services
{
    public class ResourceOwnerPasswordValidatorService : IResourceOwnerPasswordValidator
    {
        private readonly IResourceOwnerPasswordFactory _resourceOwnerPasswordFactory;

        public ResourceOwnerPasswordValidatorService(IResourceOwnerPasswordFactory resourceOwnerPasswordFactory)
        {
            _resourceOwnerPasswordFactory = resourceOwnerPasswordFactory;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var resourceOwnerPassword = await _resourceOwnerPasswordFactory.CreateIResourceOwnerPassword(context);

            await resourceOwnerPassword.Validate(context);
        }
    }
}
