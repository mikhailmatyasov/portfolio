using System.Collections.Generic;
using System.Threading.Tasks;
using WeSafe.Services.Client.Models;

namespace WeSafe.Services.Client
{
    public interface ITelegramService
    {
        Task<IEnumerable<ClientTelegramSubscriberModel>> GetClientTelegramSubscribers(int clientId);

        Task<TelegramUserModel> GetTelegramUserByChatId(long chatId);

        Task<TelegramUserModel> RegisterTelegramUser(RegisterTelegramUserModel model);

        Task<IEnumerable<DeviceStatusModel>> GetSystemStatus(long telegramId);

        Task<UserSettingsModel> GetUserSettings(long telegramId);

        Task Mute(TelegramMuteModel model);

        Task SaveCameraSettings(long telegramId, CameraSettingsModel model);
    }
}