using System.Collections.Generic;
using System.Linq;
using WeSafe.Services.Client.Models;

namespace WeSafe.Services.Extensions
{
    public static class AssignmentsExtensions
    {
        public static bool IsAssignmentAllowed(this IEnumerable<AssignmentModel> assignments, int deviceId, int cameraId)
        {
            return !assignments.Any() ||
                   assignments.Any(c => c.DeviceId == deviceId && (c.CameraId == null || c.CameraId == cameraId));
        }
    }
}