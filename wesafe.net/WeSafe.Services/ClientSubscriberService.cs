using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.DAL;
using WeSafe.DAL.Entities;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Services.Extensions;
using WeSafe.Shared.Results;

namespace WeSafe.Services
{
    public class ClientSubscriberService : BaseService, IClientSubscriberService
    {
        private readonly ClientSubscriberMapper _mapper;

        public ClientSubscriberService(WeSafeDbContext context, ClientSubscriberMapper mapper, ILoggerFactory loggerFactory) : base(context, loggerFactory)
        {
            _mapper = mapper;
        }

        public async Task<IEnumerable<ClientSubscriberModel>> GetClientSubscribers(int clientId)
        {
            var result = await DbContext.ClientSubscribers
                                        .Where(c => c.ClientId == clientId)
                                        .OrderBy(c => c.Name)
                                        .Select(_mapper.Projection)
                                        .ToListAsync();

            return result;
        }

        public async Task<ClientSubscriberModel> GetClientSubscriberById(int clientId, int subscriberId)
        {
            var result = await DbContext.ClientSubscribers
                                        .Where(c => c.ClientId == clientId && c.Id == subscriberId)
                                        .Select(_mapper.Projection)
                                        .SingleOrDefaultAsync();

            return result;
        }

        public async Task<IExecutionResult> CreateClientSubscriber(int clientId, ClientSubscriberModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            if (!model.Phone.IsValidPhoneNumber())
                throw new InvalidOperationException("Phone " + model.Phone + " is not valid");

            if (await DbContext.ClientSubscribers.AnyAsync(c => c.Phone == model.Phone && c.ClientId == clientId))
            {
                return ExecutionResult.Failed("EXIST");
            }

            model.IsActive = true;
            model.CreatedAt = DateTimeOffset.UtcNow;

            var user = _mapper.ToClientSubscriber(model);

            DbContext.ClientSubscribers.Add(user);

            await SaveChangesAsync();

            return ExecutionResult.Payload(user.Id);
        }

        public async Task<IExecutionResult> UpdateClientSubscriber(int clientId, ClientSubscriberModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            if (!model.Phone.IsValidPhoneNumber())
                throw new InvalidOperationException("Phone " + model.Phone + " is not valid");

            var user = await DbContext.ClientSubscribers.FindAsync(model.Id);

            if (user == null) return ExecutionResult.Failed("NOTFOUND");

            if (await DbContext.ClientSubscribers.AnyAsync(c => c.Phone == model.Phone && c.ClientId == clientId && c.Id != model.Id))
            {
                return ExecutionResult.Failed("EXIST");
            }

            user = _mapper.ToClientSubscriber(user, model);

            DbContext.ClientSubscribers.Update(user);

            await SaveChangesAsync();

            return ExecutionResult.Payload(user.Id);
        }

        public async Task<IExecutionResult> RemoveClientSubscriber(int clientId, int subscriberId)
        {
            var user = await DbContext.ClientSubscribers.FirstOrDefaultAsync(c => c.ClientId == clientId && c.Id == subscriberId);

            if (user == null) return ExecutionResult.Failed("NOTFOUND");

            DbContext.ClientSubscribers.Remove(user);

            await SaveChangesAsync();

            return ExecutionResult.Success();
        }

        public async Task<IEnumerable<AssignmentModel>> GetSubscriberAssignments(int clientId, int subscriberId)
        {
            var result = await DbContext.ClientSubscribers
                                        .Where(c => c.ClientId == clientId && c.Id == subscriberId)
                                        .SelectMany(c => c.Assignments.Select(x => new AssignmentModel
                                        {
                                            Id = x.Id,
                                            DeviceId = x.DeviceId,
                                            DeviceName = x.Device.Name,
                                            CameraId = x.CameraId,
                                            CameraName = x.Camera != null ? x.Camera.CameraName : null
                                        }))
                                        .ToListAsync();

            return result;
        }

        public async Task SaveSubscriberAssignments(int clientId, int subscriberId, IEnumerable<AssignmentModel> assignments)
        {
            var exists = await DbContext.ClientSubscriberAssignments
                                        .Where(c => c.ClientSubscriberId == subscriberId)
                                        .ToListAsync();
            var ids = assignments.Select(c => c.Id).ToList();

            foreach (var exist in exists.Where(c => !ids.Contains(c.Id)))
            {
                DbContext.ClientSubscriberAssignments.Remove(exist);
            }

            foreach (var assignment in assignments)
            {
                var exist = exists.SingleOrDefault(c => c.Id == assignment.Id);

                if (exist == null)
                {
                    exist = new ClientSubscriberAssignment
                    {
                        ClientSubscriberId = subscriberId
                    };

                    DbContext.ClientSubscriberAssignments.Add(exist);
                }

                exist.DeviceId = assignment.DeviceId;
                exist.CameraId = assignment.CameraId;
            }

            await SaveChangesAsync();
        }
    }
}