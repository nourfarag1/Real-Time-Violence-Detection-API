namespace Vedect.Models.Domain
{
    public class CameraStreamSession
    {
        public Guid Id { get; set; }
        public Guid CameraId { get; set; }
        public string StreamType { get; set; } // "flutter" or "ai"
        public DateTime StartedAt { get; set; }
        //public bool IsActive { get; set; }
        //public string? FfmpegProcessId { get; set; } // optional
    }
}
