
namespace WeSafe.Web.Core.Models
{
    public class DeviceFilterModel : PageFilter
    {
        public int? ClientId { get; set; }
        public int? FilterBy { get; set; }
        public string Search { get; set; }
        public string Sort { get; set; }
    }
}
