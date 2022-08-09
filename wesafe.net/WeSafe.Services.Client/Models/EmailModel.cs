namespace WeSafe.Services.Client.Models
{
    public class EmailModel
    {
        public int Id { get; set; }

        public string MailAddress { get; set; }

        public bool NotifyServerException { get; set; }
    }
}
