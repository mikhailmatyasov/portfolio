using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.DAL;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Services.Extensions;
using WeSafe.Shared.Results;

namespace WeSafe.Services
{
    public class PermittedAdminIpService : BaseService, IPermittedAdminIpService
    {
        private readonly PermittedAdminIpMapper _mapper;

        public PermittedAdminIpService(WeSafeDbContext context, PermittedAdminIpMapper mapper, ILoggerFactory loggerFactory) : base(context, loggerFactory)
        {
            _mapper = mapper;
        }

        public IEnumerable<PermittedAdminIpModel> GetPermittedAdminIps()
        {
            return DbContext.PermittedAdminIps.Select(x => _mapper.ToPermittedAdminIpModel(x)).ToList();
        }

        public async Task<IExecutionResult> CreatePermittedAdminIp(PermittedAdminIpModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (DbContext.PermittedAdminIps.Any(x => x.IpAddress == model.Ip))
                throw new InvalidOperationException(nameof(model.Ip));

            if (!model.Ip.IsValidIp())
                throw new InvalidOperationException("Ip " + model.Ip + " is not valid.");

            var permittesIpAddress = _mapper.ToPermittedAdminIp(model);

            DbContext.PermittedAdminIps.Add(permittesIpAddress);
            await SaveChangesAsync();

            return ExecutionResult.Success();
        }

        public async Task<IExecutionResult> RemovePermittedAdminIp(int ipId)
        {
            var ipAddress = await DbContext.PermittedAdminIps.FirstOrDefaultAsync(x => x.Id == ipId);

            if (ipAddress == null)
                throw new NullReferenceException("Ip " + ipId + " is not found.");

            DbContext.PermittedAdminIps.Remove(ipAddress);
            await SaveChangesAsync();

            return ExecutionResult.Success();
        }
    }
}
