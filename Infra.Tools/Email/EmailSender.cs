using Domain.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Infra.Tools.Email;

public class EmailSender(IOptions<SmtpOption> options, ILogger<EmailSender> logger)   : IEmailSender
{
    // maybe use options/ fluent validator
    private readonly SmtpOption _options = options?.Value ?? throw new Exception("No email options found");
    public Task SendEmailAsync(string email, string subject, string message)
    {
        var mailMessage = new MimeMessage();
        mailMessage.From.Add(new MailboxAddress(_options.SenderName, _options.SenderEmail));
        mailMessage.To.Add(new MailboxAddress(email, email));
        mailMessage.Subject = subject;
        mailMessage.Body = new TextPart("plain")
        {
            Text = message
        };

        using (var smtpClient = new SmtpClient())
        {
            smtpClient.Connect(_options.SmtpServer, _options.Port, true);
            smtpClient.Authenticate(_options.SenderEmail, _options.SenderPassword);
            smtpClient.Send(mailMessage);
            smtpClient.Disconnect(true);
        }
        logger.LogInformation(@"for email {email}, here is the token {message}", email, message);
        return Task.CompletedTask;
    }
}