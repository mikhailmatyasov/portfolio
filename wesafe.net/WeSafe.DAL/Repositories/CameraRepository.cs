using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.DAL.Abstractions;
using WeSafe.DAL.Entities;
using WeSafe.DAL.Repositories.Extended;

namespace WeSafe.DAL.Repositories
{
    public class CameraRepository : ExtendedRepository<Camera>, ICameraRepository
    {
        public CameraRepository(WeSafeDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Camera>> GetCamerasByDeviceIdAsync(int deviceId)
        {
            return await Get(predicate: c => c.DeviceId == deviceId, selector: c => c);
        }

        public async Task<IEnumerable<Camera>> GetCamerasNames(IEnumerable<int> ids)
        {
            return await Get(predicate: c => ids.Contains(c.Id), selector: c => new Camera()
            {
                Id = c.Id,
                CameraName = c.CameraName
            });
        }
    }
}
