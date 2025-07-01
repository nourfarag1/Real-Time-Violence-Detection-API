using System;
using System.Text.Json.Serialization;

namespace Vedect.Models.DTOs.Notifications;

public class ViolenceEventNotification
{
    [JsonPropertyName("camera_id")]
    public Guid CameraId { get; set; }

    [JsonPropertyName("event_type")]
    public string EventType { get; set; } // "violence_detected" or "warning_detected"

    [JsonPropertyName("event_timestamp_utc")]
    public DateTime EventTimestampUtc { get; set; }

    [JsonPropertyName("incident_video_url")]
    public string IncidentVideoUrl { get; set; }

    [JsonPropertyName("thumbnail_url")]
    public string ThumbnailUrl { get; set; }

    [JsonPropertyName("message_version")]
    public string MessageVersion { get; set; }
} 