using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace TheBestBean.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly EmailOptions _options;
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(EmailOptions options, ILogger<SmtpEmailSender> logger)
        {
            _options = options;
            _logger = logger;
        }

        public bool IsConfigured => _options.IsConfigured;

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if (!IsConfigured)
            {
                _logger.LogWarning("SMTP is not configured (secrets/email.json). Not sending '{Subject}' to {Email}.", subject, email);
                return;
            }

            var from = string.IsNullOrWhiteSpace(_options.From) ? _options.User : _options.From;
            using var message = new MailMessage
            {
                From = new MailAddress(from, "Purple Bean Coffee"),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };
            message.To.Add(email);

            using var client = new SmtpClient(_options.Host, _options.Port)
            {
                EnableSsl = _options.UseSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 15000
            };
            if (!string.IsNullOrWhiteSpace(_options.User))
            {
                client.Credentials = new NetworkCredential(_options.User, _options.Password);
            }

            try
            {
                await client.SendMailAsync(message);
                _logger.LogInformation("Sent '{Subject}' to {Email}.", subject, email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SMTP failed sending '{Subject}' to {Email}.", subject, email);
                throw;
            }
        }
    }
}
