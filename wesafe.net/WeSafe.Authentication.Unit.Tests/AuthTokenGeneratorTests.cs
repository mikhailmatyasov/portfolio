using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using WeSafe.Authentication.WebApi.Models;
using WeSafe.Authentication.WebApi.Services;
using WeSafe.Web.Common.Authentication;
using Xunit;

namespace WeSafe.Authentication.Unit.Tests
{
    public class AuthTokenGeneratorTests
    {
        private readonly AuthTokenGenerator _authTokenGenerator;

        public AuthTokenGeneratorTests()
        {
            _authTokenGenerator = new AuthTokenGenerator();
        }

        [Fact]
        public void CreateToken_RequestIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _authTokenGenerator.CreateToken(null));
        }

        [Fact]
        public void CreateToken_ValidParameter_CreatesToken()
        {
            // arrange
            var requestToken = new TokenRequestModel("someIndetifier", "SomeName", DateTime.Now)
            {
                DisplayName = "SOmeDisplayName",
                Role = "SomeRole"
            };

            // act
            var result = _authTokenGenerator.CreateToken(requestToken);

            // assert
            Assert.NotNull(result);
            Assert.NotNull(result.AccessToken);
            Assert.NotEmpty(result.AccessToken);
            Assert.True(result.UserName == requestToken.Name);
            Assert.True(result.Role == requestToken.Role);
            ValidateToken(requestToken, result);
        }

        private void ValidateToken(TokenRequestModel request, TokenResponseModel response)
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(response.AccessToken);

            Assert.True(jwt.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value == request.Identifier);
            Assert.True(jwt.Claims.First(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Value == request.Name);
            Assert.True(jwt.Claims.First(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value == request.Role);
        }
    }
}
