using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace WeSafe.TelegramBot.Services
{
    public class TelegramHandleUpdate : ITelegramHandleUpdate
    {
        private readonly ILogger<TelegramHandleUpdate> _logger;
        private readonly IServiceProvider _serviceProvider;

        public TelegramHandleUpdate(ILogger<TelegramHandleUpdate> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Update object received Id:{UpdateId} Type:{UpdateType}", update.Id, update.Type);

            IUpdateHandler handler = null;

            if ( IsCommand(update) )
            {
                var text = update.Message.Text.Trim().ToLower();

                if ( text == "/start" ) handler = _serviceProvider.GetService<StartCommand>();
            }
            else if ( IsContact(update) )
            {
                handler = _serviceProvider.GetService<RegisterCommand>();
            }
            else if ( IsSystemMenu(update) || IsCallback(update, "system") )
            {
                handler = _serviceProvider.GetService<SystemMenuHandler>();
            }
            else if ( IsCallback(update, "viewall") )
            {
                handler = _serviceProvider.GetService<ViewAllHandler>();
            }
            else if ( IsCallback(update, "view") )
            {
                handler = _serviceProvider.GetService<ViewHandler>();
            }
            else if ( IsCallback(update, "device") )
            {
                handler = _serviceProvider.GetService<DeviceStatHandler>();
            }
            else if ( IsCallback(update, "settings") )
            {
                handler = _serviceProvider.GetService<DeviceSettingsHandler>();
            }
            else if ( IsCallback(update, "mute_all") || IsCallback(update, "unmute_all") )
            {
                handler = _serviceProvider.GetService<MuteSystemHandler>();
            }
            else if ( IsCallback(update, "disarm") || IsCallback(update, "arm") )
            {
                handler = _serviceProvider.GetService<DeviceArmHandler>();
            }
            else if ( IsCallback(update, "camera") )
            {
                handler = _serviceProvider.GetService<CameraSettingsHandler>();
            }
            else if ( IsCallback(update, "mute") )
            {
                handler = _serviceProvider.GetService<MuteCameraHandler>();
            }
            else if ( IsCallback(update, "conf") )
            {
                handler = _serviceProvider.GetService<CameraConfidenceHandler>();
            }
            else if ( IsCallback(update, "sens") )
            {
                handler = _serviceProvider.GetService<CameraSensitivityHandler>();
            }

            if ( handler != null ) await handler.HandleUpdateAsync(botClient, update, cancellationToken);
        }

        private bool IsCommand(Update update)
        {
            return update.Message != null && !String.IsNullOrWhiteSpace(update.Message.Text) &&
                   update.Message.Text.StartsWith("/");
        }

        private bool IsContact(Update update)
        {
            return update.Message != null && update.Message.Type == MessageType.Contact &&
                   update.Message.Contact != null;
        }

        private bool IsSystemMenu(Update update)
        {
            return update.Message != null && !String.IsNullOrWhiteSpace(update.Message.Text) &&
                   update.Message.Text == "System menu";
        }

        private bool IsCallback(Update update, string pattern = null)
        {
            return update.Type == UpdateType.CallbackQuery && update.CallbackQuery != null &&
                   (pattern == null || update.CallbackQuery.Data.StartsWith(pattern));
        }
    }
}