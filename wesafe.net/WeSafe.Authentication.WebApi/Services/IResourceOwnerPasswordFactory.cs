using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.Authentication.WebApi.Authorization;
using WeSafe.Authentication.WebApi.Services.Abstract;

namespace WeSafe.Authentication.WebApi.Services
{
    /// <inheritdoc />
    public class ResourceOwnerPasswordFactory : IResourceOwnerPasswordFactory
    {
        private readonly IEnumerable<IResourceOwnerPassword> _resourceOwnerPasswords;

        public ResourceOwnerPasswordFactory(IEnumerable<IResourceOwnerPassword> resourceOwnerPasswords)
        {
            _resourceOwnerPasswords = resourceOwnerPasswords;
        }

        #region IResourceOwnerPasswordFactory

        /// <inheritdoc />
        public Task<IResourceOwnerPassword> CreateIResourceOwnerPassword(ResourceOwnerPasswordValidationContext context)
        {
            var client = AuthorizationConfig.Clients.OfType<ResourceOwnerClient>().FirstOrDefault(c => c.ClientId == context.Request.ClientId);
            if (client == null)
                throw new InvalidOperationException($"Client with id {context.Request.ClientId} not found");


            var selectedResourceOwner = _resourceOwnerPasswords.FirstOrDefault(r => r.GetType() == client.ResourceOwnerPassword);
            if (selectedResourceOwner == null)
                throw new InvalidOperationException($"The resource owner for client {context.Request.ClientId} not found.");

            return Task.FromResult(selectedResourceOwner);
        }

        #endregion
    }
}
