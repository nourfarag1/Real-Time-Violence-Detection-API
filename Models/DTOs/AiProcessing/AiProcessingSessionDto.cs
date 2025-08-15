using System.Text.Json.Serialization;
using Vedect.Models.Domain; // Assuming AiProcessingStatus is here

namespace Vedect.Models.DTOs.AiProcessing
{
    public class AiProcessingSessionDto
    {
        public Guid CameraId { get; set; }
        public DateTime StartedAtUtc { get; set; }
        public DateTime? EndedAtUtc { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))] 
        public AiProcessingStatus Status { get; set; }
        public string? LastMessageFromPipeline { get; set; }
        public string? ErrorDetails { get; set; }

        // Constructor to map from the domain model
        public AiProcessingSessionDto(AiProcessingSession session)
        {
            CameraId = session.CameraId;
            StartedAtUtc = session.StartedAtUtc;
            EndedAtUtc = session.EndedAtUtc;
            Status = session.Status;
            LastMessageFromPipeline = session.LastMessageFromPipeline;
            ErrorDetails = session.ErrorDetails;
        }
        
        // Parameterless constructor if needed for other purposes, though not strictly for this mapping
        public AiProcessingSessionDto() {}
    }
} 