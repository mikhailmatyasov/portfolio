namespace WeSafe.Services.Client.Models
{
    public class CamerasStatModel
    {
        public int Count { get; set; }

        public int ActiveCount { get; set; }

        public int? MaxActiveCameras { get; set; }
    }
}