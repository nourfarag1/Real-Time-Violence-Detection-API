namespace Vedect.Models.Domain;

public class UserDevice
{
    public int Id { get; set; }
    
    public string FcmToken { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string UserId { get; set; } = null!;
    public virtual User User { get; set; } = null!;
} 