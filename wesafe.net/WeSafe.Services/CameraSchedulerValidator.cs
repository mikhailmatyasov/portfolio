using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.Services.Client;
using WeSafe.Shared;
using WeSafe.Shared.Results;
using Camera = WeSafe.DAL.Entities.Camera;

namespace WeSafe.Services
{
    public class CameraSchedulerValidator
    {
        /// <summary>
        /// Validates the specified <paramref name="camera"/> as an asynchronous operation.
        /// </summary>
        /// <param name="service">The <see cref="ICameraService"/> that can be used to retrieve camera properties.</param>
        /// <param name="camera">The camera to validate.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IExecutionResult"/> of the validation operation.</returns>
        public async Task<IExecutionResult> ValidateAsync(ICameraService service, Camera camera)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            if (camera == null) throw new ArgumentNullException(nameof(camera));

            var errors = new List<string>();

            if (String.IsNullOrWhiteSpace(camera.CameraName)) errors.Add("Camera name is required.");
            if (camera.CameraName != null && camera.CameraName.Length > 250) errors.Add("Camera name must be less that 250 chars.");

            if (String.IsNullOrWhiteSpace(camera.Ip)) errors.Add("Camera IP is required.");
            if (camera.Ip != null && camera.Ip.Length > 16) errors.Add("Camera IP must be less that 16 chars.");

            if (String.IsNullOrWhiteSpace(camera.Port)) errors.Add("Camera port is required.");
            if (camera.Port != null && camera.Port.Length > 10) errors.Add("Camera port must be less that 10 chars.");

            if (!String.IsNullOrEmpty(camera.Login) && camera.Login.Length > 100) errors.Add("Camera login must be less that 100 chars.");

            if (!String.IsNullOrEmpty(camera.Password) && camera.Password.Length > 50) errors.Add("Camera password must be less that 50 chars.");

            if (errors.Any()) return ExecutionResult.Failed(errors);

            var deviceCameras = service.Cameras.Where(c => c.DeviceId == camera.DeviceId);
            var exist = await deviceCameras.FirstOrDefaultAsync(c => c.Ip == camera.Ip && c.Port == camera.Port);

            if (exist != null && camera.Id != exist.Id) return ExecutionResult.Failed($"Camera with such ip and port already exists.");

            if (!String.IsNullOrWhiteSpace(camera.Schedule))
            {
                try
                {
                    var scheduler = SchedulerSerializer.Deserialize(camera.Schedule);
                }
                catch (Exception e)
                {
                    return ExecutionResult.Failed(e.Message);
                }
            }

            return ExecutionResult.Success();
        }
    }
}