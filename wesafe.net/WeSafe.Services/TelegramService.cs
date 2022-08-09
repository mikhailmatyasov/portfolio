using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WeSafe.DAL;
using WeSafe.DAL.Entities;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Services.Extensions;

namespace WeSafe.Services
{
    public class TelegramService : BaseService, ITelegramService
    {
        public TelegramService(WeSafeDbContext context, ILoggerFactory loggerFactory) : base(context, loggerFactory)
        {
        }

        public async Task<IEnumerable<ClientTelegramSubscriberModel>> GetClientTelegramSubscribers(int clientId)
        {
            var query = from subscriber in DbContext.ClientSubscribers
                        join user in DbContext.TelegramUsers on subscriber.Phone equals user.Phone
                        where subscriber.ClientId == clientId && subscriber.IsActive
                        select new ClientTelegramSubscriberModel
                        {
                            Id = subscriber.Id,
                            ClientId = subscriber.ClientId,
                            Name = subscriber.Name,
                            Phone = subscriber.Phone,
                            Permissions = subscriber.Permissions,
                            IsActive = subscriber.IsActive,
                            CreatedAt = subscriber.CreatedAt,
                            TelegramId = user.TelegramId,
                            Mute = user.Mute,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
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
                        };

            return await query.ToListAsync();
        }

        public async Task<TelegramUserModel> GetTelegramUserByChatId(long chatId)
        {
            var user = await DbContext.TelegramUsers.Select(c => new TelegramUserModel
                                      {
                                          Id = c.Id,
                                          Phone = c.Phone,
                                          TelegramId = c.TelegramId,
                                          FirstName = c.FirstName,
                                          LastName = c.LastName,
                                          Settings = c.Settings,
                                          Status = c.Status,
                                          Mute = c.Mute,
                                          CreatedAt = c.CreatedAt
                                      })
                                      .SingleOrDefaultAsync(c => c.TelegramId == chatId);

            if ( user == null ) return null;

            user.IsActive = await HasActiveClients(user.Phone);

            return user;
        }

        public async Task<TelegramUserModel> RegisterTelegramUser(RegisterTelegramUserModel model)
        {
            if ( !await HasActiveClients(model.Phone) ) return null;

            if ( !model.Phone.IsValidPhoneNumber() )
                throw new InvalidOperationException("Phone " + model.Phone + " is not valid");

            var user = await DbContext.TelegramUsers.SingleOrDefaultAsync(c => c.TelegramId == model.TelegramId);

            if ( user == null )
            {
                user = new TelegramUser
                {
                    TelegramId = model.TelegramId,
                    Phone = model.Phone,
                    CreatedAt = DateTimeOffset.UtcNow,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Status = "active"
                };

                DbContext.Add(user);

                await SaveChangesAsync();
            }

            return new TelegramUserModel
            {
                Id = user.Id,
                Phone = user.Phone,
                TelegramId = user.TelegramId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Settings = user.Settings,
                Status = user.Status,
                Mute = user.Mute,
                CreatedAt = user.CreatedAt,
                IsActive = true
            };
        }

        public async Task<IEnumerable<DeviceStatusModel>> GetSystemStatus(long telegramId)
        {
            var query = from device in DbContext.Devices
                        join client in DbContext.Clients on device.ClientId equals client.Id
                        join subscriber in DbContext.ClientSubscribers on client.Id equals subscriber.ClientId
                        join user in DbContext.TelegramUsers on subscriber.Phone equals user.Phone
                        where client.IsActive && user.TelegramId == telegramId
                        select new DeviceStatusModel
                        {
                            Id = device.Id,
                            MACAddress = device.MACAddress,
                            IsArmed = device.IsArmed,
                            Status = device.Status,
                            NetworkStatus = device.NetworkStatus,
                            Name = device.Name,
                            Cameras = device.Cameras
                                            .Where(c => c.IsActive)
                                            .OrderBy(c => c.CameraName)
                                            .Select(c => new CameraStatusModel
                                            {
                                                Id = c.Id,
                                                Status = c.Status,
                                                NetworkStatus = c.NetworkStatus,
                                                CameraName = c.CameraName
                                            })
                                            .ToList()
                        };

            return await query.ToListAsync();
        }

        public async Task<UserSettingsModel> GetUserSettings(long telegramId)
        {
            var query = from user in DbContext.TelegramUsers
                        join subscriber in DbContext.ClientSubscribers on user.Phone equals subscriber.Phone
                        join client in DbContext.Clients on subscriber.ClientId equals client.Id
                        where client.IsActive && user.TelegramId == telegramId
                        select new
                        {
                            user.Mute,
                            subscriber.Id
                        };

            var data = await query.ToListAsync();

            if ( !data.Any() ) return null;

            var result = new UserSettingsModel
            {
                Mute = data[0].Mute
            };

            var ids = data.Select(c => c.Id).ToList();
            var settings = await DbContext.ClientSubscriberSettings
                                          .Where(c => ids.Contains(c.ClientSubscriberId))
                                          .Select(c => new SubscriberSettingsModel
                                          {
                                              CameraId = c.CameraId,
                                              Mute = c.Mute
                                          })
                                          .ToListAsync();

            result.Cameras = settings;

            return result;
        }

        public async Task Mute(TelegramMuteModel model)
        {
            var user = await DbContext.TelegramUsers.FirstOrDefaultAsync(c => c.TelegramId == model.TelegramId);

            if ( user != null )
            {
                user.Mute = model.Mute;

                await SaveChangesAsync();
            }
        }

        public async Task SaveCameraSettings(long telegramId, CameraSettingsModel model)
        {
            var query = from user in DbContext.TelegramUsers
                        join subscriber in DbContext.ClientSubscribers on user.Phone equals subscriber.Phone
                        join client in DbContext.Clients on subscriber.ClientId equals client.Id
                        join device in DbContext.Devices on client.Id equals device.Id
                        join camera in DbContext.Cameras on device.Id equals camera.DeviceId
                        where client.IsActive && user.TelegramId == telegramId && camera.Id == model.CameraId
                        select subscriber.Id;

            var subscriberId = await query.SingleOrDefaultAsync();

            if ( subscriberId == default ) return;

            var settings = await DbContext.ClientSubscriberSettings.SingleOrDefaultAsync(c =>
                c.CameraId == model.CameraId && c.ClientSubscriberId == subscriberId);

            if ( settings == null )
            {
                settings = new ClientSubscriberSettings
                {
                    CameraId = model.CameraId,
                    ClientSubscriberId = subscriberId
                };

                DbContext.ClientSubscriberSettings.Add(settings);
            }

            settings.Mute = model.Mute;

            await SaveChangesAsync();
        }

        private Task<bool> HasActiveClients(string phone)
        {
            return DbContext.ClientSubscribers.AnyAsync(c => c.IsActive && c.Phone == phone);
        }
    }
}