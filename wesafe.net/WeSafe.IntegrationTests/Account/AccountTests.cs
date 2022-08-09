using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WeSafe.IntegrationTests.Base;
using WeSafe.Services.Client.Models;
using WeSafe.Web.Core.Models;
using Xunit;

namespace WeSafe.IntegrationTests.Account
{
    public class AccountTests : AccountBaseTest
    {
        #region Ctor

        public AccountTests(MediaGalleryFactory<StubStartup> factory) : base(factory)
        {
            Login().Wait();
        }

        #endregion

        #region Tests

        [Fact]
        public async Task FindDeviceTokenAsync_FindDeviceByExistenceDeviceToken_IsSuccessfullStatus()
        {
            DeviceTokenModel deviceTokenModel = await deviceBaseInstanse.GetValidDeviceTokenModelAsync();

            var result = await httpClient.PostAsync(_baseUrl + "token-status", new StringContent(JsonConvert.SerializeObject(deviceTokenModel), Encoding.UTF8, "application/json"));

            Assert.True(result.IsSuccessStatusCode);
        }

        [Theory]
        [InlineData("3")]
        [InlineData("")]
        [InlineData(null)]
        public async Task FindDeviceTokenAsync_FindDeviceByInvalidDeviceToken_IsNotSuccessfullStatus(string deviceToken)
        {
            DeviceTokenModel deviceTokenModel = new DeviceTokenModel
            {
                DeviceToken = deviceToken
            };

            var result = await httpClient.PostAsync(_baseUrl + "token-status", new StringContent(JsonConvert.SerializeObject(deviceTokenModel), Encoding.UTF8, "application/json"));

            Assert.False(result.IsSuccessStatusCode);
        }

        [Fact]
        public async Task GetLoginStatusAsync_CheckNewUserLoginStatus_IsSuccessfullStatusAsync()
        {
            await SignUpUserAsync(_baseUrl + "login-status", true);
        }

        [Fact]
        public async Task GetLoginStatusAsync_CheckExistanceUserLoginStatus_IsNotSuccessfullStatus()
        {
            SignUpModel signUpModel = await SignUpUserAsync(_baseUrl + "signup", true);

            var result = await httpClient.PostAsync(_baseUrl + "login-status", new StringContent(JsonConvert.SerializeObject(signUpModel), Encoding.UTF8, "application/json"));

            Assert.False(result.IsSuccessStatusCode);
        }

        //[LS] Need to add validators into AccountController method GetLoginStatusAsync to have test passed.
        //[Fact]
        //public async Task GetLoginStatusAsync_CheckInvalidUserLoginStatus_IsNotSuccessfullStatus()
        //{
        //    DeviceTokenModel deviceTokenModel = await deviceBaseInstanse.GetValidDeviceTokenModelAsync();
        //    IEnumerable<SignUpModel> invalidSignUpModels = GetInvalidSignUpModels(deviceTokenModel.DeviceToken);
        //
        //    foreach (var invalidModel in invalidSignUpModels)
        //    {
        //        var result = await httpClient.PostAsync("/login-status", new StringContent(JsonConvert.SerializeObject(invalidModel), Encoding.UTF8, "application/json"));
        //        
        //      Assert.False(result.IsSuccessStatusCode);
        //    }     
        //}

        [Fact]
        public async Task SignUpAsync_UserCanSignUp_SuccessfullAppropriateTokenResponse()
        {
            SignUpModel signUpModel = await GetSignUpModelAsync();

            var result = await httpClient.PostAsync(_baseUrl + "signup", new StringContent(JsonConvert.SerializeObject(signUpModel), Encoding.UTF8, "application/json"));

            Assert.True(result.IsSuccessStatusCode);

            TokenResponse tokenResponse = await GetParsedResponseAsync<TokenResponse>(result);

            Assert.True(tokenResponse.UserName == signUpModel.UserName);
        }

        [Fact]
        public async Task SignUpAsync_AfterRegistrationUserIsAdded_UserIsCreatedAsync()
        {
            SignUpModel signUpModel = await SignUpUserAsync(_baseUrl + "signup", true);

            UserModel createdUser = await userBaseInstanse.GetUserByUsernameAsync(signUpModel.UserName);

            Assert.True(createdUser != null);

        }

        [Fact]
        public async Task SignUpAsync_UserCanNotSignUpWithExistanceUsername_UserIsNotCreated()
        {
            SignUpModel signUpModel = await SignUpUserAsync(_baseUrl + "signup", true);

            var result = await httpClient.PostAsync(_baseUrl + "signup", new StringContent(JsonConvert.SerializeObject(signUpModel), Encoding.UTF8, "application/json"));

            Assert.False(result.IsSuccessStatusCode);

            UserModel createdUser = await userBaseInstanse.GetUserByUsernameAsync(signUpModel.UserName);

            Assert.NotNull(createdUser);
        }

        [Fact]
        public async Task SignUpAsync_UserCanNotSignUpWithExistancePhone_UserIsNotCreated()
        {
            SignUpModel signUpModel = await SignUpUserAsync(_baseUrl + "signup", true);
            SignUpModel signUpModelWithTheSamePhone = await GetSignUpModelAsync();
            signUpModelWithTheSamePhone.Phone = signUpModel.Phone;

            var result = await httpClient.PostAsync(_baseUrl + "signup", new StringContent(JsonConvert.SerializeObject(signUpModelWithTheSamePhone), Encoding.UTF8, "application/json"));

            Assert.False(result.IsSuccessStatusCode);

            UserModel createdUser = await userBaseInstanse.GetUserByUsernameAsync(signUpModel.UserName);

            Assert.NotNull(createdUser);
        }

        [Fact]
        public async Task SignUpAsync_UserCanNotSignUpWithInvalidModel_UserIsNotCreated()
        {
            DeviceTokenModel deviceTokenModel = await deviceBaseInstanse.GetValidDeviceTokenModelAsync();
            IEnumerable<SignUpModel> invalidSignUpModels = GetInvalidSignUpModels(deviceTokenModel.DeviceToken);

            foreach (var invalidModel in invalidSignUpModels)
            {
                var result = await httpClient.PostAsync(_baseUrl + "signup", new StringContent(JsonConvert.SerializeObject(invalidModel), Encoding.UTF8, "application/json"));

                Assert.False(result.IsSuccessStatusCode);

                UserModel createdUser = await userBaseInstanse.GetUserByUsernameAsync(invalidModel?.UserName);

                Assert.Null(createdUser);
            }
        }

        [Fact]
        public async Task GetProfileAsync_UserCanGetHisProfile_AppropriateProfileIsGot()
        {
            HttpClient authHttpClient = _factory.CreateClient();
            var signUpModel = await AuthorizeUserAsync(authHttpClient);

            var result = await authHttpClient.GetAsync(_baseUrl + "profile");

            Assert.True(result.IsSuccessStatusCode);

            ProfileModel foundProfile = await GetParsedResponseAsync<ProfileModel>(result);

            Assert.Equal(signUpModel.Name, foundProfile.DisplayName);
        }

        [Theory]
        [InlineData("asda3425", "asda3425")]
        [InlineData("аврап7845", "аврап7845")]
        [InlineData("№%:;авп", "№%:;авп")]
        [InlineData("\"2542342", "\"2542342")]
        [InlineData("חותםחותם", "חותםחותם")]
        [InlineData("!@#$$%^&*()_+/*-+", "!@#$$%^&*()_+/*-+")]
        public async Task UpdateProfileAsync_UserCanChangeHisPasswordAndName_UserCanLogInWithNewPasswordAndNameIsChanged(string newPassword, string displayName)
        {
            HttpClient authHttpClient = _factory.CreateClient();
            SignUpModel signUpModel = await AuthorizeUserAsync(authHttpClient);
            ProfileModel profile = new ProfileModel
            {
                DisplayName = displayName,
                Password = newPassword,
                OldPassword = "123456"
            };

            var result = await authHttpClient.PostAsync(_baseUrl + "profile", new StringContent(JsonConvert.SerializeObject(profile), Encoding.UTF8, "application/json"));

            Assert.True(result.IsSuccessStatusCode);

            LoginModel loginModel = new LoginModel
            {
                UserName = signUpModel.UserName,
                Password = newPassword
            };
            var loginResult = await httpClient.PostAsync(_baseUrl + "token", new StringContent(JsonConvert.SerializeObject(loginModel), Encoding.UTF8, "application/json"));

            Assert.True(loginResult.IsSuccessStatusCode);

            UserModel updatedUser = await userBaseInstanse.GetUserByUsernameAsync(signUpModel.UserName);

            Assert.Equal(updatedUser.DisplayName, profile.DisplayName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task UpdateProfileAsync_UserCanNotChangeProfileWithInvalidName_UserIsNotCahnged(string name)
        {
            HttpClient authHttpClient = _factory.CreateClient();
            SignUpModel signUpModel = await AuthorizeUserAsync(authHttpClient);
            ProfileModel profile = new ProfileModel
            {
                DisplayName = name
            };

            var result = await authHttpClient.PostAsync(_baseUrl + "profile", new StringContent(JsonConvert.SerializeObject(profile), Encoding.UTF8, "application/json"));

            Assert.False(result.IsSuccessStatusCode);

            UserModel updatedUser = await userBaseInstanse.GetUserByUsernameAsync(signUpModel.UserName);

            Assert.NotEqual(updatedUser.DisplayName, profile.DisplayName);
        }

        [Theory]
        [InlineData("123456", "123456")]
        [InlineData("1234567", "12345678")]
        [InlineData("123456", "1234")]
        public async Task UpdateProfileAsync_UserCanNotChangeProfileWithInvalidPasswords_UserCanNotLogInWithNewPassword(string oldPassword, string newPassword)
        {
            HttpClient authHttpClient = _factory.CreateClient();
            SignUpModel signUpModel = await AuthorizeUserAsync(authHttpClient);
            ProfileModel profile = new ProfileModel
            {
                DisplayName = signUpModel.Name,
                Password = newPassword,
                OldPassword = oldPassword
            };

            var result = await authHttpClient.PostAsync(_baseUrl + "profile", new StringContent(JsonConvert.SerializeObject(profile), Encoding.UTF8, "application/json"));

            Assert.False(result.IsSuccessStatusCode);

            LoginModel loginModel = new LoginModel
            {
                UserName = signUpModel.UserName,
                Password = newPassword
            };
            var loginResult = await httpClient.PostAsync(_baseUrl + "token", new StringContent(JsonConvert.SerializeObject(loginModel), Encoding.UTF8, "application/json"));

            if (oldPassword == newPassword)
                Assert.True(loginResult.IsSuccessStatusCode);
            else
                Assert.False(loginResult.IsSuccessStatusCode);
        }

        [Theory]
        [InlineData("", "12345678")]
        [InlineData("123456", "")]
        [InlineData(null, "12345678")]
        [InlineData("123456", null)]
        public async Task UpdateProfileAsync_UserCanNotChangeProfileWithNullPasswords_UserCanNotLogInWithNewPassword(string oldPassword, string newPassword)
        {
            HttpClient authHttpClient = _factory.CreateClient();
            SignUpModel signUpModel = await AuthorizeUserAsync(authHttpClient);
            ProfileModel profile = new ProfileModel
            {
                DisplayName = signUpModel.Name,
                Password = newPassword,
                OldPassword = oldPassword
            };

            var result = await authHttpClient.PostAsync(_baseUrl + "profile", new StringContent(JsonConvert.SerializeObject(profile), Encoding.UTF8, "application/json"));

            Assert.True(result.IsSuccessStatusCode);

            LoginModel loginModel = new LoginModel
            {
                UserName = signUpModel.UserName,
                Password = newPassword
            };
            var loginResult = await httpClient.PostAsync(_baseUrl + "token", new StringContent(JsonConvert.SerializeObject(loginModel), Encoding.UTF8, "application/json"));

            Assert.False(loginResult.IsSuccessStatusCode);
        }

        #endregion
    }
}
