using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace WeSafe.TelegramBot.Services
{
    public class MuteSystemHandler : CallbackHandler
    {
        private readonly IApiClient _apiClient;

        public MuteSystemHandler(IApiClient apiClient) : base(apiClient)
        {
            _apiClient = apiClient;
        }

        protected override async Task HandleCallbackAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var command = update.CallbackQuery.Data;
            var muteTime = command == "mute_all" ? DateTimeOffset.MaxValue : DateTimeOffset.MinValue;

            await _apiClient.MuteSystem(update.CallbackQuery.Message.Chat.Id, muteTime, cancellationToken);

            var status = await _apiClient.GetSystemStatus(update.CallbackQuery.Message.Chat.Id, cancellationToken);
            var settings = await _apiClient.GetUserSettings(update.CallbackQuery.Message.Chat.Id, cancellationToken);

            await botClient.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id,
                update.CallbackQuery.Message.MessageId, Statuses.GetSystemStatusText(status, settings),
                cancellationToken: cancellationToken);

            await botClient.EditMessageReplyMarkupAsync(update.CallbackQuery.Message.Chat.Id,
                update.CallbackQuery.Message.MessageId, Keyboards.GetSystemKeyboard(status, settings), cancellationToken);

            await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat,
                command == "mute_all" ? "System is muted" : "System is unmuted", cancellationToken: cancellationToken,
                replyMarkup: Keyboards.GetSystemKeyboard(status, settings));
        }
    }
}