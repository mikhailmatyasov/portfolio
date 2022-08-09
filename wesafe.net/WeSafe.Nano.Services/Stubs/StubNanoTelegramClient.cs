using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Shared.Results;

namespace WeSafe.Nano.Services.Stubs
{
    public class StubNanoTelegramClient : ITelegramClient
    {
        public Task<IExecutionResult> SendPhotoAsync(string userId, Stream file, string caption)
        {
            return Task.FromResult(ExecutionResult.Payload(""));
        }

        public Task<IExecutionResult> SendPhotoAsync(long chatId, Stream file, string caption)
        {
            return Task.FromResult(ExecutionResult.Payload(""));
        }

        public Task SendMediaGroupAsync(long chatId, IEnumerable<string> fileUrls, string caption)
        {
            return Task.CompletedTask;
        }

        public Task SendToStatChatAsync(DeviceStatusModel device, string clientName)
        {
            return Task.CompletedTask;
        }
    }
}
