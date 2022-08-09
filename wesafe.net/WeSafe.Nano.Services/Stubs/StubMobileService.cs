using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Shared;

namespace WeSafe.Nano.Services.Stubs
{
    public class StubMobileService : IMobileService
    {
        public Task<MobileUserModel> SignIn(MobileSignInModel model)
        {
            return null;
        }

        public Task UpdateFirebaseToken(string mobileId, FirebaseTokenModel model)
        {
            return Task.CompletedTask;
        }

        public Task<PageResponse<CameraLogModel>> GetEvents(EventSearchRequest request)
        {
            return Task.FromResult(new PageResponse<CameraLogModel>(new List<CameraLogModel>(), 0));
        }

        public Task<CameraLogModel> GetEvent(string mobileId, int eventId)
        {
            return null;
        }

        public Task<CameraLogModel> GetEvent(int eventId)
        {
            return null;
        }

        public Task<SystemSettingsModel> GetSystemSettings(string mobileId)
        {
            return null;
        }

        public Task DeviceArm(string mobileId, DeviceArmModel model)
        {
            return Task.CompletedTask;
        }

        public Task Mute(string mobileId, MobileMuteModel model)
        {
            return Task.CompletedTask;
        }

        public Task SaveCameraSettings(string mobileId, SettingsModel model)
        {
            return Task.CompletedTask;
        }

        public Task SendNotifications(IEnumerable<ClientMobileSubscriberModel> users, MobileNotificationParams values)
        {
            return Task.CompletedTask;
        }

        public Task<IEnumerable<ClientMobileSubscriberModel>> GetClientMobileSubscribers(int clientId)
        {
            return Task.FromResult(Enumerable.Empty<ClientMobileSubscriberModel>());
        }

        public Task SendStatusChangedNotification(int deviceId)
        {
            return Task.CompletedTask;
        }
    }
}