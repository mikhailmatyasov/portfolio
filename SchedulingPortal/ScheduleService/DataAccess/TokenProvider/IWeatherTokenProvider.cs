namespace ScheduleService.DataAccess.TokenProvider
{
    public interface IWeatherTokenProvider
    {
        string GetToken();

        string GetProvider();
    }
}
