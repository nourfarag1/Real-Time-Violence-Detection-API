namespace Vedect.Services.Interfaces;

public interface INotificationService
{
    Task RegisterDeviceAsync(string userId, string fcmToken);

    Task SendNotificationAsync(
        string userId, 
        string title, 
        string body, 
        Dictionary<string, string>? metadata = null,
        bool dryRun = false
    );
} 