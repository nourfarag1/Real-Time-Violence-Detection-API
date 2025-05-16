using Vedect.Models.Domain;

public class Camera
{
    public Guid Id { get; set; }
    public string CameraName { get; set; }
    public string StreamUrl { get; set; }
 
    public ICollection<UserCamera> UserCameras { get; set; } = new List<UserCamera>();
}
