using System.Collections.Concurrent;
using System.Net.Http.Json;
using Vedect.Models.Domain;
using Vedect.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Vedect.Services.Implementations
{
    public class AiProcessingService : IAiProcessingService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AiProcessingService> _logger;
        private readonly ConcurrentDictionary<Guid, AiProcessingSession> _sessions;
        private readonly string _aiPipelineServiceUrl;

        public AiProcessingService(
            IHttpClientFactory httpClientFactory, 
            ILogger<AiProcessingService> logger,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _sessions = new ConcurrentDictionary<Guid, AiProcessingSession>();
            
            // Example: Get URL from appsettings.json, e.g., "AiPipelineService:BaseUrl"
            // Fallback to a default if not configured, adjust port if your docker-compose differs for host access
            _aiPipelineServiceUrl = configuration.GetValue<string>("AiPipelineServiceBaseUrl") 
                                    ?? "http://localhost:8000"; 
            _logger.LogInformation($"AiPipelineService URL set to: {_aiPipelineServiceUrl}");
        }

        public async Task<(bool Success, string Message)> StartProcessingAsync(Guid cameraId)
        {
            if (cameraId == Guid.Empty)
                return (false, "Camera ID cannot be empty.");

            var session = _sessions.GetOrAdd(cameraId, id => new AiProcessingSession(id));

            if (session.Status == AiProcessingStatus.Starting || session.Status == AiProcessingStatus.Processing)
            {
                _logger.LogWarning($"Attempted to start processing for camera {cameraId}, but it's already {session.Status}.");
                return (false, $"AI processing is already active or starting for camera {cameraId}.");
            }
            
            session.MarkAsStarting();
            var client = _httpClientFactory.CreateClient("AiPipelineClient");
            string requestUrl = $"{_aiPipelineServiceUrl}/process/start/{cameraId}";

            try
            {
                _logger.LogInformation($"Sending start request to AI Pipeline for camera {cameraId} at {requestUrl}");
                var response = await client.PostAsync(requestUrl, null); 

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadFromJsonAsync<PipelineResponse>();
                    var message = responseBody?.Message ?? "Successfully requested pipeline to start processing.";
                    session.MarkAsProcessing(message);
                    _logger.LogInformation($"AI Pipeline started processing for camera {cameraId}: {message}");
                    return (true, message);
                }
                else
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    var errorMessage = $"AI Pipeline failed to start processing for camera {cameraId}. Status: {response.StatusCode}. Details: {errorBody}";
                    session.MarkAsFailedToStart(errorMessage, errorBody);
                    _logger.LogError(errorMessage);
                    return (false, errorMessage);
                }
            }
            catch (HttpRequestException ex)
            {
                var errorMessage = $"HTTP request to AI Pipeline failed for camera {cameraId}: {ex.Message}";
                session.MarkAsFailedToStart(errorMessage, ex.ToString());
                _logger.LogError(ex, errorMessage);
                return (false, errorMessage);
            }
            catch (Exception ex) // Catch other potential errors like TaskCanceledException from timeout
            {
                var errorMessage = $"An unexpected error occurred while trying to start AI processing for camera {cameraId}: {ex.Message}";
                session.MarkAsFailedToStart(errorMessage, ex.ToString());
                _logger.LogError(ex, errorMessage);
                return (false, errorMessage);
            }
        }

        public async Task<(bool Success, string Message)> StopProcessingAsync(Guid cameraId)
        {
            if (cameraId == Guid.Empty)
                return (false, "Camera ID cannot be empty.");

            if (!_sessions.TryGetValue(cameraId, out var session) || 
                session.Status == AiProcessingStatus.Stopped || 
                session.Status == AiProcessingStatus.FailedToStart)
            {
                _logger.LogWarning($"Attempted to stop processing for camera {cameraId}, but no active or stoppable session found.");
                return (false, "No active AI processing session found to stop for this camera.");
            }

            session.MarkAsStopRequested();
            var client = _httpClientFactory.CreateClient("AiPipelineClient");
            string requestUrl = $"{_aiPipelineServiceUrl}/process/stop/{cameraId}";

            try
            {
                _logger.LogInformation($"Sending stop request to AI Pipeline for camera {cameraId} at {requestUrl}");
                var response = await client.PostAsync(requestUrl, null);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadFromJsonAsync<PipelineResponse>();
                    var message = responseBody?.Message ?? "Successfully requested pipeline to stop processing.";
                    session.MarkAsStopped(message);
                    _logger.LogInformation($"AI Pipeline stopped processing for camera {cameraId}: {message}");
                    return (true, message);
                }
                // Handle 404 from pipeline if it didn't know about the camera_id
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                     var message = $"AI Pipeline reported no active processing for camera {cameraId} to stop. Marking as stopped.";
                     session.MarkAsStopped(message); // Assume it's stopped from our perspective
                    _logger.LogWarning(message);
                    return (true, message); // Or (false, message) depending on desired strictness
                }
                else
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    var errorMessage = $"AI Pipeline failed to stop processing for camera {cameraId}. Status: {response.StatusCode}. Details: {errorBody}";
                    session.MarkAsError(errorMessage, errorBody); // Not necessarily "stopped" if pipeline errored
                    _logger.LogError(errorMessage);
                    return (false, errorMessage);
                }
            }
            catch (HttpRequestException ex)
            {
                var errorMessage = $"HTTP request to AI Pipeline failed for camera {cameraId}: {ex.Message}";
                session.MarkAsError(errorMessage, ex.ToString());
                _logger.LogError(ex, errorMessage);
                return (false, errorMessage);
            }
             catch (Exception ex) 
            {
                var errorMessage = $"An unexpected error occurred while trying to stop AI processing for camera {cameraId}: {ex.Message}";
                session.MarkAsError(errorMessage, ex.ToString());
                _logger.LogError(ex, errorMessage);
                return (false, errorMessage);
            }
        }

        public Task<AiProcessingSession?> GetSessionAsync(Guid cameraId)
        {
            _sessions.TryGetValue(cameraId, out var session);
            return Task.FromResult(session);
        }

        public Task<IReadOnlyList<AiProcessingSession>> GetAllSessionsAsync()
        {
            // Returns a snapshot of current sessions. 
            // Consider if you need to filter by status (e.g., only active/recently active)
            IReadOnlyList<AiProcessingSession> allSessions = _sessions.Values.ToList();
            return Task.FromResult(allSessions);
        }

        // Helper class for deserializing pipeline responses
        private class PipelineResponse
        {
            public string? Message { get; set; }
            // Add other fields if your pipeline returns more data
        }
    }
} 