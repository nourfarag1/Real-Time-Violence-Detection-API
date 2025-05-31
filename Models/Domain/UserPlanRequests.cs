namespace Vedect.Models.Domain
{
    public class UserPlanRequests
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public int RequestedPlanId { get; set; }
        public SubscriptionPlan RequestedPlan { get; set; }

        public string Status { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }

        public string? AdminReviewerId { get; set; }
        public User AdminReviewer { get; set; }
    }
}
