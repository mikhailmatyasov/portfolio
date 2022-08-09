using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;
using WeSafe.Authentication.WebApi.Services;
using WeSafe.Web.Common.Authentication;

namespace WeSafe.Authentication.WebApi.Authorization
{
    public static class AuthorizationConfig
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("desktop", "WEB Scope"),
                new ApiScope("mobile", "Mobile Scope"),
                new ApiScope("device", "Device Scope")
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new ResourceOwnerClient(typeof(WebResourceOwnerPassword), typeof(WebCustomProfileService))
                {
                    ClientId = "web",
                    ClientSecrets = { new Secret("119d5c73-82cd-4afe-bdcc-992fceca80cb".Sha256()) },

                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        "desktop"
                    },
                    AccessTokenLifetime = int.Parse(AuthOptions.Lifetime.TotalSeconds.ToString())
                },

                new ResourceOwnerClient(typeof(MobileResourceOwnerPassword), typeof(MobileCustomProfileService))
                {
                    ClientId = "mobile",
                    ClientSecrets = { new Secret("4141fb45-316d-45cd-86f1-470dc44d322a".Sha256()) },

                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        "mobile"
                    },
                    AccessTokenLifetime = int.Parse(AuthOptions.LifetimeMobile.TotalSeconds.ToString())
                },

                new ResourceOwnerClient(typeof(DeviceResourceOwnerPassword), typeof(DeviceCustomProfileService))
                {
                    ClientId = "device",
                    ClientSecrets = { new Secret("402a5fbd-c041-4b3c-b81f-b6c2370577d0".Sha256()) },

                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        "device"
                    },
                    AccessTokenLifetime = int.Parse(AuthOptions.LifetimeDevice.TotalSeconds.ToString())
                }
            };
    }
}
