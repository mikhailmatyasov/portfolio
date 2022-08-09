using System;
using System.Linq;
using WeSafe.DAL.Entities;
using WeSafe.Services.Client.Models;

namespace WeSafe.Services
{
    public class CameraManufactorMapper
    { 
        private readonly CameraMarkMapper _cameraMarkMapper;

        public CameraManufactorMapper(CameraMarkMapper cameraMarkMapper)
        {
            _cameraMarkMapper = cameraMarkMapper;
        }

        public CameraManufactorModel ToCameraManufactorModel(CameraManufacturer cameraManufacturer)
        {
            if (cameraManufacturer == null)
                throw new ArgumentNullException(nameof(cameraManufacturer));

            return new CameraManufactorModel
            {
                Id = cameraManufacturer.Id,
                Manufactor = cameraManufacturer.Manufacturer,
                CameraMarks = cameraManufacturer.CameraMarks.Select(x => _cameraMarkMapper.ToCameraManufactorModel(x)).ToList()
            };
        }
    }
}
