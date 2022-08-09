using WeSafe.Shared.Enumerations;

namespace WeSafe.DAL.Entities
{
    public class LicensePlateRestriction
    {
        public int Id { get; set; }

        public string LicensePlate { get; set; }

        public int DeviceId { get; set; }

        public virtual Device Device { get; set; }

        public LicensePlateType LicensePlateType { get; set; }
    }
}
