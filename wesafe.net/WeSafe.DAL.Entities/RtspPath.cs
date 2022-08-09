namespace WeSafe.DAL.Entities
{
    public class RtspPath
    {
        public int Id { get; set; }

        public string Path { get; set; }

        public int CameraMarkId { get; set; }

        public virtual CameraMark CameraMark { get; set; }
    }
}
