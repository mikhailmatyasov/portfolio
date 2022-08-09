using System;
using WeSafe.DAL.Entities;

namespace WeSafe.Services.Client.Models
{
    public class RtspPathMapper
    {
        public RtspPathModel ToRtspPathModel(RtspPath rtspPath)
        {
            if (rtspPath == null)
                throw new ArgumentNullException(nameof(rtspPath));

            return new RtspPathModel
            {
                Id = rtspPath.Id,
                CameraMarkId = rtspPath.CameraMarkId,
                Path = rtspPath.Path
            };
        }
    }
}
