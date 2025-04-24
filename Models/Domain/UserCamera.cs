namespace Vedect.Models.Domain
{
    public class UserCamera
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        public User? User { get; set; }

        public Guid CameraId { get; set; }
        public Camera? Camera { get; set; }
    }
}
