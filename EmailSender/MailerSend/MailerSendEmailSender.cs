using LuckyShot.Domain.Services;
using Microsoft.Extensions.Configuration;

namespace EmailSender.MailerSend;

public class MailerSendEmailSender(IMailerSendApi mailerSendApi, IConfiguration configuration) : IEmailSender
{
    private readonly string _senderEmail = GetRequiredConfig(configuration, key: "Email:SenderEmail");
    private readonly string _senderName = GetRequiredConfig(configuration, key: "Email:SenderName");

    public async Task SendAsync(string toEmail, string subject, string htmlBody)
    {
        var request = new MailerSendEmailRequest(
            new MailerSendSender(_senderEmail, _senderName),
            [new MailerSendRecipient(toEmail)],
            subject,
            htmlBody
        );

        await mailerSendApi.SendEmailAsync(request);
    }

    private static string GetRequiredConfig(IConfiguration configuration, string key)
    {
        var value = configuration[key];
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"{key} must be configured.");
        }

        return value;
    }
}