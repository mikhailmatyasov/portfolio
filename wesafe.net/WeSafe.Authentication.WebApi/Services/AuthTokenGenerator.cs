using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WeSafe.Authentication.WebApi.Models;
using WeSafe.Authentication.WebApi.Services.Abstract;
using WeSafe.Web.Common.Authentication;
using WeSafe.Web.Common.Exceptions;

namespace WeSafe.Authentication.WebApi.Services
{
    public class AuthTokenGenerator : IAuthTokenGenerator
    {
        public TokenResponseModel CreateToken(TokenRequestModel request)
        {
            ValidateRequest(request);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, request.Identifier),
                new Claim(ClaimsIdentity.DefaultNameClaimType, request.Name),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, request.Role),
            };

            if ( request.ClientId != null )
            {
                claims.Add(new Claim(WeSafeClaimTypes.ClientIdClaimType, request.ClientId.ToString()));
            }

            var claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            var jwt = new JwtSecurityToken(
                notBefore: DateTime.UtcNow,
                claims: claimsIdentity.Claims,
                expires: request.Expires,
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new TokenResponseModel
            {
                AccessToken = encodedJwt,
                UserName = claimsIdentity.Name,
                DisplayName = request.DisplayName,
                Role = request.Role,
                ExpiresAt = request.Expires,
                Demo = request.Demo
            };

            return response;
        }

        private void ValidateRequest(TokenRequestModel request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Identifier))
                throw new SystemOperationException($"{nameof(request.Identifier)} is required.");

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new SystemOperationException($"{nameof(request.Name)} is required.");
        }
    }
}
