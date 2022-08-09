using System.Collections.Generic;

namespace WeSafe.DAL.Entities
{
    public class CameraManufacturer
    {
        public int Id { get; set; }

        public string Manufacturer { get; set; }

        public virtual ICollection<CameraMark> CameraMarks { get; set; }
    }
}
