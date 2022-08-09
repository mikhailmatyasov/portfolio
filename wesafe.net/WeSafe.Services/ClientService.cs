using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeZoneConverter;
using WeSafe.DAL;
using WeSafe.DAL.Entities;
using WeSafe.DAL.Extensions;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Services.Extensions;
using WeSafe.Shared;
using WeSafe.Shared.Extensions;
using WeSafe.Shared.Results;

namespace WeSafe.Services
{
    public class ClientService : BaseService, IClientService
    {
        private readonly ClientMapper _mapper;

        public ClientService(WeSafeDbContext context, ClientMapper mapper, ILoggerFactory loggerFactory) : base(context, loggerFactory)
        {
            _mapper = mapper;
        }

        public async Task<PageResponse<ClientModel>> GetClients(PageRequest request)
        {
            var query = DbContext.Clients.Select(_mapper.Projection);

            if (!String.IsNullOrWhiteSpace(request.SearchText))
            {
                query = query.Where(c =>
                    c.Name.Contains(request.SearchText) || c.Phone.Contains(request.SearchText) || c.Token.Contains(request.SearchText) ||
                    c.ContractNumber.Contains(request.SearchText));
            }

            var result = await query.OrderBy(c => c.Id).ApplyPageRequest(request);

            return new PageResponse<ClientModel>
            {
                Items = await result.Query.ToListAsync(),
                Total = result.Total
            };
        }

        public async Task<ClientModel> GetClientById(int clientId)
        {
            var client = await DbContext.Clients.FindAsync(clientId);

            if (client == null) return null;

            var result = _mapper.ToClientModel(client);

            return result;
        }

        public async Task<IExecutionResult> CreateClient(ClientModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            if (await DbContext.Clients.AnyAsync(c => c.Phone == model.Phone))
            {
                return ExecutionResult.Failed("EXIST");
            }

            var client = _mapper.ToClient(model);

            DbContext.Clients.Add(client);

            await SaveChangesAsync();

            return ExecutionResult.Payload(client.Id);
        }

        public async Task<IExecutionResult> UpdateClient(ClientModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var client = await DbContext.Clients.FindAsync(model.Id);

            if (client == null) return ExecutionResult.Failed("NOTFOUND");

            if (await DbContext.Clients.AnyAsync(c => c.Phone == model.Phone && c.Id != model.Id))
            {
                return ExecutionResult.Failed("EXIST");
            }

            client = _mapper.ToClient(client, model);

            DbContext.Clients.Update(client);

            await SaveChangesAsync();

            return ExecutionResult.Payload(client.Id);
        }

        public async Task<IExecutionResult> RemoveClient(int clientId)
        {
            var client = await DbContext.Clients.FindAsync(clientId);

            if (client == null) return ExecutionResult.Failed("NOTFOUND");

            DbContext.Clients.Remove(client);

            await SaveChangesAsync();

            return ExecutionResult.Success();
        }

        public async Task<IExecutionResult> SignUpClient(ClientModel model, string timeZone, string password)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!model.Phone.IsValidPhoneNumber())
                throw new InvalidOperationException("Phone " + model.Phone + " is not valid");

            if (await DbContext.Clients.AnyAsync(c => c.Phone == model.Phone))
                return ExecutionResult.Failed("EXIST");

            var token = model.Token.Trim().ToLower();

            var device = await DbContext.Devices.SingleOrDefaultAsync(c => c.Token.ToLower() == token);

            if (device == null)
                return ExecutionResult.Failed("DEVICENOTFOUND");

            if (device.ClientId != null)
                return ExecutionResult.Failed("DEVICEACTIVATED");

            var client = _mapper.ToClient(model);

            client.SendToDevChat = false;

            device.Client = client;
            device.DeviceType = model.DeviceType;
            device.ActivationDate = DateTimeOffset.UtcNow;
            device.TimeZone = GetTimeZoneId(timeZone);

            client.Subscribers = new List<ClientSubscriber>
            {
                new ClientSubscriber
                {
                    Phone = model.Phone,
                    Password = password,
                    Permissions = "OWNER",
                    IsActive = true,
                    CreatedAt = DateTimeOffset.UtcNow
                }
            };

            DbContext.Clients.Add(client);

            await SaveChangesAsync();

            return ExecutionResult.Payload(client.Id);
        }

        public async Task<SystemSettingsModel> GetSystemSettings(int clientId)
        {
            var query = from device in DbContext.Devices
                        join client in DbContext.Clients on device.ClientId equals client.Id
                        where client.IsActive && client.Id == clientId
                        select new DeviceSettingsModel
                        {
                            Id = device.Id,
                            MACAddress = device.MACAddress,
                            LocalIp = device.ClientNetworkIp,
                            IsArmed = device.IsArmed,
                            Status = device.Status,
                            Name = device.Name,
                            Cameras = device.Cameras.Select(x => new SettingsModelEx
                            {
                                CameraId = x.Id,
                                CameraIp = x.Ip,
                                CameraName = x.CameraName,
                                IsActive = x.IsActive,
                                Status = x.Status,
                                Rtsp = x.SpecificRtcpConnectionString.Decrypt()
                            }).ToList()
                        };

            var devices = await query.ToListAsync();

            return new SystemSettingsModel
            {
                Devices = devices
            };
        }

        public async Task<PageResponse<CameraLogModel>> GetEvents(int clientId, EventBaseRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            request.Prepare();
            request.Validate();

            var events = await GetFilteredEventsAsync(clientId, request);

            var total = events.Count();

            return new PageResponse<CameraLogModel>(events, total);
        }

        protected virtual async Task<IEnumerable<CameraLogModel>> GetFilteredEventsAsync(int clientId, EventBaseRequest request)
        {
            var entityQuery = GetBaseEntityQuery(clientId).ApplyDateFilterQuery(request).ApplyEventsFilterQuery(request);

            var events = await entityQuery.Select(cameraLog => new CameraLogModel
            {
                Id = cameraLog.Id,
                CameraId = cameraLog.CameraId,
                CameraName = cameraLog.Camera.CameraName,
                DeviceId = cameraLog.Camera.Device.Id,
                DeviceName = cameraLog.Camera.Device.Name,
                Message = cameraLog.Message,
                Time = cameraLog.Time,
                Alert = cameraLog.Alert,
                Entries = cameraLog.Entries.Select(c => new CameraLogEntryModel
                {
                    Id = c.Id,
                    CameraLogId = c.CameraLogId,
                    TypeKey = c.TypeKey,
                    ImageUrl = c.ImageUrl
                })
            }).ToListAsync();

            return events;
        }

        protected IQueryable<CameraLog> GetBaseEntityQuery(int clientId)
        {
            return DbContext.CameraLogs.Include(c => c.Entries).Include(c => c.Camera).ThenInclude(c => c.Device)
                .Where(c => c.Camera.Device.ClientId == clientId);
        }

        private string GetTimeZoneId(string timeZone)
        {
            if ( String.IsNullOrEmpty(timeZone) )
            {
                return null;
            }

            var tz = TZConvert.GetTimeZoneInfo(timeZone);

            return tz.Id;
        }
    }
}