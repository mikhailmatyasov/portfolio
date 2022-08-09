using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WeSafe.Services.Client.Models;

namespace WeSafe.TelegramBot.Services
{
    public interface IApiClient
    {
        Task<TelegramUserModel> GetUserByChatId(long chatId, CancellationToken cancellationToken);

        Task<TelegramUserModel> RegisterTelegramUser(RegisterTelegramUserModel model, CancellationToken cancellationToken);

        Task<IEnumerable<DeviceStatusModel>> GetSystemStatus(long chatId, CancellationToken cancellationToken);

        Task<UserSettingsModel> GetUserSettings(long chatId, CancellationToken cancellationToken);

        Task<IEnumerable<CameraModel>> GetDeviceCameras(int deviceId, CancellationToken cancellationToken);

        Task<CameraModel> GetCameraById(int cameraId, CancellationToken cancellationToken);

        Task SaveCamera(CameraModel camera, CancellationToken cancellationToken);

        Task MuteSystem(long chatId, DateTimeOffset? mute, CancellationToken cancellationToken);

        Task ArmDevice(int deviceId, bool arm, CancellationToken cancellationToken);

        Task MuteCamera(long chatId, CameraSettingsModel model, CancellationToken cancellationToken);
    }
}