using Microsoft.AspNetCore.Identity;

namespace Vedect.Models.Domain
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }

        public string VerificationCode { get; set; }

        public DateTime? VerificationCodeExpiration { get; set; }

        public bool IsEmailVerified { get; set; }

        public string? ConnectionId { get; set; }
    }
}
