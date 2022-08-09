namespace WeSafe.DAL.Entities
{
    public class Email
    {
        public int Id { get; set; }

        public string MailAddress { get; set; }

        public bool NotifyServerException { get; set; }
    }
}
