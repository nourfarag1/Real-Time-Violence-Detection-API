namespace Vedect.Shared
{
    public interface IEmailSender
    {
        Task SendVerificationEmailAsync(string email, string verificationCode);
    }
}
