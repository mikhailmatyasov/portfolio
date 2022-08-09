using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WeSafe.IntegrationTests.Account;
using WeSafe.IntegrationTests.Base;
using WeSafe.Services.Client.Models;
using Xunit;
using Newtonsoft.Json;

namespace WeSafe.IntegrationTests.Cameras
{
    public class CameraBaseTests : BaseTest
    {
        #region Fields

        protected const string _baseUrl = "/api/cameras/";

        #endregion

        #region Ctor

        public CameraBaseTests(MediaGalleryFactory<StubStartup> factory) : base(factory)
        {
        }

        #endregion

        #region Public Methods

        public CameraModel GetValidCameraModel(bool isActive = false)
        {
            CameraModel cameraModel = new CameraModel
            {
                CameraName = GetRandomString(),
                Ip = GetRandomIpAddress(),
                Port = "80",
                Login = GetRandomString(),
                Password = GetRandomString(),
                RecognitionSettings = GenerateCameraRecognitionSettings(),
                IsActive = isActive
            };

            cameraModel.SpecificRtcpConnectionString = $"rtsp://{cameraModel.Login}:{cameraModel.Password}@{cameraModel.Ip}:{cameraModel.Port}";
            
            return cameraModel;
        }

        public IEnumerable<CameraModel> GetInvalidCameraModels() => new List<CameraModel>()
        {
            null,
            new CameraModel{Ip = GetRandomIpAddress(), Port = "80", Login = GetRandomString(), Password = GetRandomString(),
                 RecognitionSettings = GenerateCameraRecognitionSettings(), SpecificRtcpConnectionString = "rtsp://login:pass@ip:port"},
            new CameraModel{CameraName = "", Ip = GetRandomIpAddress(), Port = "80", Login = GetRandomString(), Password = GetRandomString(),
                 RecognitionSettings = GenerateCameraRecognitionSettings(), SpecificRtcpConnectionString = "rtsp://login:pass@ip:port"},
            new CameraModel{CameraName = GetRandomString(), Port = "80", Login = GetRandomString(), Password = GetRandomString(),
                 RecognitionSettings = GenerateCameraRecognitionSettings(), SpecificRtcpConnectionString = "rtsp://login:pass@ip:port"},
            new CameraModel{CameraName = GetRandomString(), Ip = "", Port = "80", Login = GetRandomString(), Password = GetRandomString(),
                 RecognitionSettings = GenerateCameraRecognitionSettings(), SpecificRtcpConnectionString = "rtsp://login:pass@ip:port"},
            new CameraModel{CameraName = GetRandomString(), Ip ="122.2", Port = "80", Login = GetRandomString(), Password = GetRandomString(),
                 RecognitionSettings = GenerateCameraRecognitionSettings(), SpecificRtcpConnectionString = "rtsp://login:pass@ip:port"},
            new CameraModel{CameraName = GetRandomString(), Ip = GetRandomIpAddress(), Login = GetRandomString(), Password = GetRandomString(),
                 RecognitionSettings = GenerateCameraRecognitionSettings(), SpecificRtcpConnectionString = "rtsp://login:pass@ip:port"},
            new CameraModel{CameraName = GetRandomString(), Ip = GetRandomIpAddress(), Port = "", Login = GetRandomString(), Password = GetRandomString(),
                 RecognitionSettings = GenerateCameraRecognitionSettings(), SpecificRtcpConnectionString = "rtsp://login:pass@ip:port"},
            new CameraModel{CameraName = GetRandomString(), Ip = GetRandomIpAddress(), Port = "sada213", Login = GetRandomString(), Password = GetRandomString(),
                 RecognitionSettings = GenerateCameraRecognitionSettings(), SpecificRtcpConnectionString = "rtsp://login:pass@ip:port"},           
            new CameraModel{CameraName = GetRandomString(), Ip = GetRandomIpAddress(), Port = "80", Password = GetRandomString(),
                 RecognitionSettings = GenerateCameraRecognitionSettings(), SpecificRtcpConnectionString = "rtsp://login:pass@ip:port"},
            new CameraModel{CameraName = GetRandomString(), Ip = GetRandomIpAddress(), Port = "80", Login ="", Password = GetRandomString(),
                 RecognitionSettings = GenerateCameraRecognitionSettings(), SpecificRtcpConnectionString = "rtsp://login:pass@ip:port"},
            new CameraModel{CameraName = GetRandomString(), Ip = GetRandomIpAddress(), Port = "80", Login = GetRandomString(),
                 RecognitionSettings = GenerateCameraRecognitionSettings(), SpecificRtcpConnectionString = "rtsp://login:pass@ip:port"},
            new CameraModel{CameraName = GetRandomString(), Ip = GetRandomIpAddress(), Port = "80", Login = GetRandomString(), Password = "",
                 RecognitionSettings = GenerateCameraRecognitionSettings(), SpecificRtcpConnectionString = "rtsp://login:pass@ip:port"},
            new CameraModel{CameraName = GetRandomString(), Ip = GetRandomIpAddress(), Port = "80", Login = GetRandomString(), Password = GetRandomString(),
                 SpecificRtcpConnectionString = "rtsp://login:pass@ip:port"},
            new CameraModel{CameraName = GetRandomString(), Ip = GetRandomIpAddress(), Port = "80", Login = GetRandomString(), Password = GetRandomString(),
                 RecognitionSettings = "", SpecificRtcpConnectionString = "rtsp://login:pass@ip:port"},
            new CameraModel{CameraName = GetRandomString(), Ip = GetRandomIpAddress(), Port = "80", Login = GetRandomString(), Password = GetRandomString(),
                 RecognitionSettings = "unparsed", SpecificRtcpConnectionString = "rtsp://login:pass@ip:port"},
            new CameraModel{CameraName = GetRandomString(), Ip = GetRandomIpAddress(), Port = "80", Login = GetRandomString(), Password = GetRandomString(),
                 RecognitionSettings = GenerateCameraRecognitionSettings()},
            new CameraModel{CameraName = GetRandomString(), Ip = GetRandomIpAddress(), Port = "80", Login = GetRandomString(), Password = GetRandomString(),
                 RecognitionSettings = GenerateCameraRecognitionSettings(), SpecificRtcpConnectionString = ""}       
        };

        #endregion

        #region Protected 

        protected string GenerateCameraRecognitionSettings()
        {
            Random random = new Random();

            CameraRecognitionSettings cameraRecognitionSettings = new CameraRecognitionSettings
            {
                Confidence = random.Next(70, 100),
                Sensitivity = random.Next(0, 10),
                AlertFrequency = random.Next(1, 60)
            };

            return JsonConvert.SerializeObject(cameraRecognitionSettings).ToLower();
        }
    #endregion
    }

    public class CameraRecognitionSettings
    {
        public int Confidence { get; set; }

        public int Sensitivity { get; set; }

        public int AlertFrequency { get; set; }
    }
}
