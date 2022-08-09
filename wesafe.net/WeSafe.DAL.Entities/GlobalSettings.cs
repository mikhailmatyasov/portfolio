namespace WeSafe.DAL.Entities
{
    public class GlobalSettings
    {
        public int Id { get; set; }

        public int KeepingDeviceLogsDays { get; set; }

        public int KeepingCameraLogsDays { get; set; }
    }
}