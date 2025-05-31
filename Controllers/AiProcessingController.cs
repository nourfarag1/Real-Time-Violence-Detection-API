using Microsoft.AspNetCore.Mvc;
using Vedect.Models.Domain;
using Vedect.Models.DTOs.AiProcessing;
using Vedect.Services.Interfaces;

namespace Vedect.Controllers
{
    [Route("api/aiprocessing")] // Changed base route for clarity
    [ApiController]
    public class AiProcessingController : ControllerBase
    {
        private readonly IAiProcessingService _aiProcessingService;
        private readonly ILogger<AiProcessingController> _logger;

        public AiProcessingController(
            IAiProcessingService aiProcessingService,
            ILogger<AiProcessingController> logger)
        {
            _aiProcessingService = aiProcessingService;
            _logger = logger;
        }

        /// <summary>
        /// Starts AI processing for a specific camera.
        /// </summary>
        /// <param name="cameraId">The unique identifier of the camera.</param>
        /// <remarks>
        /// This endpoint will trigger the AI pipeline to start chunking the camera's stream.
        /// </remarks>
        /// <response code="200">AI processing successfully initiated or already active.</response>
        /// <response code="400">Invalid request (e.g., bad camera ID, pipeline service error).</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost("{cameraId}/start")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> StartProcessing(Guid cameraId)
        {
            if (cameraId == Guid.Empty)
                return BadRequest(new { message = "Camera ID cannot be empty." });

            try
            {
                var (success, message) = await _aiProcessingService.StartProcessingAsync(cameraId);
                var sessionDomain = await _aiProcessingService.GetSessionAsync(cameraId);
                var sessionDto = sessionDomain != null ? new AiProcessingSessionDto(sessionDomain) : null;

                if (success)
                {
                    // If the pipeline says it's already active, that's also a form of success from API perspective
                    return Ok(new { message, sessionStatus = sessionDto?.Status.ToString(), sessionDetails = sessionDto });
                }
                else
                {
                    // Differentiate between a bad request and a failure to start in the pipeline
                    if (sessionDomain?.Status == AiProcessingStatus.FailedToStart)
                    {
                         _logger.LogWarning($"Failed to start AI processing for camera {cameraId}: {message}");
                        return BadRequest(new { message, errorDetails = sessionDto?.ErrorDetails, sessionStatus = sessionDto?.Status.ToString() });
                    }
                    return BadRequest(new { message, sessionStatus = sessionDto?.Status.ToString() });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error in StartProcessing for camera {cameraId}.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An internal server error occurred.", error = ex.Message });
            }
        }

        /// <summary>
        /// Stops AI processing for a specific camera.
        /// </summary>
        /// <param name="cameraId">The unique identifier of the camera.</param>
        /// <remarks>
        /// This endpoint will trigger the AI pipeline to stop chunking the camera's stream.
        /// </remarks>
        /// <response code="200">AI processing successfully stopped or no active session was found.</response>
        /// <response code="400">Error stopping processing or bad request.</response>
        /// <response code="404">No stoppable AI processing session found for the camera.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost("{cameraId}/stop")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> StopProcessing(Guid cameraId)
        {
            if (cameraId == Guid.Empty)
                return BadRequest(new { message = "Camera ID cannot be empty." });

            try
            {
                var (success, message) = await _aiProcessingService.StopProcessingAsync(cameraId);
                var sessionDomain = await _aiProcessingService.GetSessionAsync(cameraId);
                var sessionDto = sessionDomain != null ? new AiProcessingSessionDto(sessionDomain) : null;

                if (success)
                {
                    return Ok(new { message, sessionStatus = sessionDto?.Status.ToString(), sessionDetails = sessionDto });
                }
                else
                {
                    // Check if the reason for failure was "not found" in the service layer logic
                    if (message.ToLower().Contains("no active ai processing session found"))
                    {
                        return NotFound(new { message, sessionStatus = sessionDto?.Status.ToString() });
                    }
                    _logger.LogWarning($"Failed to stop AI processing for camera {cameraId}: {message}");
                    return BadRequest(new { message, errorDetails = sessionDto?.ErrorDetails, sessionStatus = sessionDto?.Status.ToString() });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error in StopProcessing for camera {cameraId}.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An internal server error occurred.", error = ex.Message });
            }
        }

        /// <summary>
        /// Gets the current AI processing status for a specific camera.
        /// </summary>
        /// <param name="cameraId">The unique identifier of the camera.</param>
        /// <response code="200">Returns the session details.</response>
        /// <response code="404">No session found for the camera.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("{cameraId}/status")]
        [ProducesResponseType(typeof(AiProcessingSessionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProcessingStatus(Guid cameraId)
        {
            if (cameraId == Guid.Empty)
                return BadRequest(new { message = "Camera ID cannot be empty." });
            try
            {
                var sessionDomain = await _aiProcessingService.GetSessionAsync(cameraId);
                if (sessionDomain == null)
                {
                    return NotFound(new { message = $"No AI processing session found for camera {cameraId}." });
                }
                return Ok(new AiProcessingSessionDto(sessionDomain));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting AI processing status for camera {cameraId}.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An internal server error occurred.", error = ex.Message });
            }
        }

        /// <summary>
        /// Gets a list of all currently tracked AI processing sessions.
        /// </summary>
        /// <response code="200">Returns a list of all sessions and their statuses.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("sessions/all")] // Changed route for clarity
        [ProducesResponseType(typeof(IEnumerable<AiProcessingSessionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllSessions()
        {
            try
            {
                var sessionsDomain = await _aiProcessingService.GetAllSessionsAsync();
                var sessionsDto = sessionsDomain.Select(s => new AiProcessingSessionDto(s)).ToList();
                return Ok(new { totalSessions = sessionsDto.Count, sessions = sessionsDto });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all AI processing sessions.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An internal server error occurred.", error = ex.Message });
            }
        }
    }
} 