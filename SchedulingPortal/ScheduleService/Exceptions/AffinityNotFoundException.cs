namespace ScheduleService.Exceptions
{
    public class AffinityNotFoundException : AppBaseException
    {
        public AffinityNotFoundException(string message)
            : base(message)
        {
        }
    }
}
