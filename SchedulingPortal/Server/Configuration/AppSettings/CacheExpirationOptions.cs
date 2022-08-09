namespace Server.Configuration.AppSettings
{
    public class CacheExpirationOptions
    {
        public const string SectionName = "CacheExpiration";

        public int WeatherTimeoutInHours { get; set; }

        public int StaticDataTimeoutMinutes { get; set; }

        public int MediumDataTimeoutMinutes { get; set; }

        public int FluidDataTimeoutMinutes { get; set; }
    }
}
