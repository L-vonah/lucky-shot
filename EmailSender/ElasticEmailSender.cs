using LuckyShot.Domain.Services;
using Microsoft.Extensions.Configuration;

namespace EmailSender;

public class ElasticEmailSender(IElasticEmailApi elasticEmailApi, IConfiguration configuration) : IEmailSender
{
    private readonly string _senderEmail = GetRequiredConfig(configuration, "Email:Elastic:SenderEmail");
    private readonly string _senderName = GetRequiredConfig(configuration, "Email:Elastic:SenderName");

    public async Task SendAsync(string toEmail, string subject, string htmlBody)
    {
        var from = $"{_senderName} <{_senderEmail}>";
        var request = new ElasticEmailRequest(
            new ElasticEmailRecipients(To: [toEmail]),
            new ElasticEmailContent(
                from,
                subject,
                Body: [new ElasticEmailBody(ContentType: "HTML", Content: htmlBody)]
            )
        );

        await elasticEmailApi.SendTransactionalAsync(request);
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