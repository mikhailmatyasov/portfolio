using System.Collections.Generic;
using System.Threading.Tasks;
using WeSafe.Authentication.Integration.Tests;
using WeSafe.Dashboard.WebApi;
using WeSafe.Dashboard.WebApi.Models;
using WeSafe.Web.Common.Exceptions.Models;
using Xunit;

namespace WeSafe.Dashboard.Integration.Tests.Cameras
{
    public class CameraTestsIClassFixture : DashboardBaseTest, IClassFixture<DashboardWebApplicationFactory<Startup>>,
        IClassFixture<AuthenticationWebApplicationFactory<WeSafe.Authentication.WebApi.Startup>>
    {
        public CameraTestsIClassFixture(DashboardWebApplicationFactory<Startup> factory, AuthenticationWebApplicationFactory<WeSafe.Authentication.WebApi.Startup> authFactory) : base(factory, authFactory)
        {
            Authorize("Admin").GetAwaiter().GetResult();
        }

        [Fact]
        public async Task GetCameraByIdAsync_UserInvalidUserId_BadRequestException()
        {
            var result = await Get<ErrorModel>("/api/cameras/-1");

            Assert.True(result.Code == 400);
        }

        [Fact]
        public async Task GetCameraByIdAsync_CameraNotFound_NotFoundException()
        {
            var result = await Get<ErrorModel>("/api/cameras/77");

            Assert.True(result.Code == 404);
        }

        [Fact]
        public async Task CreateCamerasAsync_DeviceNotFound_NotFoundException()
        {
            var cameras = GetValidCameras();

            var result = await Post<ErrorModel>("/api/cameras/notExistedMac/cameras", cameras);

            Assert.True(result.Code == 404);
        }

        [Fact]
        public async Task CreateCamerasAsync_EmptyCamerasCollection_BadRequestException()
        {
            var result = await Post<ErrorModel>("/api/cameras/1a:30:48:5a:58:65/cameras", new List<CameraBaseModel>());

            Assert.True(result.Code == 400);
        }

        [Fact]
        public async Task CreateCamerasAsync_ProcessSuccess()
        {
            var cameras = GetValidCameras();

            await Post("/api/cameras/1a:30:48:5a:58:65/cameras", cameras);
        }

        private IEnumerable<CameraBaseModel> GetValidCameras()
        {
            return new CameraBaseModel[]
            {
                new CameraBaseModel()
                {
                    CameraName = "Camera1",
                    Login = "login",
                    Password = "password",
                    Ip = "1.1.1.1",
                    Port = "80",
                    SpecificRtcpConnectionString = "connectionString"
                },
                new CameraBaseModel()
                {
                    CameraName = "Camera2",
                    Login = "login",
                    Password = "password",
                    Ip = "2.2.2.2",
                    Port = "80",
                    SpecificRtcpConnectionString = "connectionString"
                }
            };
        }
    }
}
