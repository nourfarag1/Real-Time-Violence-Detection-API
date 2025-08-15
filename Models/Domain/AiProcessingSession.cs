using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Vedect.Models.Domain
{
    public enum AiProcessingStatus
    {
        None,
        Initializing,
        Starting,
        Processing,
        StopRequested,
        Stopping,
        Stopped,
        FailedToStart,
        Error
    }

    public class AiProcessingSession
    {
        [Key]
        public Guid Id { get; private set; } // Primary Key for the database table

        public Guid CameraId { get; private set; }
        public DateTime StartedAtUtc { get; private set; }
        public DateTime? EndedAtUtc { get; private set; }
        
        [JsonConverter(typeof(JsonStringEnumConverter))] 
        public AiProcessingStatus Status { get; private set; }
        public string? LastMessageFromPipeline { get; private set; }
        public string? ErrorDetails { get; private set; }

        [NotMapped] // This property will not be stored in the database
        public Process? ChunkingProcess { get; private set; } // Only for in-memory runtime state

        // Parameterless constructor for EF Core
        private AiProcessingSession() 
        {
            // Required by EF Core, ensure default values are sensible if needed
            Id = Guid.NewGuid(); // Or let the DB generate it if configured
            CameraId = Guid.Empty;
            StartedAtUtc = DateTime.UtcNow;
            Status = AiProcessingStatus.None;
        }

        public AiProcessingSession(Guid cameraId) // Constructor for application logic
        {
            if (cameraId == Guid.Empty)
                throw new ArgumentException("Camera ID cannot be empty.", nameof(cameraId));

            Id = Guid.NewGuid(); // Initialize the new PK
            CameraId = cameraId;
            Status = AiProcessingStatus.Initializing;
            StartedAtUtc = DateTime.UtcNow; 
        }

        public void SetChunkingProcess(Process process) // Method for runtime state
        {
            ChunkingProcess = process ?? throw new ArgumentNullException(nameof(process));
            // Status change related to runtime process might not be directly persisted
            // if this is only for the in-memory service's use of this object.
        }

        public void MarkAsStarting(string? messageFromPipeline = null)
        {
            Status = AiProcessingStatus.Starting;
            LastMessageFromPipeline = messageFromPipeline;
        }

        public void MarkAsProcessing(string? messageFromPipeline = null)
        {
            Status = AiProcessingStatus.Processing;
            LastMessageFromPipeline = messageFromPipeline;
            if (Status == AiProcessingStatus.Initializing || StartedAtUtc == DateTime.MinValue) // Ensure StartedAt is set
                StartedAtUtc = DateTime.UtcNow;
        }

        public void MarkAsStopRequested()
        {
            Status = AiProcessingStatus.StopRequested;
        }
        
        public void MarkAsStopping(string? messageFromPipeline = null)
        {
            Status = AiProcessingStatus.Stopping;
            LastMessageFromPipeline = messageFromPipeline;
        }

        public void MarkAsStopped(string? messageFromPipeline = null)
        {
            Status = AiProcessingStatus.Stopped;
            EndedAtUtc = DateTime.UtcNow;
            LastMessageFromPipeline = messageFromPipeline;
        }

        public void MarkAsFailedToStart(string? errorDetails, string? messageFromPipeline = null)
        {
            Status = AiProcessingStatus.FailedToStart;
            ErrorDetails = errorDetails;
            EndedAtUtc = DateTime.UtcNow;
            LastMessageFromPipeline = messageFromPipeline;
        }

        public void MarkAsError(string? errorDetails, string? messageFromPipeline = null)
        {
            Status = AiProcessingStatus.Error;
            ErrorDetails = errorDetails;
            LastMessageFromPipeline = messageFromPipeline;
        }
    }
} 