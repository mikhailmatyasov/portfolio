using FirebaseAdmin.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.DAL;
using WeSafe.DAL.Entities;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Services.Extensions;
using WeSafe.Shared;

namespace WeSafe.Services
{
    public class MobileService : BaseService, IMobileService
    {
        public MobileService(WeSafeDbContext context, ILoggerFactory loggerFactory, IOptionsSnapshot<MobileOptions> options) : base(context, loggerFactory)
        {
        }

        public async Task<MobileUserModel> SignIn(MobileSignInModel model)
        {
            if (model.UserName == null || !model.UserName.IsValidPhoneNumber())
                throw new InvalidOperationException("Phone " + model.UserName + " is not valid");

            if (!await HasActiveClients(model.UserName, model.Password)) return null;

            var user = await DbContext.MobileUsers.SingleOrDefaultAsync(c => c.Phone == model.UserName);

            if (user == null)
            {
                user = new MobileUser
                {
                    Phone = model.UserName,
                    CreatedAt = DateTimeOffset.UtcNow,
                    Status = "active"
                };

                DbContext.Add(user);

                await SaveChangesAsync();
            }

            return new MobileUserModel
            {
                Id = user.Id,
                Phone = user.Phone,
                Status = user.Status,
                Mute = user.Mute,
                CreatedAt = user.CreatedAt,
                IsActive = true
            };
        }

        public async Task UpdateFirebaseToken(string mobileId, FirebaseTokenModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var user = await DbContext.MobileUsers
                                      .Include(c => c.Devices)
                                      .SingleOrDefaultAsync(c => c.Phone == mobileId);

            if (user == null) throw new InvalidOperationException("Not found");

            var token = user.Devices.FirstOrDefault(c => c.FirebaseToken == model.CurrentToken);

            if (token == null && !String.IsNullOrWhiteSpace(model.NewToken))
            {
                token = new MobileDevice
                {
                    MobileUserId = user.Id,
                    FirebaseToken = model.NewToken
                };

                DbContext.MobileDevices.Add(token);
            }
            else if (token != null)
            {
                if (!String.IsNullOrWhiteSpace(model.NewToken)) token.FirebaseToken = model.NewToken;
                else DbContext.MobileDevices.Remove(token);
            }

            await SaveChangesAsync();
        }

        public async Task<PageResponse<CameraLogModel>> GetEvents(EventSearchRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (String.IsNullOrWhiteSpace(request.MobileId))
            {
                throw new ArgumentException("MobileId can not be empty");
            }

            request.Prepare();
            request.Validate();

            var clients = await DbContext.ClientSubscribers.Where(c => c.Phone == request.MobileId)
                                         .Select(c => c.ClientId)
                                         .ToListAsync();
            var query = DbContext.CameraLogs.Include(c => c.Camera)
                                 .ThenInclude(c => c.Device)
                                 .Where(c => c.Camera.Device.ClientId != null &&
                                             clients.Contains(c.Camera.Device.ClientId.Value))
                                 .ApplyDateFilterQuery(request)
                                 .ApplyEventsFilterQuery(request)
                                 .Select(c => new CameraLogModel
                                 {
                                     Id = c.Id,
                                     CameraId = c.CameraId,
                                     CameraName = c.Camera.CameraName,
                                     DeviceId = c.Camera.Device.Id,
                                     DeviceName = c.Camera.Device.Name,
                                     Message = c.Message,
                                     Time = c.Time,
                                     Alert = c.Alert,
                                     Entries = c.Entries.Select(x => new CameraLogEntryModel
                                     {
                                         Id = x.Id,
                                         CameraLogId = x.CameraLogId,
                                         TypeKey = x.TypeKey,
                                         ImageUrl = x.ImageUrl
                                     })
                                                .OrderBy(x => x.Id)
                                                .ToList()
                                 });

            var items = await query.ToListAsync();

            return new PageResponse<CameraLogModel>(items, 0);
        }

        public async Task<CameraLogModel> GetEvent(string mobileId, int eventId)
        {
            var query = from cameraLog in DbContext.CameraLogs
                        join camera in DbContext.Cameras on cameraLog.CameraId equals camera.Id
                        join device in DbContext.Devices on camera.DeviceId equals device.Id
                        join subscribers in DbContext.ClientSubscribers on device.ClientId equals subscribers.ClientId
                        join mobile in DbContext.MobileUsers on subscribers.Phone equals mobile.Phone
                        where mobile.Phone == mobileId && cameraLog.Id == eventId
                        select new CameraLogModel
                        {
                            Id = cameraLog.Id,
                            CameraId = cameraLog.CameraId,
                            CameraName = camera.CameraName,
                            DeviceId = device.Id,
                            DeviceName = device.Name,
                            Message = cameraLog.Message,
                            Parameters = cameraLog.Parameters,
                            Time = cameraLog.Time,
                            Alert = cameraLog.Alert,
                            Entries = cameraLog.Entries.Select(c => new CameraLogEntryModel
                            {
                                Id = c.Id,
                                CameraLogId = c.CameraLogId,
                                TypeKey = c.TypeKey,
                                ImageUrl = c.ImageUrl
                            })
                                               .OrderBy(c => c.Id)
                                               .ToList()
                        };

            return await query.FirstOrDefaultAsync();
        }

        public async Task<CameraLogModel> GetEvent(int eventId)
        {
            var query = from cameraLog in DbContext.CameraLogs
                        join camera in DbContext.Cameras on cameraLog.CameraId equals camera.Id
                        join device in DbContext.Devices on camera.DeviceId equals device.Id
                        where cameraLog.Id == eventId
                        select new CameraLogModel
                        {
                            Id = cameraLog.Id,
                            CameraId = cameraLog.CameraId,
                            CameraName = camera.CameraName,
                            DeviceId = device.Id,
                            DeviceName = device.Name,
                            Message = cameraLog.Message,
                            Parameters = cameraLog.Parameters,
                            Time = cameraLog.Time,
                            Alert = cameraLog.Alert,
                            Entries = cameraLog.Entries.Select(c => new CameraLogEntryModel
                            {
                                Id = c.Id,
                                CameraLogId = c.CameraLogId,
                                TypeKey = c.TypeKey,
                                ImageUrl = c.ImageUrl
                            })
                                               .OrderBy(c => c.Id)
                                               .ToList()
                        };

            return await query.FirstOrDefaultAsync();
        }

        public async Task<SystemSettingsModel> GetSystemSettings(string mobileId)
        {
            var user = await DbContext.MobileUsers.FirstOrDefaultAsync(c => c.Phone == mobileId);

            if (user == null) return null;

            var query = from device in DbContext.Devices
                        join client in DbContext.Clients on device.ClientId equals client.Id
                        join subscriber in DbContext.ClientSubscribers on client.Id equals subscriber.ClientId
                        join u in DbContext.MobileUsers on subscriber.Phone equals u.Phone
                        where client.IsActive && u.Phone == mobileId
                        select new
                        {
                            DeviceId = device.Id,
                            SubscriberId = subscriber.Id
                        };

            var data = await query.ToListAsync();
            var deviceIds = data.Select(c => c.DeviceId).ToList();
            var subscriberIds = data.Select(c => c.SubscriberId).ToList();

            var devices = await DbContext.Devices
                                         .Where(device => deviceIds.Contains(device.Id))
                                         .Select(device => new DeviceSettingsModel
                                         {
                                             Id = device.Id,
                                             MACAddress = device.MACAddress,
                                             IsArmed = device.IsArmed,
                                             Status = device.Status,
                                             NetworkStatus = device.NetworkStatus,
                                             Name = device.Name,
                                             Cameras = device.Cameras.Select(x => new SettingsModel
                                                             {
                                                                 CameraId = x.Id,
                                                                 CameraName = x.CameraName,
                                                                 IsActive = x.IsActive,
                                                                 Status = x.Status,
                                                                 NetworkStatus = x.NetworkStatus
                                                             })
                                                             .ToList()
                                         })
                                         .ToListAsync();

            var settings = await DbContext.ClientSubscriberSettings
                                          .Where(c => subscriberIds.Contains(c.ClientSubscriberId))
                                          .Select(c => new
                                          {
                                              c.ClientSubscriberId,
                                              c.CameraId,
                                              c.Mute
                                          })
                                          .ToListAsync();

            foreach ( var device in devices )
            {
                foreach ( SettingsModel camera in device.Cameras )
                {
                    camera.Mute = settings.Any(c => c.CameraId == camera.CameraId && DateTimeOffset.UtcNow < c.Mute);
                }
            }

            return new ClientSystemSettingsModel
            {
                Mute = DateTimeOffset.UtcNow < user.Mute,
                Devices = devices
            };
        }

        public async Task DeviceArm(string mobileId, DeviceArmModel model)
        {
            var query = from dev in DbContext.Devices
                        join client in DbContext.Clients on dev.ClientId equals client.Id
                        join subscriber in DbContext.ClientSubscribers on client.Id equals subscriber.ClientId
                        join user in DbContext.MobileUsers on subscriber.Phone equals user.Phone
                        where user.Phone == mobileId && dev.Id == model.DeviceId
                        select dev;

            var device = await query.SingleOrDefaultAsync();

            if (device == null) throw new InvalidOperationException("Device not found");

            device.IsArmed = model.Arm;

            await SaveChangesAsync();
        }

        public async Task Mute(string mobileId, MobileMuteModel model)
        {
            var user = await DbContext.MobileUsers.FirstOrDefaultAsync(c => c.Phone == mobileId);

            if (user != null)
            {
                user.Mute = model.Mute ? (DateTimeOffset?)DateTimeOffset.MaxValue : null;

                await SaveChangesAsync();
            }
        }

        public async Task SaveCameraSettings(string mobileId, SettingsModel model)
        {
            var query = from user in DbContext.MobileUsers
                        join subscriber in DbContext.ClientSubscribers on user.Phone equals subscriber.Phone
                        join client in DbContext.Clients on subscriber.ClientId equals client.Id
                        join device in DbContext.Devices on client.Id equals device.ClientId
                        join camera in DbContext.Cameras on device.Id equals camera.DeviceId
                        where client.IsActive && user.Phone == mobileId && camera.Id == model.CameraId
                        select subscriber.Id;

            var subscriberId = await query.SingleOrDefaultAsync();

            if (subscriberId == default) return;

            var settings = await DbContext.ClientSubscriberSettings.SingleOrDefaultAsync(c =>
                c.CameraId == model.CameraId && c.ClientSubscriberId == subscriberId);

            if (settings == null)
            {
                settings = new ClientSubscriberSettings
                {
                    CameraId = model.CameraId,
                    ClientSubscriberId = subscriberId
                };

                DbContext.ClientSubscriberSettings.Add(settings);
            }

            settings.Mute = model.Mute ? (DateTimeOffset?)DateTimeOffset.MaxValue : null;

            await SaveChangesAsync();
        }

        public async Task SendNotifications(IEnumerable<ClientMobileSubscriberModel> users, MobileNotificationParams values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            if (values.ImageUrls == null) throw new ArgumentNullException(nameof(values.ImageUrls));

            var tokens = new List<string>();

            foreach (var user in users)
            {
                var cameraMute = user.CameraSettings
                                     .Where(c => c.CameraId == values.CameraId)
                                     .Select(c => c.Mute)
                                     .FirstOrDefault();

                if ((user.Mute == null || DateTimeOffset.UtcNow > user.Mute) &&
                     (cameraMute == null || DateTimeOffset.UtcNow > cameraMute) &&
                     user.Assignments.IsAssignmentAllowed(values.DeviceId, values.CameraId))
                {
                    tokens.AddRange(user.Devices.Select(c => c.FirebaseToken));
                }
            }

            if (tokens.Any())
            {
                var data = new Dictionary<string, string>
                {
                    { "id", values.LogId.ToString() }
                };

                await SendMessageAsync(values, tokens, data);
            }
        }

        private async Task SendMessageAsync(MobileNotificationParams values, List<string> tokens, Dictionary<string, string> data)
        {
            var message = new MulticastMessage()
            {
                Tokens = tokens,
                Data = data,
                Notification = new Notification
                {
                    Title = values.Title,
                    Body = values.NotificationText,
                    ImageUrl = values.ImageUrls != null && values.ImageUrls.Any() ? values.ImageUrls.ElementAt(0) : null
                }
            };

            try
            {
                Logger.LogDebug("Send to mobile app. Message: {0}", JsonConvert.SerializeObject(message));

                var response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);

                if (response.FailureCount != 0)
                {
                    var errors = response.Responses.Where(c => c.Exception != null)
                                         .Select(c => c.Exception.ToString());

                    Logger.LogDebug("End sending to mobile app. FailureCount: {FailureCount}. Errors: {Errors}",
                        response.FailureCount, errors);
                }
                else
                {
                    Logger.LogDebug("End sending to mobile app. SuccessCount: {SuccessCount}", response.SuccessCount);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Error by sending notification to mobile app: {e.Message}");
            }
        }

        public async Task<IEnumerable<ClientMobileSubscriberModel>> GetClientMobileSubscribers(int clientId)
        {
            var users = await (from subscriber in DbContext.ClientSubscribers
                               join user in DbContext.MobileUsers on subscriber.Phone equals user.Phone
                               where subscriber.ClientId == clientId && subscriber.IsActive
                               select new ClientMobileSubscriberModel
                               {
                                   Id = subscriber.Id,
                                   ClientId = subscriber.ClientId,
                                   Name = subscriber.Name,
                                   Phone = subscriber.Phone,
                                   Permissions = subscriber.Permissions,
                                   IsActive = subscriber.IsActive,
                                   CreatedAt = subscriber.CreatedAt,
                                   Mute = user.Mute,
                                   Devices = user.Devices.Select(c => new MobileDeviceModel
                                   {
                                       FirebaseToken = c.FirebaseToken
                                   }).ToList(),
                                   CameraSettings = subscriber.Settings
                                                              .Select(c => new CameraSettingsModel
                                                              {
                                                                  CameraId = c.CameraId,
                                                                  Mute = c.Mute
                                                              })
                                                              .ToList(),
                                   Assignments = subscriber.Assignments
                                                           .Select(c => new AssignmentModel
                                                           {
                                                               Id = c.Id,
                                                               DeviceId = c.DeviceId,
                                                               CameraId = c.CameraId
                                                           })
                                                           .ToList()
                               }).ToListAsync();

            return users;
        }

        public async Task SendStatusChangedNotification(int deviceId)
        {
            var device = await DbContext.Devices.FindAsync(deviceId);

            if (device?.ClientId == null) return;

            var cameras = await DbContext.Cameras
                                         .Where(c => c.DeviceId == deviceId && c.IsActive)
                                         .OrderBy(c => c.CameraName)
                                         .Select(c => new CameraMonitoringModel
                                         {
                                             Id = c.Id,
                                             CameraName = c.CameraName,
                                             Status = c.Status,
                                             NetworkStatus = c.NetworkStatus
                                         })
                                         .ToListAsync();

            var message = GetStatusChangedMessage(device, cameras);
            var users = await GetClientMobileSubscribers(device.ClientId.Value);
            var tokens = new List<string>();

            foreach (var user in users)
            {
                if ((user.Mute == null || DateTimeOffset.UtcNow > user.Mute) &&
                     (!user.Assignments.Any() || user.Assignments.Any(c => c.DeviceId == deviceId)))
                {
                    tokens.AddRange(user.Devices.Select(c => c.FirebaseToken));
                }
            }

            if (tokens.Any())
            {
                await SendMessageAsync(new MobileNotificationParams
                {
                    Title = "Device status changed",
                    NotificationText = message,
                    ImageUrls = new List<string>()
                }, tokens, null);
            }
        }

        private static string GetStatusChangedMessage(Device device, List<CameraMonitoringModel> cameras)
        {
            string status;
            string networkStatus;

            if (device.Status == "online") status = "✅";
            else if (device.Status == "offline") status = "❌";
            else status = "❔";

            if (device.NetworkStatus == "online") networkStatus = "✅";
            else if (device.NetworkStatus == "offline") networkStatus = "❌";
            else networkStatus = "❔";

            string message = $"Device {device.MACAddress} Status: {status}, Network status: {networkStatus}\n";

            if (!cameras.Any())
            {
                message += "No cameras were configured.\n";
            }

            foreach (var camera in cameras)
            {
                if (camera.Status == "online") status = "✅";
                else if (camera.Status == "offline") status = "❌";
                else status = "❔";

                if (camera.NetworkStatus == "online") networkStatus = "✅";
                else if (camera.NetworkStatus == "offline") networkStatus = "❌";
                else networkStatus = "❔";

                message += $"Camera {camera.CameraName} Status: {status}, Network status: {networkStatus}\n";
            }

            return message;
        }

        private Task<bool> HasActiveClients(string phone, string password)
        {
            return DbContext.ClientSubscribers.AnyAsync(c => c.IsActive && c.Phone == phone && c.Password == password);
        }
    }
}