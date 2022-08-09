namespace WeSafe.Monitoring.Services
{
    public class MonitoringOptions
    {
        public string TelegramToken { get; set; }

        public long TelegramChannelId { get; set; }

        public int Delay { get; set; }

        public MonitoringOptions()
        {
            Delay = 3;
        }
    }
}