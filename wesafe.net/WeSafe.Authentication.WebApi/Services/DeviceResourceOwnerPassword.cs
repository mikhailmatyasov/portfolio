using Arch.EntityFrameworkCore.UnitOfWork;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WeSafe.Authentication.WebApi.Services.Abstract;
using WeSafe.DAL.Extensions;

namespace WeSafe.Authentication.WebApi.Services
{
    /// <inheritdoc />
    public class DeviceResourceOwnerPassword : IResourceOwnerPassword
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeviceResourceOwnerPassword> _logger;

        public DeviceResourceOwnerPassword(IUnitOfWork unitOfWork, ILogger<DeviceResourceOwnerPassword> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region IResourceOwnerPassword

        /// <inheritdoc />
        public async Task Validate(ResourceOwnerPasswordValidationContext context)
        {
            var device = await _unitOfWork.GetDeviceRepository().FindByMacAddressAsync(context.UserName);
            if (device == null)
            {
                _logger.LogInformation($"Authentication failed for username: {context.UserName}");
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
            }

            context.Result = new GrantValidationResult(device.MACAddress, OidcConstants.AuthenticationMethods.Password);
        }

        #endregion
    }
}
