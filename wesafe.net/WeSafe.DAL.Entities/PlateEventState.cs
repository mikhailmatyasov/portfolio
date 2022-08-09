using WeSafe.Shared.Enumerations;

namespace WeSafe.DAL.Entities
{
    public class PlateEventState
    {
        public int Id { get; set; }

        public int PlateEventId { get; set; }

        public virtual PlateEvent PlateEvent { get; set; }

        public LicensePlateType State { get; set; }
    }
}
