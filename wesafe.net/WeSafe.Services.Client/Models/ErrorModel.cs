namespace WeSafe.Services.Client.Models
{
    public class ErrorModel
    {
        public int Code { get; set; }

        public string UserId { get; set; }

        public string ErrorMessage { get; set; }

        public string StackTrace { get; set; }
    }
}
