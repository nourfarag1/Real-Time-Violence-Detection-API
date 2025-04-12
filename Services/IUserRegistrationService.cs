    using Vedect.Models.DTOs;

    namespace Vedect.Services
    {
        public interface IUserRegistrationService
        {
            Task RegisterUserAsync(RegisterDto dto);
        }
    }
