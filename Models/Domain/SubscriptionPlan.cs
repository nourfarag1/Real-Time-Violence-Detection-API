namespace Vedect.Models.Domain
{
    public class SubscriptionPlan
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public bool EnableStreaming { get; set; }
        public bool EnableFullStreamStorage { get; set; }
        public bool EnableAIDetection { get; set; }
        public bool EnableAIChunkStorage { get; set; }

        public int FullStreamRetentionHours { get; set; }
        public int AIChunkRetentionHours { get; set; }

        public long MaxTotalStorageMB { get; set; }
    }
}
