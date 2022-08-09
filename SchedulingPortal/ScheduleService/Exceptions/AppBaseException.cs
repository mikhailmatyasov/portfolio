using System;

namespace ScheduleService.Exceptions
{
    public abstract class AppBaseException : ApplicationException
    {
        protected AppBaseException(string message)
            : base(message)
        {
        }
    }
}
