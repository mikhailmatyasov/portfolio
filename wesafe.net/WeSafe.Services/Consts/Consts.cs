namespace WeSafe.Services.Consts
{
    public class Consts
    {
        public const string ipRegularString = @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b";
        public const string macAddressRegularString = @"^([0-9A-Fa-f]{2}:){5}([0-9A-Fa-f]{2})$";
        public const string phoneNumberRegularString = @"^\+\d{11,12}$";

        public const string defaultUserName = "Anonymous";
    }
}
