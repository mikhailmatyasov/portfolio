using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WeSafe.IntegrationTests.Base;
using WeSafe.Services.Client.Models;
using WeSafe.Shared;
using Xunit;

namespace WeSafe.IntegrationTests.Users
{
    public class UserBaseTests : BaseTest
    {
        #region Fields

        protected const string _baseUrl = "/api/users/";

        #endregion

        #region Ctor

        public UserBaseTests(MediaGalleryFactory<StubStartup> factory) : base(factory)
        {
            Login().Wait();
        }

        #endregion

        #region PublicMethods

        public async Task<UserModel> GetUserByUsernameAsync(string username)
        {
            IEnumerable<UserModel> users = await GetAllUsers();

            Assert.False(users.Count(u => u.UserName == username) > 1);

            return users.SingleOrDefault(u => u.UserName == username);
        }
        public async Task<UserModel> GetCreatedUserAsync()
        {
            UpsertUserModel userModel = GetValidUserModel();
            await AdminHttpClient.PostAsync(_baseUrl, new StringContent(JsonConvert.SerializeObject(userModel), Encoding.UTF8, "application/json"));

            return await GetUserByUsernameAsync(userModel.UserName);
        }

        #endregion

        #region ProtectedMethods

        protected async Task<IEnumerable<UserModel>> GetAllUsers()
        {
            var result = await GetAsync<PageResponse<UserModel>>(_baseUrl);

            Assert.NotNull(result);
            Assert.NotNull(result.Items);

            return result.Items;
        }

        protected UpsertUserModel GetValidUserModel()
        {
            return new UpsertUserModel
            {
                DisplayName = GetRandomString(),
                Phone = GetRandomPhoneNumber(),
                UserName = GetRandomString(),
                Password = "123456",
                RoleName = "Users",
                IsActive = true
            };
        }
        #endregion
    }
}
