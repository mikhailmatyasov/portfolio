using System;
using System.Collections.Generic;
using System.Text;
using WeSafe.Services.Client.Models;

namespace WeSafe.Services.Client
{
    public interface ICameraManufactorService
    {
        IEnumerable<CameraManufactorModel> GetCameraManufactors();
    }
}
