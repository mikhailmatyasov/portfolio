using System.Collections.Generic;
using WeSafe.Services.Client.Models;

namespace WeSafe.Services.Client
{
    public interface IRtspPathService
    {
        IEnumerable<RtspPathModel> GetRtspPaths(int cameraMarkId);
    }
}
