using IdentityModel;
using Microsoft.AspNetCore.Http;
using System;
using WeSafe.Shared.Roles;
using WeSafe.Web.Common.Authentication.Abstract;

namespace WeSafe.Web.Common.Authentication
{
    public class WeSafeCurrentUser : ICurrentUser
    {
        #region Fields

        private readonly IHttpContextAccessor _httpContextAccessor;
        private string _userId;
        private string _roleName;
        private int? _clientId;
        private bool _clientIdProcessed;

        #endregion

        #region Constructors

        public WeSafeCurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        #endregion

        #region ICurrentUser implementation

        public string Id => _userId ??= _httpContextAccessor.HttpContext.User.FindFirst(WeSafeClaimTypes.UserIdClaimType).Value;

        public string Role => _roleName ??= _httpContextAccessor.HttpContext.User.FindFirst(JwtClaimTypes.Role).Value;

        public int? ClientId
        {
            get
            {
                if (!_clientIdProcessed)
                {
                    var claim = _httpContextAccessor.HttpContext.User.FindFirst(WeSafeClaimTypes.ClientIdClaimType);

                    if (claim != null)
                    {
                        if (!Int32.TryParse(claim.Value, out int value))
                        {
                            throw new InvalidCastException($"ClientId value '{claim.Value}' is not valid integer.");
                        }

                        _clientId = value;
                        _clientIdProcessed = true;
                    }
                }

                return _clientId;
            }
        }

        public bool IsInRole(string role)
        {
            return Role == role;
        }

        public bool IsAdmin()
        {
            return IsInRole(UserRoles.Administrators);
        }

        #endregion
    }
}