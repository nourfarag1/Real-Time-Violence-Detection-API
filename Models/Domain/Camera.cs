namespace Vedect.Models.Domain
{
    public class Camera
    {
        public int Id { get; set; }
        public string CameraName { get; set; }
        public string CameraType { get; set; }
        public string StreamURL { get; set; }
        public bool IsOnline { get; set; }
        public DateTime LastChecked { get; set; }

        public ICollection<UserCamera> UserCameras { get; set; }
    }
}
