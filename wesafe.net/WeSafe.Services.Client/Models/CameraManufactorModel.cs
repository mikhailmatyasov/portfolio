using System;
using System.Collections.Generic;
using System.Text;

namespace WeSafe.Services.Client.Models
{
    public class CameraManufactorModel
    {
        public int Id { get; set; }
        public string Manufactor { get; set; }
        public IEnumerable<CameraMarkModel> CameraMarks { get; set; }
    }
}
