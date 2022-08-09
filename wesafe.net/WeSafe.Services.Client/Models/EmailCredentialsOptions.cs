namespace WeSafe.Services.Client.Models
{
    public class EmailCredentialsOptions
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string SmtpHost { get; set; }

        public int SmtpPort { get; set; }
    }
}
