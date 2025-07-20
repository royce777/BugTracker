using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace BugTracker.Services;

public class EmailSender : IEmailSender
{
    private readonly ILogger _logger;

    public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor,
                       ILogger<EmailSender> logger)
    {
        Options = optionsAccessor.Value;
        _logger = logger;
    }

    public AuthMessageSenderOptions Options { get; } //Set with Secret Manager.

    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        if (string.IsNullOrEmpty(Options.SmtpUsername) || string.IsNullOrEmpty(Options.SmtpPassword))
        {
            throw new Exception("SMTP credentials not configured");
        }
        await Execute(subject, message, toEmail);
    }

    public async Task Execute(string subject, string message, string toEmail)
    {
        try
        {
            using var client = new SmtpClient(Options.SmtpHost, Options.SmtpPort);
            
            // Configure secure connection
            client.EnableSsl = Options.EnableSsl;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(Options.SmtpUsername, Options.SmtpPassword);
            
            // Force TLS 1.2+
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
            
            // Validate SSL certificate
            ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertificate;
            
            // Set timeout
            client.Timeout = 30000; // 30 seconds

            using var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(Options.FromEmail, Options.FromName);
            mailMessage.To.Add(toEmail);
            mailMessage.Subject = subject;
            mailMessage.Body = message;
            mailMessage.IsBodyHtml = true;

            await client.SendMailAsync(mailMessage);
            _logger.LogInformation($"Email to {toEmail} sent successfully via SMTP");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send email to {toEmail}: {ex.Message}");
            throw;
        }
    }

    private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        // Accept only valid certificates from trusted CAs
        return sslPolicyErrors == SslPolicyErrors.None;
    }
}
