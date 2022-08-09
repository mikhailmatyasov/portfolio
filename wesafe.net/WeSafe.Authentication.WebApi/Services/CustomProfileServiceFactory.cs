using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.Authentication.WebApi.Authorization;
using WeSafe.Authentication.WebApi.Services.Abstract;

namespace WeSafe.Authentication.WebApi.Services
{
    /// <inheritdoc />
    public class CustomProfileServiceFactory : ICustomProfileServiceFactory
    {
        private readonly IEnumerable<ICustomProfileService> _customProfileServices;

        public CustomProfileServiceFactory(IEnumerable<ICustomProfileService> customProfileServices)
        {
            _customProfileServices = customProfileServices;
        }

        #region ICustomProfileServiceFactory

        /// <inheritdoc />
        public Task<ICustomProfileService> CreateCustomProfileService(string clientId)
        {
            var client = AuthorizationConfig.Clients.OfType<ResourceOwnerClient>().FirstOrDefault(c => c.ClientId == clientId);
            if (client == null)
                throw new InvalidOperationException($"Client with id {clientId} not found");


            var selectedCustomProfileService = _customProfileServices.FirstOrDefault(r => r.GetType() == client.ProfileServiceType);
            if (selectedCustomProfileService == null)
                throw new InvalidOperationException($"The resource owner for client {clientId} not found.");

            return Task.FromResult(selectedCustomProfileService);
        }

        #endregion
    }
}
