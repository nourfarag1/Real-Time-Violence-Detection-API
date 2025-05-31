namespace Vedect.Models.Domain
{
    public class CameraStreamSession
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CameraId { get; set; }
        public Camera Camera { get; set; }
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? EndedAt { get; set; }
        public string StreamKey { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
