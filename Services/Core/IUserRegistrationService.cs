using Vedect.Models.DTOs;

namespace Vedect.Services.Core
{
    public interface IUserRegistrationService
    {
        Task RegisterUserAsync(RegisterDto dto);
    }
}
