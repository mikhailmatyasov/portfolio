using System;
using System.Collections.Generic;
using System.Text;

namespace WeSafe.Services.Client.Models
{
    public class CameraMarkModel
    {
        public int Id { get; set; }
        public int CameraManufactorId { get; set; }
        public string Model { get; set; }
        public IEnumerable<RtspPathModel> RtspPaths { get; set; }
    }
}
