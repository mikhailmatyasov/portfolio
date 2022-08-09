namespace WeSafe.Services.Client.Models
{
    public class CleanupLogsOptions
    {
        public CleanupLogsOptions()
        {
            KeepingDeviceLogsDays = 7;
            KeepingCameraLogsDays = 30;
            KeepingUnhandledExceptionsDays = 7;
        }

        public int KeepingDeviceLogsDays { get; set; }

        public int KeepingCameraLogsDays { get; set; }

        public int KeepingUnhandledExceptionsDays { get; set; }
    }
}