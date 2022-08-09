using System;
using System.Collections.Generic;
using WeSafe.Nano.Services.Abstraction.Models;

namespace WeSafe.Nano.Services.Abstraction.Abstraction.Services
{
    public interface IVideoRecordsService
    {
        IEnumerable<string> GetVideoFilesPaths(VideoRecordSearchModel videoRecordSearchModel);

        string GetVideoFile(int cameraId, DateTime eventDate);
    }
}
