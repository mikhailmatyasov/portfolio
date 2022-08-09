using System.Collections.Generic;

namespace WeSafe.DAL.Entities
{
    public class CameraMark
    {
        public int Id { get; set; }

        public int CameraManufactorId { get; set; }

        public virtual CameraManufacturer CameraManufacturer { get; set; }

        public string Model { get; set; }

        public virtual ICollection<RtspPath> RtspPaths { get; set; }
    }
}
