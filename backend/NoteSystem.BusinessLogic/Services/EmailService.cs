using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using NoteSystem.Core.Interfaces;
using System.Net.Mail;

namespace NoteSystem.BusinessLogic.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_configuration["Smtp:From"]));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

        using var smtp = new MailKit.Net.Smtp.SmtpClient();

        string host = _configuration["Smtp:Host"];
        int port = int.Parse(_configuration["Smtp:Port"]);

        var connectTask = smtp.ConnectAsync(host, port);

        if (await Task.WhenAny(connectTask, Task.Delay(15000)) != connectTask)
            throw new TimeoutException("SMTP connect timed out");

        await smtp.AuthenticateAsync(
            _configuration["Smtp:Username"],
            _configuration["Smtp:Password"]);

        var sendTask = smtp.SendAsync(email);
        if (await Task.WhenAny(sendTask, Task.Delay(15000)) != sendTask)
            throw new TimeoutException("SMTP send timed out");

        await smtp.DisconnectAsync(true);

    }

}
