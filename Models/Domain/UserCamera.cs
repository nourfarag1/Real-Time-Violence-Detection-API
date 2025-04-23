namespace Vedect.Models.Domain
{
    public class UserCamera
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int CameraId { get; set; }
        public Camera Camera { get; set; }
    }
}
