using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Vedect.Models.DTOs.Notifications;
using Microsoft.Extensions.DependencyInjection;
using Vedect.Services.Interfaces;
using Vedect.Data;
using Microsoft.EntityFrameworkCore;

namespace Vedect.Services.BackgroundServices;

public class NotificationConsumerService : IHostedService, IDisposable
{
    private readonly ILogger<NotificationConsumerService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _scopeFactory;
    private IConnection _connection;
    private IModel _channel;

    public NotificationConsumerService(
        ILogger<NotificationConsumerService> logger, 
        IConfiguration configuration,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _configuration = configuration;
        _scopeFactory = scopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQ:HostName"],
                UserName = _configuration["RabbitMQ:UserName"],
                Password = _configuration["RabbitMQ:Password"],
                DispatchConsumersAsync = true // Important for async consumers
            };
            
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _logger.LogInformation("RabbitMQ connection established.");

            var exchangeName = "event_notifications_exchange";
            var queueName = "backend_notifications_queue";
            var routingKey = "events.violence.*";

            _channel.ExchangeDeclare(exchange: exchangeName, type: "topic", durable: true);
            _logger.LogInformation($"Exchange '{exchangeName}' declared.");

            _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _logger.LogInformation($"Queue '{queueName}' declared.");

            _channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routingKey);
            _logger.LogInformation($"Queue bound to exchange with routing key '{routingKey}'.");

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                

                try
                {
                    var notificationDto = JsonSerializer.Deserialize<ViolenceEventNotification>(message);
                    if (notificationDto != null)
                    {
                        _logger.LogInformation($"Successfully deserialized event for camera {notificationDto.CameraId}. Processing...");
                        
                        using var scope = _scopeFactory.CreateScope();
                        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                        var userIds = await dbContext.UserCameras
                            .Where(uc => uc.CameraId == notificationDto.CameraId)
                            .Select(uc => uc.UserId)
                            .Distinct()
                            .ToListAsync();

                        if (userIds.Any())
                        {
                            _logger.LogInformation($"Found {userIds.Count} users associated with camera {notificationDto.CameraId}. Sending notifications.");
                            
                            var title = "Violence Alert Detected";
                            var notificationBody = $"A violence event was detected on one of your cameras. Tap to view the incident.";
                            
                            foreach (var userId in userIds)
                            {
                                // The notification service already handles finding the specific device tokens for the user.
                                await notificationService.SendNotificationAsync(
                                    userId, 
                                    title, 
                                    notificationBody
                                    // We could pass more data here if the service supported it, e.g., notificationDto.IncidentVideoUrl
                                );
                            }
                        }
                        else
                        {
                            _logger.LogWarning($"No users found for camera {notificationDto.CameraId}.");
                        }
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Failed to deserialize notification message: {Message}", message);
                }
                catch (Exception ex)
                {
                     _logger.LogError(ex, "An unexpected error occurred while processing notification for message: {Message}", message);
                }

                _channel.BasicAck(ea.DeliveryTag, false);
                await Task.Yield();
            };

            _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
            _logger.LogInformation($"Consumer started. Waiting for messages on queue '{queueName}'.");

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred starting the NotificationConsumerService.");
        }
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _channel?.Close();
        _connection?.Close();
        _logger.LogInformation("NotificationConsumerService stopped.");
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
} 