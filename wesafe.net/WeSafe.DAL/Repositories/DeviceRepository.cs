using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.DAL.Abstractions;
using WeSafe.DAL.Entities;
using WeSafe.DAL.Repositories.Extended;

namespace WeSafe.DAL.Repositories
{
    /// <inheritdoc cref="IDeviceRepository"/>
    public class DeviceRepository : ExtendedRepository<Device>, IDeviceRepository
    {
        #region Constructors

        public DeviceRepository(WeSafeDbContext dbContext) : base(dbContext)
        {
        }

        #endregion

        #region IDeviceRepository implementation

        public Task<Device> FindByMacAddressAsync(string macAddress, bool disableTracking = false)
        {
            if (String.IsNullOrEmpty(macAddress))
            {
                throw new ArgumentNullException(nameof(macAddress));
            }

            var query = GetQueryable(disableTracking);

            return query.FirstOrDefaultAsync(c => c.MACAddress.ToLower() == macAddress.ToLower());
        }

        public Task<Device> FindByTokenAsync(string token, bool disableTracking = false)
        {
            if (String.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            var query = GetQueryable(disableTracking);

            return query.FirstOrDefaultAsync(c => c.Token.ToLower() == token.ToLower());
        }

        public async Task<IEnumerable<Device>> FindAllByClientIdAsync(int clientId)
        {
            return await _dbSet.Where(c => c.ClientId == clientId)
                               .OrderBy(c => c.Name)
                               .ThenBy(c => c.ActivationDate)
                               .ToListAsync();
        }

        public Task<Device> FindByClientIdAsync(int clientId, int deviceId)
        {
            return _dbSet.FirstOrDefaultAsync(c => c.ClientId == clientId && c.Id == deviceId);
        }

        public async Task<IEnumerable<Device>> GetDevicesWithNames(IEnumerable<int> ids)
        {
            return await Get(predicate: c => ids.Contains(c.Id), selector: c => new Device()
            {
                Id = c.Id,
                Name = c.Name
            });
        }

        #endregion
    }
}