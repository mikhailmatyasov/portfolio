using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using WeSafe.DAL;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using System;

namespace WeSafe.Services
{
    public class RtspPathService : BaseService, IRtspPathService
    {
        private readonly RtspPathMapper _rtspPathMapper;

        public RtspPathService(WeSafeDbContext context, RtspPathMapper rtspPathMapper, ILoggerFactory loggerFactory) : base(context, loggerFactory)
        {
            _rtspPathMapper = rtspPathMapper;
        }
        
        public IEnumerable<RtspPathModel> GetRtspPaths(int cameraMarkId)
        {
            if (!DbContext.CameraMarks.Where(m => m.Id == cameraMarkId).Any())
                throw new InvalidOperationException("Camera mark with id " + cameraMarkId + " is not found");

            return DbContext.RtspPaths.Where(x => x.CameraMarkId == cameraMarkId).Select(x => _rtspPathMapper.ToRtspPathModel(x)).ToList();
        }
    }
}