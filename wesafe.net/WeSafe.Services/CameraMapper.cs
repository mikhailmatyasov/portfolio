using System;
using System.Linq.Expressions;
using WeSafe.DAL.Entities;
using WeSafe.Services.Client.Models;
using WeSafe.Shared.Extensions;

namespace WeSafe.Services
{
    public class CameraMapper
    {
        private readonly Func<Camera, CameraModel> _compiledProjection;

        public CameraMapper()
        {
            _compiledProjection = Projection.Compile();
        }

        public Expression<Func<Camera, CameraModel>> Projection => camera => new CameraModel
        {
            Id = camera.Id,
            CameraName = camera.CameraName,
            Port = camera.Port,
            Ip = camera.Ip,
            IsActive = camera.IsActive,
            Login = camera.Login,
            Password = camera.Password.Decrypt(),
            Roi = camera.Roi,
            Schedule = camera.Schedule,
            SpecificRtcpConnectionString = camera.SpecificRtcpConnectionString.Decrypt(),
            DeviceId = camera.DeviceId,
            LastImagePath = camera.LastImagePath,
            Status = camera.Status != null ? camera.Status.ToUpper() : null,
            NetworkStatus = camera.NetworkStatus != null ? camera.NetworkStatus.ToUpper() : null,
            RecognitionSettings = camera.RecognitionSettings
        };

        public CameraModel ToCameraModel(Camera camera)
        {
            if (camera == null) throw new ArgumentNullException(nameof(camera));

            return _compiledProjection(camera);
        }

        public Camera ToCamera(CameraModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            return new Camera
            {
                Id = model.Id,
                CameraName = model.CameraName,
                Port = model.Port,
                Ip = model.Ip,
                IsActive = model.IsActive,
                Login = model.Login,
                Password = model.Password.Encrypt(),
                Roi = model.Roi,
                Schedule = model.Schedule,
                SpecificRtcpConnectionString = model.SpecificRtcpConnectionString.Encrypt(),
                DeviceId = model.DeviceId,
                Status = model.Status,
                NetworkStatus = model.NetworkStatus,
                RecognitionSettings = model.RecognitionSettings
            };
        }

        public Camera ToCamera(Camera camera, CameraModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (camera == null) throw new ArgumentNullException(nameof(camera));

            camera.CameraName = model.CameraName;
            camera.Port = model.Port;
            camera.Ip = model.Ip;
            camera.IsActive = model.IsActive;
            camera.Login = model.Login;
            camera.Password = model.Password.Encrypt();
            camera.Roi = model.Roi;
            camera.Schedule = model.Schedule;
            camera.SpecificRtcpConnectionString = model.SpecificRtcpConnectionString.Encrypt();
            camera.RecognitionSettings = model.RecognitionSettings;

            return camera;
        }
    }
}