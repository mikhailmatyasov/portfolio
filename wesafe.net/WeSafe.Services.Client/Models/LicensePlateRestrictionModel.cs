using WeSafe.Shared.Enumerations;

namespace WeSafe.Services.Client.Models
{
    public class LicensePlateRestrictionModel
    {
        public int Id { get; set; }

        public string LicensePlate { get; set; }

        public LicensePlateType LicensePlateType { get; set; }
    }
}
