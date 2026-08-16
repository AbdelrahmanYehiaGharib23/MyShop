using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MyShop.BLL.Services.IdentityServices
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(
            IConfiguration config,
            ILogger<EmailSender> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendEmailAsync(
            string email,
            string subject,
            string htmlMessage)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException(
                    "Recipient email is required.",
                    nameof(email));

            var smtpHost = _config["MailSettings:Host"];
            var smtpPort = int.TryParse(
                _config["MailSettings:Port"],
                out var port)
                ? port
                : 587;

            var smtpUser = _config["MailSettings:Email"];
            var smtpPassword = _config["MailSettings:Password"];

            if (string.IsNullOrWhiteSpace(smtpHost) ||
                string.IsNullOrWhiteSpace(smtpUser) ||
                string.IsNullOrWhiteSpace(smtpPassword))
            {
                throw new InvalidOperationException(
                    "SMTP settings are not configured.");
            }

            try
            {
                var message = new MimeMessage();

                message.From.Add(
                    new MailboxAddress("MyShop", smtpUser));

                message.To.Add(
                    MailboxAddress.Parse(email));

                message.Subject = subject;

                message.Body = new BodyBuilder
                {
                    HtmlBody = htmlMessage
                }.ToMessageBody();

                using var smtpClient = new SmtpClient();

                await smtpClient.ConnectAsync(
                    smtpHost,
                    smtpPort,
                    SecureSocketOptions.StartTls);

                await smtpClient.AuthenticateAsync(
                    smtpUser,
                    smtpPassword);

                await smtpClient.SendAsync(message);

                await smtpClient.DisconnectAsync(true);

                _logger.LogInformation(
                    "Email sent successfully to {To}",
                    email);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to send email to {To}",
                    email);

                throw;
            }
        }
    }
}