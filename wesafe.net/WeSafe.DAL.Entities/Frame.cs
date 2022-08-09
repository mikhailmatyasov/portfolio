using WeSafe.Shared.Enumerations;

namespace WeSafe.DAL.Entities
{
    public class Frame
    {
        public int Id { get; set; }

        public int PlateEventId { get; set; }

        public PlateEvent PlateEvent { get; set; }

        public ImageType ImageType { get; set; }

        public string ImageUrl { get; set; }

    }
}
