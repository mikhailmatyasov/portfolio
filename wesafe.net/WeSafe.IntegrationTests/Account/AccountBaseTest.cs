using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WeSafe.IntegrationTests.Base;
using WeSafe.IntegrationTests.Devices;
using WeSafe.IntegrationTests.Users;
using WeSafe.Services.Client.Models;
using WeSafe.Web.Core.Models;
using Xunit;

namespace WeSafe.IntegrationTests.Account
{
    public class AccountBaseTest : BaseTest
    {
        #region Fields

        protected readonly UserBaseTests userBaseInstanse;
        protected readonly DevicesBaseTests deviceBaseInstanse;
        protected readonly HttpClient httpClient;

        protected const string _baseUrl = "/api/account/";


        #endregion

        #region Ctor

        public AccountBaseTest(MediaGalleryFactory<StubStartup> factory) : base(factory)
        {
            deviceBaseInstanse = new DevicesBaseTests(factory);
            httpClient = _factory.CreateClient();
            userBaseInstanse = new UserBaseTests(factory);
        }

        #endregion

        #region Public Methods

        public async Task<SignUpModel> AuthorizeUserAsync(HttpClient httpClient)
        {
            SignUpModel signUpModel = await GetSignUpModelAsync();
            var tokenResult = httpClient.PostAsync(_baseUrl + "signup", new StringContent(JsonConvert.SerializeObject(signUpModel), Encoding.UTF8, "application/json")).Result;
            var token = GetParsedResponseAsync<TokenResponse>(tokenResult).Result.AccessToken;
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            return signUpModel;
        }

        #endregion

        #region Protected 

        protected async Task<SignUpModel> GetSignUpModelAsync()
        {
            DeviceTokenModel deviceTokenModel = await deviceBaseInstanse.GetValidDeviceTokenModelAsync();

            return new SignUpModel
            {
                DeviceToken = deviceTokenModel.DeviceToken,
                Name = GetRandomString(),
                Password = "123456",
                Phone = GetRandomPhoneNumber(),
                UserName = GetRandomString()
            };
        }

        protected async Task<SignUpModel> SignUpUserAsync(string url, bool isSucceeded)
        {
            SignUpModel signUpModel = await GetSignUpModelAsync();

            var result = await httpClient.PostAsync(url, new StringContent(JsonConvert.SerializeObject(signUpModel), Encoding.UTF8, "application/json"));

            if (isSucceeded)
                Assert.True(result.IsSuccessStatusCode);
            else
                Assert.False(result.IsSuccessStatusCode);

            return signUpModel;
        }

        protected IEnumerable<SignUpModel> GetInvalidSignUpModels(string deviceTokenModel) => new List<SignUpModel>()
        {
            null,
            new SignUpModel{DeviceToken = "", Name = "", Password = "", Phone = "", UserName = ""},
            new SignUpModel{DeviceToken = "", Name = "Name", Password = "123456", Phone = "+12346789112", UserName = "UserName"},
            new SignUpModel{DeviceToken = deviceTokenModel, Name = "", Password = "123456", Phone = "+12346789112", UserName = "UserName"},
            new SignUpModel{DeviceToken = deviceTokenModel, Name = "Name", Password = "", Phone = "+12346789112", UserName = "UserName"},
            new SignUpModel{DeviceToken = deviceTokenModel, Name = "Name", Password = "123456", Phone = "", UserName = "UserName"},
            new SignUpModel{DeviceToken = deviceTokenModel, Name = "Name", Password = "123456", Phone = "+12345467891", UserName = ""},
            new SignUpModel{DeviceToken = deviceTokenModel, Password = "123456", Phone = "+12345467891", UserName = "UserName"},
            new SignUpModel{DeviceToken = deviceTokenModel, Name = "Name", Phone = "+12345467891", UserName = "UserName"},
            new SignUpModel{DeviceToken = deviceTokenModel, Name = "Name", Password = "123456", UserName = "UserName"},
            new SignUpModel{DeviceToken = deviceTokenModel, Name = "Name", Password = "123456", Phone = "+12345467891"},
            new SignUpModel{ Name = "Name", Password = "123456", Phone = "1234", UserName = "UserName"},
            new SignUpModel{DeviceToken = deviceTokenModel, Name = "Name", Password = "123456", Phone = "12345467891", UserName = "UserName"},
            new SignUpModel{DeviceToken = deviceTokenModel, Name = "Name", Password = "123456", Phone = "123454675891", UserName = "UserName"},
            new SignUpModel{DeviceToken = deviceTokenModel, Name = "Name", Password = "123456", Phone = "+1234546789", UserName = "UserName"},
            new SignUpModel{DeviceToken = deviceTokenModel, Name = "Name", Password = "123456", Phone = "+1234546789111", UserName = "UserName"},
        };

        #endregion
    }
}
