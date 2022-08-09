using Arch.EntityFrameworkCore.UnitOfWork;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WeSafe.Authentication.WebApi.Services.Abstract;
using WeSafe.DAL.Extensions;
using WeSafe.Shared.Roles;
using WeSafe.Web.Common.Authentication;

namespace WeSafe.Authentication.WebApi.Services
{
    /// <inheritdoc />
    public class DeviceCustomProfileService : ICustomProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeviceCustomProfileService> _logger;

        public DeviceCustomProfileService(IUnitOfWork unitOfWork, ILogger<DeviceCustomProfileService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region ICustomProfileService

        /// <inheritdoc />
        public async Task GetProfileData(ProfileDataRequestContext context)
        {
            var device = await _unitOfWork.GetDeviceRepository().FindByMacAddressAsync(context.Subject.GetSubjectId());
            var claims = new List<Claim>
            {
                new Claim(WeSafeClaimTypes.UserDisplayNameClaimType, device.Name),
                new Claim(JwtClaimTypes.Role, UserRoles.Devices)
            };

            context.IssuedClaims.AddRange(claims);
        }

        /// <inheritdoc />
        public Task IsActive(IsActiveContext context)
        {
            return Task.FromResult(true);
        }

        #endregion
    }
}
