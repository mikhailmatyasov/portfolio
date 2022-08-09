using System.Collections.Generic;
using System.Threading.Tasks;
using WeSafe.Services.Client.Models;
using WeSafe.Shared;

namespace WeSafe.Services.Client
{
    public interface IMobileService
    {
        Task<MobileUserModel> SignIn(MobileSignInModel model);

        Task UpdateFirebaseToken(string mobileId, FirebaseTokenModel model);

        Task<PageResponse<CameraLogModel>> GetEvents(EventSearchRequest request);

        Task<CameraLogModel> GetEvent(string mobileId, int eventId);

        Task<CameraLogModel> GetEvent(int eventId);

        Task<SystemSettingsModel> GetSystemSettings(string mobileId);

        Task DeviceArm(string mobileId, DeviceArmModel model);

        Task Mute(string mobileId, MobileMuteModel model);

        Task SaveCameraSettings(string mobileId, SettingsModel model);

        Task SendNotifications(IEnumerable<ClientMobileSubscriberModel> users, MobileNotificationParams values);

        Task<IEnumerable<ClientMobileSubscriberModel>> GetClientMobileSubscribers(int clientId);

        Task SendStatusChangedNotification(int deviceId);
    }
}