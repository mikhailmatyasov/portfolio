using System.Threading.Tasks;
using WeSafe.Authentication.Integration.Tests.Static;
using WeSafe.Authentication.WebApi;
using WeSafe.Authentication.WebApi.Commands.Users;
using WeSafe.Shared.Roles;
using WeSafe.Web.Common.Exceptions.Models;
using WeSafe.Web.Common.Models;
using Xunit;

namespace WeSafe.Authentication.Integration.Tests.Users
{
    public class CreateUserTests : BaseTest<Startup>,
        IClassFixture<AuthenticationWebApplicationFactory<Startup>>
    {
        private AuthenticationWebApplicationFactory<Startup> _factory;

        public CreateUserTests(AuthenticationWebApplicationFactory<Startup> factory) : base(factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task CreateUser_Success()
        {
            var result = await Post<IdModel<string>>(AuthenticationUrls.createUserInternalUrl, new CreateUserCommand
            {
                UserName = "user54532",
                DisplayName = "User",
                Password = "123456",
                RoleName = UserRoles.Users
            });

            Assert.NotNull(result);
            Assert.NotNull(result.Id);
        }

        [Fact]
        public async Task CreateUser_UserNameExist()
        {
            var result = await Post<ErrorModel>(AuthenticationUrls.createUserInternalUrl, new CreateUserCommand
            {
                UserName = "user",
                DisplayName = "User",
                Password = "123456",
                RoleName = UserRoles.Users
            });

            Assert.NotNull(result);
            Assert.True(result.Code == 400);
        }

        [Fact]
        public async Task CreateUser_InvalidPhone()
        {
            var result = await Post<ErrorModel>(AuthenticationUrls.createUserInternalUrl, new CreateUserCommand
            {
                UserName = "user2",
                DisplayName = "User",
                Password = "123456",
                Phone = "invalid_phone",
                RoleName = UserRoles.Users
            });

            Assert.NotNull(result);
            Assert.True(result.Code == 400);
        }

        [Fact]
        public async Task CreateUser_InvalidPassword()
        {
            var result = await Post<ErrorModel>(AuthenticationUrls.createUserInternalUrl, new CreateUserCommand
            {
                UserName = "user3",
                DisplayName = "User",
                Password = "1234",
                RoleName = UserRoles.Users
            });

            Assert.NotNull(result);
            Assert.True(result.Code == 400);
        }

        [Fact]
        public async Task CreateUser_InvalidUserName()
        {
            var result = await Post<ErrorModel>(AuthenticationUrls.createUserInternalUrl, new CreateUserCommand
            {
                UserName = null,
                DisplayName = "User",
                Password = "123456",
                RoleName = UserRoles.Users
            });

            Assert.NotNull(result);
            Assert.True(result.Code == 400);
        }

        [Fact]
        public async Task CreateUser_InvalidDisplayName()
        {
            var result = await Post<ErrorModel>(AuthenticationUrls.createUserInternalUrl, new CreateUserCommand
            {
                UserName = "user4",
                DisplayName = null,
                Password = "123456",
                RoleName = UserRoles.Users
            });

            Assert.NotNull(result);
            Assert.True(result.Code == 400);
        }

        [Fact]
        public async Task CreateUser_RequiredRoleName()
        {
            var result = await Post<ErrorModel>(AuthenticationUrls.createUserInternalUrl, new CreateUserCommand
            {
                UserName = "user4",
                DisplayName = "User",
                Password = "123456",
                RoleName = null
            });

            Assert.NotNull(result);
            Assert.True(result.Code == 400);
        }

        [Fact]
        public async Task CreateUser_InvalidRoleName()
        {
            var result = await Post<ErrorModel>(AuthenticationUrls.createUserInternalUrl, new CreateUserCommand
            {
                UserName = "user4",
                DisplayName = "User",
                Password = "123456",
                RoleName = "invalid_role"
            });

            Assert.NotNull(result);
            Assert.True(result.Code == 400);
        }
    }
}