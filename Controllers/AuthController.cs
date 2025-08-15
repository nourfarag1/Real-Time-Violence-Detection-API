using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Vedect.Models.Domain;
using Vedect.Models.DTOs;
using Vedect.Services.Core;
using Vedect.Shared;

namespace Vedect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly JWTService _jwtService;

        public AuthController(UserManager<User> userManager, IEmailSender emailSender, SignInManager<User> signInManager, IUserRegistrationService userRegistrationService, IConfiguration configuration, JWTService jWTService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailSender = emailSender;
            _userRegistrationService = userRegistrationService;
            _jwtService = jWTService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            var user = await _userManager.FindByNameAsync(login.UserName);

            if(user == null)
            {
                return BadRequest("This username does not exist");
            }

            if (!await _userManager.CheckPasswordAsync(user, login.Password))
            {
                return Unauthorized();
            }

            var token = await _jwtService.GenerateJwtToken(user);
            return Ok(new { token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var users = await _userManager.Users.Where(u => u.Email == dto.Email).ToListAsync();

            if (users.Count >= 1)
            {
                return BadRequest("You cannot have more than one account with the same email.");
            }

            var user = await _userManager.Users.Where(u => u.UserName == dto.Username).FirstOrDefaultAsync();

            if (user != null)
            {
                return Problem(
                    detail: "Usename already exists",
                    statusCode: 402,
                    title: "BadRequest"
                );
            }

            try
            {
                await _userRegistrationService.RegisterUserAsync(dto);
                return Ok(new { message = "A verification code has been sent to your email." });
            }

            catch (Exception ex)
            {
                return Problem(
                    detail: ex.Message,
                    statusCode: 401,
                    title: "BadRequest"
                );
            }
        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromBody] VerificationDto dto)
        {
            // Find users by email
            var users = await _userManager.Users.Where(u => u.Email == dto.Email).ToListAsync();

            var user = users.Where(x => x.IsEmailVerified == false).SingleOrDefault(); // We can safely assume at least one user will be found here.

            if (user == null || user.VerificationCode != dto.VerificationCode || user.VerificationCodeExpiration < DateTime.Now)
            {
                return BadRequest("Invalid or expired verification code.");
            }

            // Proceed with the rest of the process if the email verification is successful
            user.EmailConfirmed = true;
            user.IsEmailVerified = true;
            user.VerificationCode = "";
            user.VerificationCodeExpiration = DateTime.Now.AddMinutes(15);

            // Update the user record
            await _userManager.UpdateAsync(user);

            return Ok(new { message = "Email verified successfully." });
        }

        [HttpGet("external-login")]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Auth", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet("external-login-callback")]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                return BadRequest($"Error from external provider: {remoteError}");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return BadRequest("Error loading external login information.");
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);

            if (result.Succeeded)
            {
                // ✅ Login success
                return Ok("Logged in via Google");
            }

            // User doesn't have an account yet, so let's create one
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = new User
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var identityResult = await _userManager.CreateAsync(user);
            if (identityResult.Succeeded)
            {
                await _userManager.AddLoginAsync(user, info);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return Ok("Account created and logged in via Google");
            }

            return BadRequest("Error creating user account.");
        }
    }
}
