namespace WeSafe.Services.Client.Models
{
    public class RecognitionSettings
    {
        public int Confidence { get; set; }

        public int Sensitivity { get; set; }

        public int AlertFrequency { get; set; }
    }
}