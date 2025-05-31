using Vedect.Models.Domain;

namespace Vedect.Services.Interfaces
{
    public interface IAiProcessingService
    {
        /// <summary>
        /// Initiates AI processing for a specific camera.
        /// </summary>
        /// <param name="cameraId">The unique identifier for the camera.</param>
        /// <returns>A tuple indicating success and a message. The session details can be retrieved via GetSessionAsync.</returns>
        Task<(bool Success, string Message)> StartProcessingAsync(Guid cameraId);

        /// <summary>
        /// Stops AI processing for a specific camera.
        /// </summary>
        /// <param name="cameraId">The unique identifier for the camera.</param>
        /// <returns>A tuple indicating success and a message.</returns>
        Task<(bool Success, string Message)> StopProcessingAsync(Guid cameraId);

        /// <summary>
        /// Retrieves the current AI processing session for a specific camera.
        /// </summary>
        /// <param name="cameraId">The unique identifier for the camera.</param>
        /// <returns>The AiProcessingSession object if found; otherwise, null.</returns>
        Task<AiProcessingSession?> GetSessionAsync(Guid cameraId);

        /// <summary>
        /// Retrieves all currently active or recently managed AI processing sessions.
        /// </summary>
        /// <returns>A read-only list of AiProcessingSession objects.</returns>
        Task<IReadOnlyList<AiProcessingSession>> GetAllSessionsAsync();
    }
} 