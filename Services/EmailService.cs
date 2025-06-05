using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Sep490_Eduseen_BE.Repositories;

namespace Sep490_Eduseen_BE.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(to))
            {
                throw new ArgumentException("Recipient email address cannot be null or empty.", nameof(to));
            }
            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentException("Email subject cannot be null or empty.", nameof(subject));
            }
            if (string.IsNullOrWhiteSpace(body))
            {
                throw new ArgumentException("Email body cannot be null or empty.", nameof(body));
            }

            var smtpSettings = _configuration.GetSection("SmtpSettings");
            if (!smtpSettings.Exists())
            {
                _logger.LogError("SMTP configuration section 'SmtpSettings' is missing in appsettings.json.");
                throw new InvalidOperationException("SMTP configuration section 'SmtpSettings' is missing.");
            }

            var host = smtpSettings["SmtpServer"] ?? throw new InvalidOperationException("SMTP Host not configured in 'SmtpSettings:SmtpServer'.");
            if (!int.TryParse(smtpSettings["Port"], out var port))
            {
                _logger.LogError("Invalid or missing SMTP Port in 'SmtpSettings:Port'.");
                throw new InvalidOperationException("Invalid or missing SMTP Port in 'SmtpSettings:Port'.");
            }
            var enableSsl = bool.TryParse(smtpSettings["EnableSsl"], out var parsedSsl) && parsedSsl;
            var username = smtpSettings["Username"] ?? throw new InvalidOperationException("SMTP Username not configured in 'SmtpSettings:Username'.");
            var password = smtpSettings["Password"] ?? throw new InvalidOperationException("SMTP Password not configured in 'SmtpSettings:Password'.");

            try
            {
                _logger.LogInformation("Sending email to {To} with subject: {Subject}. SMTP Host: {Host}, Port: {Port}, EnableSsl: {EnableSsl}, Username: {Username}", to, subject, host, port, enableSsl, username);

                using var smtpClient = new SmtpClient(host, port)
                {
                    EnableSsl = enableSsl,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(username, password)
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(username, "Eduseen Application"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(to);

                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation("Email sent successfully to {To}", to);
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}. SMTP error: {Message}", to, ex.Message);
                throw new InvalidOperationException($"Failed to send email due to SMTP server error: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while sending email to {To}: {Message}", to, ex.Message);
                throw new InvalidOperationException("An unexpected error occurred while sending email.", ex);
            }
        }
    }
}