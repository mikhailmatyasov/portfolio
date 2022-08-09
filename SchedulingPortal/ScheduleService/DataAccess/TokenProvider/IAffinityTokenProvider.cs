namespace ScheduleService.DataAccess.TokenProvider
{
    public interface IAffinityTokenProvider
    {
        /// <summary>
        /// Generates an Affinity API access token.
        /// </summary>
        string GenerateAffinityApiToken();
    }
}
