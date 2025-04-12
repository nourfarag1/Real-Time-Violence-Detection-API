using System.Net.Mail;
using System.Net;

namespace Vedect.Shared
{
    public class EmailSender: IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendVerificationEmailAsync(string email, string verificationCode)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings").Get<SMTPSettings>();

            using (var client = new SmtpClient(smtpSettings.SmtpServer, smtpSettings.SmtpPort))
            {
                client.Credentials = new NetworkCredential(smtpSettings.SmtpUsername, smtpSettings.SmtpPassword);
                client.EnableSsl = true;

                var message = new MailMessage(
                from: smtpSettings.SmtpUsername,
                to: email,
                subject: "Your Verification Code",
                body: $"Here is your verification code: {verificationCode}"
            );
                await client.SendMailAsync(message);
            }
        }
    }
}
