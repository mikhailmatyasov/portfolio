using System;

namespace WeSafe.DAL.Entities
{
    public class UnhandledException
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public virtual User User { get; set; }

        public string ErrorMessage { get; set; }

        public string StackTrace { get; set; }

        public DateTime DateTime { get; set; }
    }
}
