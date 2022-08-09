using System;
using IdentityServer4.Models;

namespace WeSafe.Authentication.WebApi.Authorization
{
    public class ResourceOwnerClient : Client
    {
        public Type ResourceOwnerPassword { get; }

        public Type ProfileServiceType { get; }

        public ResourceOwnerClient(Type resourceOwnerPassword, Type profileServiceType)
        {
            ResourceOwnerPassword = resourceOwnerPassword;
            ProfileServiceType = profileServiceType;
        }
    }
}
