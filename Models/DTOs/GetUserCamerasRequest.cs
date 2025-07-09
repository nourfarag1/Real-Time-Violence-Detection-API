namespace Vedect.Models.DTOs
{
    public class GetUserCamerasRequest
    {
        public string CameraName { get; set; }
        public Guid CameraId { get; set; }
        public string StreamUrl { get; set; }
    }
}
