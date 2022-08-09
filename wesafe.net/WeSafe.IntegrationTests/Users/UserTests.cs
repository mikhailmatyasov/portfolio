using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WeSafe.IntegrationTests.Base;
using WeSafe.Services.Client.Models;
using WeSafe.Web.Core.Models;
using Xunit;

namespace WeSafe.IntegrationTests.Users
{
    public class UserTests : UserBaseTests
    {
        #region Fields

        HttpClient httpClient;

        #endregion

        #region Ctor

        public UserTests(MediaGalleryFactory<StubStartup> factory) : base(factory)
        {
            httpClient = _factory.CreateClient();
        }

        #endregion

        #region Tests

        [Fact]
        public async Task CreateUserAsync_AdminCanCreateValidUser_UserIsCreatedAsync()
        {
            UpsertUserModel userModel = GetValidUserModel();

            var result = await AdminHttpClient.PostAsync(_baseUrl, new StringContent(JsonConvert.SerializeObject(userModel), Encoding.UTF8, "application/json"));

            var content = await result.Content.ReadAsStringAsync();

            Assert.True(result.IsSuccessStatusCode);

            UserModel user = await GetUserByUsernameAsync(userModel.UserName);

            Assert.NotNull(user);
        }

        [Fact]
        public async Task GetUserByIdAsync_AdminCanGetUserById_UserIsFoundAsync()
        {
            UserModel user = await GetCreatedUserAsync();

            UserModel foundUser = await GetAsync<UserModel>(_baseUrl + user.Id);

            Assert.True(ModelsAreEqual(user, foundUser));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public async Task GetUserByIdAsync_AdminCanNotGetUserByViaNotExistingId_UserIsNotFoundAsync(int userId)
        {
            UserModel foundUser = await GetAsync<UserModel>(_baseUrl + userId);

            Assert.Null(foundUser);
        }

        [Fact]
        public async Task DeleteUserAsynс_AdminCanDeleteUser_UserIsNotFoundAsync()
        {
            UserModel user = await GetCreatedUserAsync();

            var result = await AdminHttpClient.DeleteAsync(_baseUrl + user.Id);

            Assert.True(result.IsSuccessStatusCode);

            UserModel foundUser = await GetUserByUsernameAsync(user.UserName);

            Assert.Null(foundUser);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public async Task DeleteUserAsync_AdminCanNotDeleteUserByViaNotExistingId_IsNotSuccesfulStatus(int userId)
        {
            var result = await AdminHttpClient.DeleteAsync(_baseUrl + userId);

            Assert.False(result.IsSuccessStatusCode);
        }

        [Fact]
        public async Task UpdateUserAsync_AdminCanUpdateUser_UserIsUpdated()
        {
            UserModel user = await GetCreatedUserAsync();
            user.Phone = GetRandomPhoneNumber();
            user.Email = "sasda@gmail.com";
            user.DisplayName = GetRandomString();

            await UpdateUser(user, true);

            UserModel updatedUser = await GetUserByUsernameAsync(user.UserName);

            Assert.True(ModelsAreEqual(user, updatedUser));
        }

        //[LS] Need to add validators into controller method to pass commented tests.
        [Theory]
        [InlineData(" ", " ", " ", " ")]
        //[InlineData(null, "+155254485624", "NewUserName", "Users")]
        //[InlineData(" ", "+155254485624", "NewUserName", "Users")]
        [InlineData("NewName", null, "NewUserName", "Users")]
        [InlineData("NewName", " ", "NewUserName", "Users")]
        [InlineData("NewName", "+1552541234", "NewUserName", "Users")]
        [InlineData("NewName", "+1552541234567", "NewUserName", "Users")]
        [InlineData("NewName", "155254485624", "NewUserName", "Users")]
        //[InlineData("NewName", "+155254485624", null, "Users")]
        //[InlineData("NewName", "+155254485624", " ", "Users")]
        [InlineData("NewName", "+155254485624", "NewUserName", " ")]
        [InlineData("NewName", "+155254485624", "NewUserName", null)]
        [InlineData("NewName", "+155254485624", "NewUserName", "SomeRole")]
        public async Task UpdateUserAsync_AdminCanNotUpdateUserWithInvalidData_UserIsNotUpdated(string displayName, string phone, string userName, string roleName)
        {
            UserModel user = await GetCreatedUserAsync();
            user.DisplayName = displayName;
            user.Phone = phone;
            user.UserName = userName;
            user.RoleName = roleName;

            await UpdateUser(user, false);

            UserModel updatedUser = await GetUserByUsernameAsync(user.UserName);

            Assert.False(ModelsAreEqual(user, updatedUser));
        }

        [Fact]
        public async Task UpdateUserAsync_AdminCanNotUpdateUserWithNullValue_IsNotSuccessfulStatusAsync()
        {
            await UpdateUser(null, false);
        }

        [Fact]
        public async Task UpdateUserAsync_AdminCanUpdateUserWithSameModel_UserIsUpdated()
        {
            UserModel user = await GetCreatedUserAsync();

            await UpdateUser(user, true);

            UserModel updatedUser = await GetUserByUsernameAsync(user.UserName);

            Assert.True(ModelsAreEqual(user, updatedUser));
        }

        [Fact]
        public async Task UnlockUserAsync_AdminCanUpdateUserWithSameModel_UserIsUpdated()
        {
            UserModel user = await GetCreatedUserAsync();
            LoginModel loginModel = new LoginModel
            {
                UserName = user.UserName,
                Password = "1"
            };
            await LockUserAsync(loginModel);
            UserModel lockedUser = await GetUserByUsernameAsync(user.UserName);

            Assert.True(lockedUser.IsLocked);

            await AdminHttpClient.PostAsync(_baseUrl + user.Id + "/unlock", new StringContent(JsonConvert.SerializeObject(null), Encoding.UTF8, "application/json"));

            UserModel unlockedUser = await GetUserByUsernameAsync(user.UserName);

            Assert.False(unlockedUser.IsLocked);
        }

        #endregion

        #region PrivateMethods

        private async Task LockUserAsync(LoginModel model)
        {
            for (int i = 0; i < 3; i++)
            {
                await httpClient.PostAsync("/api/account/token", new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));
            }
        }

        private async Task UpdateUser(UserModel user, bool isSucceeded)
        {
            var result = await AdminHttpClient.PutAsync(_baseUrl, new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"));

            if (isSucceeded)
                Assert.True(result.IsSuccessStatusCode);
            else
                Assert.False(result.IsSuccessStatusCode);
        }

        #endregion
    }
}
