using FirebaseAdmin.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vedect.Data;
using Vedect.Models.Domain;
using Vedect.Services.Interfaces;

namespace Vedect.Services.Implementations;

public class FirebaseNotificationService : INotificationService
{
    private readonly AppDbContext _context;
    private readonly ILogger<FirebaseNotificationService> _logger;

    public FirebaseNotificationService(AppDbContext context, ILogger<FirebaseNotificationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task RegisterDeviceAsync(string userId, string fcmToken)
    {
        // Check if this token already exists for any user to avoid duplicates
        var tokenExists = await _context.UserDevices.AnyAsync(ud => ud.FcmToken == fcmToken);

        if (tokenExists)
        {
            // Optional: If the token exists but is now associated with a new user,
            // you might want to re-assign it. For now, we'll just ignore it.
            return;
        }

        var userDevice = new UserDevice
        {
            UserId = userId,
            FcmToken = fcmToken,
            CreatedAt = DateTime.UtcNow
        };

        await _context.UserDevices.AddAsync(userDevice);
        await _context.SaveChangesAsync();
    }

    public async Task SendNotificationAsync(
        string userId, 
        string title, 
        string body, 
        Dictionary<string, string>? metadata = null,
        bool dryRun = false)
    {
        _logger.LogInformation("Attempting to send notification to UserId: {UserId}", userId);

        var userDevices = await _context.UserDevices
            .Where(ud => ud.UserId == userId)
            .Select(ud => ud.FcmToken)
            .ToListAsync();

        _logger.LogInformation("Found {DeviceCount} device token(s) for UserId: {UserId}", userDevices.Count, userId);

        if (userDevices.Count == 0)
        {
            _logger.LogWarning("No registered FCM tokens found for UserId: {UserId}. Notification will not be sent.", userId);
            return; // No registered devices for this user
        }

        var message = new MulticastMessage()
        {
            Tokens = userDevices,
            Notification = new Notification
            {
                Title = title,
                Body = body,
            },
            Data = metadata ?? new Dictionary<string, string>()
        };

        _logger.LogInformation("Sending multicast message to FCM for UserId: {UserId}. DryRun: {DryRun}", userId, dryRun);
        await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message, dryRun);
        _logger.LogInformation("Successfully sent multicast message to FCM for UserId: {UserId}", userId);
    }
} 