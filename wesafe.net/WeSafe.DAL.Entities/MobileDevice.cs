namespace WeSafe.DAL.Entities
{
    public class MobileDevice
    {
        public int Id { get; set; }

        public int MobileUserId { get; set; }

        public MobileUser MobileUser { get; set; }

        public string FirebaseToken { get; set; }
    }
}