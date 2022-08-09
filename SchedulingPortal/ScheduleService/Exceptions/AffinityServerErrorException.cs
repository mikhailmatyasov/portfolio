namespace ScheduleService.Exceptions
{
    public class AffinityServerErrorException : AppBaseException
    {
        public AffinityServerErrorException(string message)
            : base(message)
        {
        }
    }
}
