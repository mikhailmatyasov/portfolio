using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using WeSafe.DAL;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;

namespace WeSafe.Services
{
    public class CameraManufactorService : BaseService, ICameraManufactorService
    {
        private readonly CameraManufactorMapper _cameraManufactorMapper;

        public CameraManufactorService(WeSafeDbContext context, CameraManufactorMapper cameraManufactorMapper, ILoggerFactory loggerFactory) : base(context, loggerFactory)
        {
            _cameraManufactorMapper = cameraManufactorMapper;
        }
        public IEnumerable<CameraManufactorModel> GetCameraManufactors()
        {
            var cameraManufactors = DbContext.CameraManufactors.Include(x => x.CameraMarks).ThenInclude(x => x.RtspPaths).ToList();

            return cameraManufactors.Select(x => _cameraManufactorMapper.ToCameraManufactorModel(x)).ToList();
        }
    }
}
