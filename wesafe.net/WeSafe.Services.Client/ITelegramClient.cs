using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WeSafe.Services.Client.Models;
using WeSafe.Shared.Results;

namespace WeSafe.Services.Client
{
    public interface ITelegramClient
    {
        Task<IExecutionResult> SendPhotoAsync(string userId, Stream file, string caption);

        Task<IExecutionResult> SendPhotoAsync(Int64 chatId, Stream file, string caption);

        Task SendMediaGroupAsync(Int64 chatId, IEnumerable<string> fileUrls, string caption);

        Task SendToStatChatAsync(DeviceStatusModel device, string clientName);
    }
}