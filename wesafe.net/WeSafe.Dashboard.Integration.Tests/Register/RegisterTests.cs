using System.Threading.Tasks;
using WeSafe.Authentication.Integration.Tests;
using WeSafe.Dashboard.WebApi;
using WeSafe.Dashboard.WebApi.Commands.Register;
using Xunit;

namespace WeSafe.Dashboard.Integration.Tests.Register
{
    public class RegisterTests : DashboardBaseTest, IClassFixture<DashboardWebApplicationFactory<Startup>>,
        IClassFixture<AuthenticationWebApplicationFactory<WeSafe.Authentication.WebApi.Startup>>
    {
        public RegisterTests(DashboardWebApplicationFactory<Startup> factory,
            AuthenticationWebApplicationFactory<WeSafe.Authentication.WebApi.Startup> authFactory) : base(factory, authFactory)
        {
            Authorize("Admin").GetAwaiter().GetResult();
        }

        [Fact]
        public async Task RegisterDevice_Success()
        {
            //await Post("/api/account/signup", new RegisterCommand
            //{
            //    Name = "My name",
            //    UserName = "user1",
            //    Password = "123456",
            //    Phone = "+74444567890",
            //    DeviceToken = "123456abc"
            //});
        }
    }
}