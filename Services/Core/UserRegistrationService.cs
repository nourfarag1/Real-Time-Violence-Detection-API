using Microsoft.AspNetCore.Identity;
using Vedect.Models.Domain;
using Vedect.Models.DTOs;
using Vedect.Shared;

namespace Vedect.Services.Core
{
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRegistrationService(UserManager<User> userManager, IEmailSender emailSender, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        public async Task RegisterUserAsync(RegisterDto dto)
        {
            var user = new User
            {
                UserName = dto.Username,
                Email = dto.Email,
                FullName = dto.Fullname,
                VerificationCode = GenerateVerificationCode(),
                VerificationCodeExpiration = DateTime.Now.AddMinutes(10),
                IsEmailVerified = false,
                SubscriptionPlanId = 0
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            var defaultRole = "User";

            await _userManager.AddToRoleAsync(user, defaultRole);

            await _emailSender.SendVerificationEmailAsync(user.Email, user.VerificationCode);
        }

        private string GenerateVerificationCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}
