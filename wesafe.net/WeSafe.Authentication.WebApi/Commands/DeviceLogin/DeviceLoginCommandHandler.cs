using System;
using System.Threading;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using MediatR;
using WeSafe.Authentication.WebApi.Models;
using WeSafe.Authentication.WebApi.Services.Abstract;
using WeSafe.DAL.Entities;
using WeSafe.DAL.Extensions;
using WeSafe.Shared.Roles;
using WeSafe.Web.Common.Authentication;

namespace WeSafe.Authentication.WebApi.Commands.DeviceLogin
{
    /// <summary>
    /// Represents a device login operation handler.
    /// </summary>
    public class DeviceLoginCommandHandler : IRequestHandler<DeviceLoginCommand, TokenResponseModel>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthTokenGenerator _authTokenGenerator;

        #endregion

        #region Constructors

        public DeviceLoginCommandHandler(IUnitOfWork unitOfWork, IAuthTokenGenerator authTokenGenerator)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _authTokenGenerator = authTokenGenerator ?? throw new ArgumentNullException(nameof(authTokenGenerator));
        }

        #endregion

        #region IRequestHandler implementation

        public async Task<TokenResponseModel> Handle(DeviceLoginCommand request, CancellationToken cancellationToken)
        {
            var device = await _unitOfWork.GetDeviceRepository().FindByMacAddressAsync(request.MacAddress);
            var token = CreateTokenResponse(device);

            device.AuthToken = token.AccessToken;

            await _unitOfWork.SaveChangesAsync();

            return token;
        }

        #endregion

        #region Private members

        private TokenResponseModel CreateTokenResponse(Device device)
        {
            var expiresAt = DateTime.UtcNow.Add(AuthOptions.LifetimeDevice);

            return _authTokenGenerator.CreateToken(new TokenRequestModel(device.MACAddress, device.Name, expiresAt)
            {
                Role = UserRoles.Devices
            });
        }

        #endregion
    }
}