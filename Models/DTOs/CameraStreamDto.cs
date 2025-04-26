namespace Vedect.Models.DTOs
{
    public class CameraStreamDto
    {
        public Guid CameraId { get; set; }
        public string CameraName { get; set; } = string.Empty;
        public string StreamURL { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
    }
}
