using System;
using System.Linq;
using WeSafe.DAL.Entities;
using WeSafe.Services.Client.Models;

namespace WeSafe.Services
{
    public class CameraMarkMapper
    {
        private readonly RtspPathMapper _rtspPathMapper;

        public CameraMarkMapper(RtspPathMapper rtspPathMapper)
        {
            _rtspPathMapper = rtspPathMapper;
        }

        public CameraMarkModel ToCameraManufactorModel(CameraMark cameraMark)
        {
            if (cameraMark == null)
                throw new ArgumentNullException(nameof(cameraMark));

            return new CameraMarkModel
            {
                Id = cameraMark.Id,
                CameraManufactorId = cameraMark.CameraManufactorId,
                Model = cameraMark.Model,
                RtspPaths = cameraMark.RtspPaths.Select(x => _rtspPathMapper.ToRtspPathModel(x)).ToList()
            };
        }
    }
}
