using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;

namespace WeSafe.Web.Core.Authentication
{
    public class AuthTokenGenerator : IAuthTokenGenerator
    {
        public TokenResponse CreateToken(TokenRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, request.Identifier),
                new Claim(ClaimsIdentity.DefaultNameClaimType, request.Name),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, request.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            var jwt = new JwtSecurityToken(
                notBefore: DateTime.UtcNow,
                claims: claimsIdentity.Claims,
                expires: request.Expires,
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new TokenResponse
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
    }
}