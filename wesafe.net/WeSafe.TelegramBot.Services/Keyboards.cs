using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Telegram.Bot.Types.ReplyMarkups;
using WeSafe.Services.Client.Models;

namespace WeSafe.TelegramBot.Services
{
    public class Keyboards
    {
        public static InlineKeyboardMarkup GetSystemKeyboard(IEnumerable<DeviceStatusModel> devices, UserSettingsModel settings)
        {
            if ( !devices.Any() ) return null;
            if ( devices.Count() == 1 ) return GetDeviceKeyboard(devices.First(), settings, false);

            var keyboard = new List<InlineKeyboardButton[]>
            {
                new[] { InlineKeyboardButton.WithCallbackData("🔄 System status", "system status") }
            };

            foreach ( var device in devices )
            {
                keyboard.Add(new[]
                {
                    InlineKeyboardButton.WithCallbackData($"🔹 Device {device.Name ?? device.MACAddress} menu",
                        $"device {device.Id}")
                });
            }

            keyboard.Add(new[]
            {
                GetMuteButton(settings?.Mute)
            });

            return new InlineKeyboardMarkup(keyboard);
        }

        public static InlineKeyboardButton GetMuteButton(DateTimeOffset? mute)
        {
            if ( mute == null || DateTimeOffset.UtcNow > mute )
            {
                return InlineKeyboardButton.WithCallbackData("Mute system", "mute_all");
            }
            else
            {
                return InlineKeyboardButton.WithCallbackData("Unmute system", "unmute_all");
            }
        }

        public static InlineKeyboardMarkup GetDeviceKeyboard(DeviceStatusModel device, UserSettingsModel settings, bool sysMenu)
        {
            var keyboard = new List<InlineKeyboardButton[]>();

            if ( device.Cameras.Any() )
            {
                keyboard.Add(new[]
                {
                    InlineKeyboardButton.WithCallbackData($"📹🖼 All cameras views",
                        $"viewall {device.Id}")
                });
            }

            var row = new List<InlineKeyboardButton>();

            foreach ( var camera in device.Cameras )
            {
                row.Add(InlineKeyboardButton.WithCallbackData($"📹🖼 {camera.CameraName}",
                    $"view {camera.Id}"));
            }

            keyboard.Add(row.ToArray());

            keyboard.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData($"Status", $"device {device.Id}"),
                InlineKeyboardButton.WithCallbackData($"Settings", $"settings {device.Id}")
            });

            if ( sysMenu )
            {
                keyboard.Add(new[]
                {
                    InlineKeyboardButton.WithCallbackData($"System menu", $"system menu"),
                    GetMuteButton(settings?.Mute)
                });
            }
            else
            {
                keyboard.Add(new[]
                {
                    GetMuteButton(settings?.Mute)
                });
            }

            return new InlineKeyboardMarkup(keyboard);
        }

        public static InlineKeyboardMarkup GetDeviceSettingsKeyboard(DeviceStatusModel device)
        {
            var keyboard = new List<InlineKeyboardButton[]>();

            if ( device.IsArmed )
            {
                keyboard.Add(new[]
                {
                    InlineKeyboardButton.WithCallbackData($"Disarm",
                        $"disarm {device.Id}")
                });
            }
            else
            {
                keyboard.Add(new[]
                {
                    InlineKeyboardButton.WithCallbackData($"Arm",
                        $"arm {device.Id}")
                });
            }

            var row = new List<InlineKeyboardButton>();

            foreach ( var camera in device.Cameras )
            {
                row.Add(InlineKeyboardButton.WithCallbackData($"⚙️📹 {camera.CameraName}",
                    $"camera {camera.Id}"));
            }

            keyboard.Add(row.ToArray());

            keyboard.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData($"Device menu",
                    $"device {device.Id}")
            });

            return new InlineKeyboardMarkup(keyboard);
        }

        public static IEnumerable<(string, DateTimeOffset)> CameraMutes = new[]
        {
            ("🔔", DateTimeOffset.MinValue),
            ("🔕", DateTimeOffset.MaxValue),
            ("🔕15m", DateTimeOffset.UtcNow.AddMinutes(15)),
            ("🔕1h", DateTimeOffset.UtcNow.AddHours(1)),
            ("🔕8h", DateTimeOffset.UtcNow.AddHours(8)),
            ("🔕1d", DateTimeOffset.UtcNow.AddDays(1))
        };

        public static InlineKeyboardMarkup GetCameraKeyboard(CameraModel camera)
        {
            var row = new List<InlineKeyboardButton>();

            foreach ( var mute in CameraMutes )
            {
                row.Add(InlineKeyboardButton.WithCallbackData(mute.Item1, $"mute;{mute.Item2};{camera.Id}"));
            }

            RecognitionSettings recognition = new RecognitionSettings { Confidence = 90, Sensitivity = 7, AlertFrequency = 30 };

            if ( !String.IsNullOrEmpty(camera.RecognitionSettings) )
            {
                try
                {
                    recognition = JsonConvert.DeserializeObject<RecognitionSettings>(camera.RecognitionSettings);
                }
                catch ( Exception e )
                {
                }
            }

            InlineKeyboardButton incConf, decConf;
            int confStep = 1, minConf = 90;

            if ( recognition.Confidence <= minConf )
            {
                decConf = InlineKeyboardButton.WithCallbackData($"Confidence {minConf}% (min)",
                    $"conf {minConf} {camera.Id}");
            }
            else
            {
                decConf = InlineKeyboardButton.WithCallbackData($"Confidence {recognition.Confidence - confStep}%",
                    $"conf {recognition.Confidence - confStep} {camera.Id}");
            }

            if ( recognition.Confidence >= 100 )
            {
                incConf = InlineKeyboardButton.WithCallbackData($"Confidence 100% (max)", $"conf 100 {camera.Id}");
            }
            else
            {
                incConf = InlineKeyboardButton.WithCallbackData($"Confidence {recognition.Confidence + confStep}%",
                    $"conf {recognition.Confidence + confStep} {camera.Id}");
            }

            InlineKeyboardButton incSens, decSens;

            if ( recognition.Sensitivity <= 1 )
            {
                decSens = InlineKeyboardButton.WithCallbackData($"Sensitivity 1/10 (min)", $"sens 1 {camera.Id}");
            }
            else
            {
                decSens = InlineKeyboardButton.WithCallbackData($"Sensitivity {recognition.Sensitivity - 1}/10",
                    $"sens {recognition.Sensitivity - 1} {camera.Id}");
            }

            if ( recognition.Sensitivity >= 10 )
            {
                incSens = InlineKeyboardButton.WithCallbackData($"Sensitivity 10/10 (max)", $"sens 10 {camera.Id}");
            }
            else
            {
                incSens = InlineKeyboardButton.WithCallbackData($"Sensitivity {recognition.Sensitivity + 1}/10",
                    $"sens {recognition.Sensitivity + 1} {camera.Id}");
            }

            var keyboard = new List<InlineKeyboardButton[]>();

            keyboard.Add(row.ToArray());
            keyboard.Add(new[]
            {
                decConf, incConf
            });
            keyboard.Add(new[]
            {
                decSens, incSens
            });
            keyboard.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData($"Settings menu",
                    $"settings {camera.DeviceId}")
            });

            return new InlineKeyboardMarkup(keyboard);
        }
    }
}