using System.Text.Json.Serialization;
using Refit;

namespace EmailSender;

public interface IMailerSendApi
{
    [Post("/v1/email")]
    Task SendEmailAsync([Body] MailerSendEmailRequest request);
}

public record MailerSendEmailRequest(
    [property: JsonPropertyName("from")] MailerSendSender From,
    [property: JsonPropertyName("to")] List<MailerSendRecipient> To,
    [property: JsonPropertyName("subject")] string Subject,
    [property: JsonPropertyName("html")] string Html
);

public record MailerSendSender(
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("name")] string Name
);

public record MailerSendRecipient(
    [property: JsonPropertyName("email")] string Email
);